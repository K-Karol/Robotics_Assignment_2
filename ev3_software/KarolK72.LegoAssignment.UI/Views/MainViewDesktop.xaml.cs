namespace KarolK72.LegoAssignment.UI.Views
{
    public partial class MainViewDesktop : ContentView
    {
        private readonly ViewModels.MainViewModel _viewModel;
        public MainViewDesktop(ViewModels.MainViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            BindingContext = _viewModel;
        }
    }
}