using KarolK72.LegoAssignment.Library;
using ReactiveUI;
using System;
using System.Reactive;

namespace KarolK72.LegoAssignment.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IEV3CommunicationService _ev3CommunicationService;

        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }

        public bool IsLoading { get; set; } = false;

        public MainWindowViewModel(IEV3CommunicationService eV3CommunicationService)
        {
            _ev3CommunicationService = eV3CommunicationService;
            ConnectCommand = ReactiveCommand.CreateFromTask(async _ => {
                IsLoading = true;
                await _ev3CommunicationService.Connect("ev3dev.local", 5000);
                IsLoading = false;
            });
        }
    }
}