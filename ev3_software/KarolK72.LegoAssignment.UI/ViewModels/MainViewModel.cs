using KarolK72.LegoAssignment.Library;
using KarolK72.LegoAssignment.Library.Commands.Upstream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KarolK72.LegoAssignment.UI.ViewModels
{
    public class MainViewModel : BasePropertyChangedClass, IDisposable
    {
        private bool disposedValue;

        private readonly IEV3CommunicationService _ev3CommunicationService;

        private string _hostURL = "ev3dev.local";
        public string HostURL { get => _hostURL; set { SetProperty(ref _hostURL, value); OnPropertyChanged(nameof(IsConnectButtonEnabled)); } }

        private int _hostPort = 5000;
        public int HostPort { get => _hostPort; set => SetProperty(ref _hostPort, value); }

        public bool IsConnectButtonEnabled
        {
            get => !_isConnected && (_hostPort > 0 && _hostPort <= 65535 && !string.IsNullOrWhiteSpace(_hostURL) && Uri.IsWellFormedUriString(_hostURL, UriKind.RelativeOrAbsolute))
                || _isConnected;
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                SetProperty(ref _isLoading, value);
                OnPropertyChanged(nameof(ConnectDisconnectCommand));
                OnPropertyChanged(nameof(IsConnectButtonEnabled));
                OnPropertyChanged(nameof(ConnectDisconnectButtonText));

            }
        }

        public string ConnectDisconnectButtonText => !_isConnected ? "Connect" : "Disconnect";
        private bool _isConnected = false;

        private string _statusLabel = "Not Connected";
        public string StatusLabel
        {
            get => _statusLabel;
            set => SetProperty(ref _statusLabel, value);
        }

        //stats

        private int _noOfProcessedItems = 0;
        public int NoOfProcessedItems => _noOfProcessedItems;
        private int _noOfRejectedItems = 0;
        public int NoOfRejectedItems => _noOfRejectedItems;

        public string RejectionRate =>  _noOfProcessedItems > 0 ? $"{(float)_noOfRejectedItems / (float)_noOfProcessedItems:P2}" : "N/A";

        private ICommand _connectCommand;
        private ICommand _disconnectCommand;
        public ICommand ConnectDisconnectCommand => !_isConnected ? _connectCommand : _disconnectCommand;

        public MainViewModel(IEV3CommunicationService ev3CommuncationService)
        {
            _ev3CommunicationService = ev3CommuncationService;
            _ev3CommunicationService.RegisterHandler(typeof(DetectedCommand), (command) =>
            {
                DetectedHandler(command as DetectedCommand);
                return Task.CompletedTask;
            });
            _connectCommand = new Command(
                execute: async () =>
                {
                    IsLoading = true;
                    try
                    {
                        StatusLabel = "Connecting...";
                        await _ev3CommunicationService.Connect(_hostURL, _hostPort);
                        StatusLabel = "Connected";
                        _isConnected = true;
                    } catch (Exception ex)
                    {
                        StatusLabel = $"Failed: {ex.Message}";
                        _isConnected = false;
                    }
                    IsLoading = false;
                });

            _disconnectCommand = new Command(
                execute: async () =>
                {
                    IsLoading = true;
                    try
                    {
                        StatusLabel = "Disconnecting...";
                        await _ev3CommunicationService.Disconnect();
                        StatusLabel = "Disconnected";
                        _isConnected = false;
                    }
                    catch (Exception ex)
                    {
                        StatusLabel = $"Failed: {ex.Message}";
                        _isConnected = false;
                    }
                    IsLoading = false;
                });
        }

        private void DetectedHandler(DetectedCommand detectedCommand)
        {
            _noOfProcessedItems += 1;
            if (detectedCommand.IsRejected)
            {
                _noOfRejectedItems += 1;
            }

            OnPropertyChanged(nameof(NoOfProcessedItems));
            OnPropertyChanged(nameof(NoOfRejectedItems));
            OnPropertyChanged(nameof(RejectionRate));


        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _ev3CommunicationService.Disconnect();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MainViewModel()
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
