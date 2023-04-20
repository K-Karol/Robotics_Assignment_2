using KarolK72.LegoAssignment.Library;
using KarolK72.LegoAssignment.Library.Commands.Downstream;
using KarolK72.LegoAssignment.Library.Commands.Upstream;
using System.Windows.Input;

namespace KarolK72.LegoAssignment.UI.ViewModels
{
    /// <summary>
    /// Contains all of the business logic of the dashboard, seperated from the UI.
    /// Contains properties that are bound to the UI, including commands
    /// </summary>
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
        public string ConnectDisconnectButtonText => !_isConnected ? "Connect" : "Disconnect";
        private bool _isConnected = false;

        private string _statusLabel = "Not Connected";
        public string StatusLabel
        {
            get => _statusLabel;
            set => SetProperty(ref _statusLabel, value);
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

        // config
        private bool _isBlacklist = false;
        public bool IsBlacklist
        {
            get => _isBlacklist;
            set => SetProperty(ref _isBlacklist, value);
        }

        public class ColourWrapper
        {
            public string Colour { get; set; }
        }


        private List<object> _coloursList = new List<object>();
        public List<object> ColoursList
        {
            get => _coloursList;
            set => SetProperty(ref _coloursList, value);
        }

        private List<ColourWrapper> _colourOptions = new List<ColourWrapper>() {
            new ColourWrapper(){ Colour = "Color.RED"},
            new ColourWrapper(){ Colour = "Color.GREEN"},
            new ColourWrapper(){ Colour = "Color.BLACK"},
            new ColourWrapper(){ Colour = "Color.YELLOW"},
            new ColourWrapper(){ Colour = "Color.BLUE"},
            new ColourWrapper(){ Colour = "Color.WHITE"},
            new ColourWrapper(){ Colour = "Color.BROWN"},
        };

        public List<ColourWrapper> ColourOptions => _colourOptions;

        //stats
        private int _noOfProcessedItems = 0;
        public int NoOfProcessedItems => _noOfProcessedItems;
        private int _noOfRejectedItems = 0;
        public int NoOfRejectedItems => _noOfRejectedItems;
        public string RejectionRate => _noOfProcessedItems > 0 ? $"{(float)_noOfRejectedItems / (float)_noOfProcessedItems:P2}" : "N/A";

        private ICommand _connectCommand;
        private ICommand _disconnectCommand;
        /// <summary>
        /// Either runs the connect or disconnect command based on the state.
        /// </summary>
        public ICommand ConnectDisconnectCommand => !_isConnected ? _connectCommand : _disconnectCommand;
        /// <summary>
        /// Dispatches a <see cref="GetCurrentConfigurationCommand"/> to the robot
        /// </summary>
        public ICommand GetConfigCommand { get; private set; }
        /// <summary>
        /// Dispatches a <see cref="UpdateConfigurationCommand"/> to the robot
        /// </summary>

        /* Unmerged change from project 'KarolK72.LegoAssignment.UI (net7.0-ios)'
        Before:
                public ICommand UpdateConfigurationCommand { get; private set; }


                public MainViewModel(IEV3CommunicationService ev3CommuncationService)
        After:
                public ICommand UpdateConfigurationCommand { get; private set; }


                public MainViewModel(IEV3CommunicationService ev3CommuncationService)
        */

        /* Unmerged change from project 'KarolK72.LegoAssignment.UI (net7.0-windows10.0.19041.0)'
        Before:
                public ICommand UpdateConfigurationCommand { get; private set; }


                public MainViewModel(IEV3CommunicationService ev3CommuncationService)
        After:
                public ICommand UpdateConfigurationCommand { get; private set; }


                public MainViewModel(IEV3CommunicationService ev3CommuncationService)
        */

        /* Unmerged change from project 'KarolK72.LegoAssignment.UI (net7.0-maccatalyst)'
        Before:
                public ICommand UpdateConfigurationCommand { get; private set; }


                public MainViewModel(IEV3CommunicationService ev3CommuncationService)
        After:
                public ICommand UpdateConfigurationCommand { get; private set; }


                public MainViewModel(IEV3CommunicationService ev3CommuncationService)
        */
        public ICommand UpdateConfigurationCommand { get; private set; }


        public MainViewModel(IEV3CommunicationService ev3CommuncationService)
        {

            _ev3CommunicationService = ev3CommuncationService;
            _ev3CommunicationService.RegisterHandler(typeof(DetectedCommand), (command) =>
            {
                DetectedHandler(command as DetectedCommand);
                return Task.CompletedTask;
            });
            _ev3CommunicationService.RegisterHandler(typeof(CurrentConfigurationCommand), (command) =>
            {
                UpdateConfigurationHandler(command as CurrentConfigurationCommand);
                return Task.CompletedTask;
            });
            _connectCommand = new Command(
                execute: async () =>
                {
                    //connects to the robot using the IEV3CommunicationService
                    IsLoading = true;
                    try
                    {
                        StatusLabel = "Connecting...";
                        await _ev3CommunicationService.Connect(_hostURL, _hostPort);
                        StatusLabel = "Connected";
                        _isConnected = true;
                    }
                    catch (Exception ex)
                    {
                        StatusLabel = $"Failed: {ex.Message}";
                        _isConnected = false;
                    }
                    IsLoading = false;
                    OnPropertyChanged(nameof(ConnectDisconnectCommand));
                    OnPropertyChanged(nameof(IsConnectButtonEnabled));
                    OnPropertyChanged(nameof(ConnectDisconnectButtonText));
                });

            _disconnectCommand = new Command(
                execute: async () =>
                {
                    //disconnects from the robot using the IEV3CommunicationService
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
                    OnPropertyChanged(nameof(ConnectDisconnectCommand));
                    OnPropertyChanged(nameof(IsConnectButtonEnabled));
                    OnPropertyChanged(nameof(ConnectDisconnectButtonText));
                });

            GetConfigCommand = new Command(execute: async () =>
            {
                // sends a GetCurrentConfigurationCommand command to the robot
                IsLoading = true;
                await _ev3CommunicationService.Dispatch(new GetCurrentConfigurationCommand().ConvertToPayload());
                IsLoading = false;
            });

            UpdateConfigurationCommand = new Command(execute: async () =>
            {
                // sends a UpdateConfigurationCommand command with configuration details  to the robot
                IsLoading = true;
                await _ev3CommunicationService.Dispatch(new UpdateConfigurationCommand(_isBlacklist, _coloursList.Select(c => ((ColourWrapper)c).Colour).ToList()).ConvertToPayload());
                IsLoading = false;
            });
        }

        /// <summary>
        /// Handler for handling when an item was detected. Used for statistics
        /// </summary>
        /// <param name="detectedCommand"></param>
        private void DetectedHandler(DetectedCommand detectedCommand)
        {
            _noOfProcessedItems += 1;


            if (detectedCommand.IsRejected)
            {
                _noOfRejectedItems += 1;
            }
            else
            {
            }

            //_largeNotificationTimer.Change(200,Timeout.Infinite);

            OnPropertyChanged(nameof(NoOfProcessedItems));
            OnPropertyChanged(nameof(NoOfRejectedItems));
            OnPropertyChanged(nameof(RejectionRate));


        }

        /// <summary>
        /// Handles when the robot sends their configuration so the UI can be updated
        /// </summary>
        /// <param name="currentConfigurationCommand"></param>
        private void UpdateConfigurationHandler(CurrentConfigurationCommand currentConfigurationCommand)
        {
            IsBlacklist = currentConfigurationCommand.IsBlackList;
            ColoursList = currentConfigurationCommand.ColourList.Select(c => _colourOptions.FirstOrDefault(cw => cw.Colour == c) ?? new ColourWrapper() { Colour = c } as object).ToList(); // wraps the colour string in a ColourWrapper that needs to be converted into an object for the UI (idk why it needs to be an object, its a bug?)
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
