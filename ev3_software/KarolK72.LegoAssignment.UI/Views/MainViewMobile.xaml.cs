namespace KarolK72.LegoAssignment.UI.Views
{
    public partial class MainViewMobile : ContentView
    {
        private readonly ViewModels.MainViewModel _viewModel;
        public MainViewMobile(ViewModels.MainViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            BindingContext = _viewModel;
        }

    }
}