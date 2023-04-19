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
        private readonly IEV3CommunicationService _ev3CommunicationService;


        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _label = "Idle";
        private bool disposedValue;

        public string Label
        {
            get => _label;
            set => SetProperty(ref _label, value);
        }

        public ICommand ConnectCommand { private set; get; }
        public ICommand TestCommand { private set; get; }


        public MainViewModel(IEV3CommunicationService ev3CommuncationService)
        {
            _ev3CommunicationService = ev3CommuncationService;
            _ev3CommunicationService.RegisterHandler(typeof(DetectedCommand), (command) =>
            {
                Label = $"Detected: {(command as DetectedCommand).Colour}";
                return Task.CompletedTask;
            });
            ConnectCommand = new Command(
                execute: async () =>
                {
                    Label = "Starting...";
                    IsLoading = true;
                    try
                    {
                        Label = "Connecting...";
                        await _ev3CommunicationService.Connect("ev3dev.local", 5000);
                        Label = "Success";
                    } catch (Exception ex)
                    {
                        Label = "Failed";
                    }
                    IsLoading = false;
                });

            TestCommand = new Command(
                execute: async () =>
                {
                    Label = "Starting to send...";
                    IsLoading = true;
                    try
                    {
                        Label = "Sending...";
                        await _ev3CommunicationService.Dispatch(new Payload() { CommandID = 1, Paramaters = new Dictionary<string, string>() { { "Hello", "World" } } });
                    }
                    catch (Exception ex)
                    {
                        Label = "Failed";
                    }
                    IsLoading = false;
                });
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
