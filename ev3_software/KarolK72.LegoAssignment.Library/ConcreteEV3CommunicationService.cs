using KarolK72.LegoAssignment.Library.Commands;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace KarolK72.LegoAssignment.Library
{
    /// <summary>
    /// Wrapper class that stores registered callbacks.
    /// </summary>
    public class UpstreamCommandRegistered
    {
        public Type CommandType { get; set; }
        public Func<IUpstreamCommand, Task> Handler { get; set; }
    }

    /// <summary>
    /// Disposable object used by <see cref="ConcreteEV3CommunicationService.RegisterHandler(Type, Func{IUpstreamCommand, Task})"/>
    /// that will run an action when disposed that unregisters the callback.
    /// </summary>
    public class RegisteredClassDisposable : IDisposable
    {
        private Action _deleteRegisteredCommandAction;
        public RegisteredClassDisposable(Action deleteRegisteredCommandAction)
        {
            _deleteRegisteredCommandAction = deleteRegisteredCommandAction;
        }
        public void Dispose()
        {
            _deleteRegisteredCommandAction.Invoke();
        }
    }

    /// <summary>
    /// Concrete implementation of the <see cref="IEV3CommunicationService"/>
    /// </summary>
    public class ConcreteEV3CommunicationService : IEV3CommunicationService
    {
        private readonly ILogger<ConcreteEV3CommunicationService> _logger;
        private Socket? _socket = null;
        private bool disposedValue;
        private Thread? _readingThread = null;
        private CancellationTokenSource? _cts = null;

        private Dictionary<int, UpstreamCommandRegistered> _registeredCommands = new Dictionary<int, UpstreamCommandRegistered>();

        public ConcreteEV3CommunicationService(ILogger<ConcreteEV3CommunicationService> logger)
        {
            _logger = logger;
        }

        public async Task Connect(string url, int port)
        {
            IPAddress? ipAddress = null;
            if (!IPAddress.TryParse(url, out ipAddress))
            {
                IPHostEntry ipHostInfo = await Task.Run(() => Dns.GetHostEntry(url));
                ipAddress = ipHostInfo.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                if (ipAddress is null)
                {
                    ipAddress = ipHostInfo.AddressList.First();
                }
            }


            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            _socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await _socket.ConnectAsync(ipEndPoint);
            _cts = new CancellationTokenSource();
            _readingThread = new Thread(() => readingThread(_cts.Token));
            _readingThread.Start();
        }

        public Task Disconnect()
        {
            if (_socket is null)
            {
                return Task.CompletedTask;
            }
            _socket.Close();
            _socket.Dispose();
            _socket = null;
            _cts?.Cancel();
            _readingThread!.Join();
            _readingThread = null;
            return Task.CompletedTask;
        }

        public Task Dispatch(Payload payload)
        {
            string payloadStr = payload.ToString();
            byte[] buffer = Encoding.UTF8.GetBytes(payloadStr);
            return _socket!.SendAsync(buffer, SocketFlags.None);
        }

        public IDisposable? RegisterHandler(Type commandType, Func<IUpstreamCommand, Task> handler)
        {
            var attribute = commandType.GetCustomAttribute<CommandAttribute>(); // The key of the dictionary is the commandID that is configured in the CommandAttribute on the command
            if (attribute is null)
                throw new Exception($"{commandType.Name} does not have an {nameof(CommandAttribute)} attribute");

            if (_registeredCommands.ContainsKey(attribute.CommandID))
            {
                return null;
            }

            _registeredCommands.Add(attribute.CommandID, new UpstreamCommandRegistered() { CommandType = commandType, Handler = handler });
            return new RegisteredClassDisposable(() => _registeredCommands.Remove(attribute.CommandID));
        }

        /// <summary>
        /// This method is used in a dedicated reading thread that reads all data over the socket.
        /// </summary>
        /// <param name="cancellationToken"></param>
        private async void readingThread(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting read");
            while (!cancellationToken.IsCancellationRequested)
            {
                string data = string.Empty;
                byte[] bytes = new byte[1024];
                bool eoc = false;
                while (!eoc)
                {
                    int bytesRec = 0;
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    try
                    {
                        bytesRec = await _socket!.ReceiveAsync(bytes, SocketFlags.None, cancellationToken); //blocking operations so Async version is used as allows to specify a cancellation token
                    }
                    catch (TaskCanceledException tce)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception when receiving");
                        continue;
                    }



                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec); // decodes bytes from the socket as a string
                    if (data.IndexOf(";") > -1)
                    {
                        eoc = true; //if a ; character is found, it signals an EndOfCommand
                    }
                }

                if (eoc) // if a command was identified by the ; character
                {
                    // If the ev3 brick is fast enough, multiple commands can be contained within 1 packet/output of the ReceiveAsync function, so the following code handles that so no data is lost
                    string leftOver = string.Empty;
                    List<string> commands = new List<string>();

                    string firstCommand = data.Substring(0, data.IndexOf(";") + 1);
                    commands.Add(firstCommand);
                    string rest = data.Substring(data.IndexOf(";") + 1);

                    bool reachedEnd = false;
                    while (!reachedEnd)
                    {
                        if (rest.IndexOf(";") > -1)
                        {
                            commands.Add(rest.Substring(0, rest.IndexOf(";") + 1));
                            rest = rest.Substring(rest.IndexOf(";") + 1);
                        }
                        else
                        {
                            if (rest.Length > 0)
                            {
                                leftOver = rest;
                            }
                            reachedEnd = true;
                        }
                    }

                    foreach (string commandString in commands)
                    {
                        Payload? parsedPayload = null;
                        try
                        {
                            parsedPayload = Payload.Parse(commandString); // parses the payload
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Exception thrown when parsing socket data as a Payload\nData:{commandString}");
                        }
                        eoc = false;
                        if (parsedPayload is null)
                        {
                            _logger.LogError($"Failed to parse socket data as a payload\nData:{commandString}");
                            continue;
                        }
                        _logger.LogDebug($"Data parsed sucesfully!\nData:{commandString}\nParsed Payload:{parsedPayload}");

                        if (_registeredCommands.TryGetValue(parsedPayload.CommandID, out var regCommand)) // sees if a handler was registered for the command ID of the payload
                        {
                            IUpstreamCommand? upstreamCommand = Activator.CreateInstance(regCommand.CommandType) as IUpstreamCommand; // Creates an instance of the type that implements IUpstreamCommand that is registered in the callback
                            if (upstreamCommand is null)
                            {
                                _logger.LogError($"Could not create an instance of the command {regCommand.CommandType.Name}");
                            }
                            else
                            {
                                upstreamCommand.ParsePayload(parsedPayload); // passes the payload to the instance of the command so the command can parse the paramaters dict. into properties on the class itself
                                _ = regCommand.Handler.Invoke(upstreamCommand).ConfigureAwait(false); // invokes the async handler using "fire and forget" 
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"No handler registered for command {parsedPayload.CommandID}");
                        }
                    }

                    data = leftOver;
                }
                else
                {
                    break;
                }

            }


        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _socket?.Close();
                    _socket?.Dispose();
                    _cts?.Cancel();
                    _readingThread?.Join();
                    _readingThread = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ConcreteEV3CommunicationService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}
