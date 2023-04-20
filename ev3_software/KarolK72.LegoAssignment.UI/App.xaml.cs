namespace KarolK72.LegoAssignment.UI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
            MainPage = new AppShell();
        }
    }
}