using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KarolK72.LegoAssignment.Library
{
    public class ConcreteEV3CommunicationService : IEV3CommunicationService
    {
        private readonly ILogger<ConcreteEV3CommunicationService> _logger;
        private Socket? _socket = null;
        private bool disposedValue;
        private Thread? _readingThread = null;
        private CancellationTokenSource? _cts = null;
        public ConcreteEV3CommunicationService(ILogger<ConcreteEV3CommunicationService> logger)
        {
            _logger = logger;
        }

        public async Task Connect(string url, int port)
        {
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(url);
            IPAddress? ipAddress = ipHostInfo.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            if(ipAddress is null)
            {
                ipAddress = ipHostInfo.AddressList.First();
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
            if(_socket is null)
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

        private async void readingThread(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting read");
            while(!cancellationToken.IsCancellationRequested) {
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
                        bytesRec = await _socket!.ReceiveAsync(bytes, SocketFlags.None, cancellationToken);
                    } catch (TaskCanceledException tce)
                    {
                        break;
                    } catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception when receiving");
                        continue;
                    }
                    


                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if(data.IndexOf(";") > -1)
                    {
                        eoc = true;
                    }
                }

                if (eoc)
                {
                    Payload? parsedPayload = null;
                    try
                    {
                        parsedPayload = Payload.Parse(data);
                    } catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Exception thrown when parsing socket data as a Payload\nData:{data}");
                    }
                    eoc = false;
                    if (parsedPayload is null) {
                        _logger.LogError($"Failed to parse socket data as a payload\nData:{data}");
                        data = string.Empty;
                        continue;
                    }
                    _logger.LogDebug($"Data parsed sucesfully!\nData:{data}\nParsed Payload:{parsedPayload}");
                    data = string.Empty;


                } else
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
