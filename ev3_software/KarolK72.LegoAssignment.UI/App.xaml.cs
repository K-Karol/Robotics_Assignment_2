using KarolK72.LegoAssignment.Library;

namespace KarolK72.LegoAssignment.UI
{
    public partial class App : Application
    {
        private readonly IEV3CommunicationService _ev3CommunicationService;
        public App(IEV3CommunicationService ev3CommunicationService)
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
            MainPage = new AppShell();
        }

        //public override void CloseWindow(Window window)
        //{
        //    _ev3CommunicationService.Disconnect();
        //    base.CloseWindow(window);
        //}
    }
}