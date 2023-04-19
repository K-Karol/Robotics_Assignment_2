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
        //private readonly ILogger<ConcreteEV3CommunicationService> _logger;
        Socket? _socket = null;
        private bool disposedValue;

        //public ConcreteEV3CommunicationService(ILogger<ConcreteEV3CommunicationService> logger)
        //{
        //    _logger = logger;
        //}

        public async Task Connect(string url, int port)
        {
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(url);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            _socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await _socket.ConnectAsync(ipEndPoint);
        }

        public Task Disconnect()
        {
            if(_socket is null)
            {
                return Task.CompletedTask;
            }
            _socket.Close();
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _socket?.Dispose();
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
