namespace KarolK72.LegoAssignment.UI.Views
{
    public partial class MainView : ContentPage
    {
        private readonly ViewModels.MainViewModel _viewModel;
        public MainView(ViewModels.MainViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            BindingContext = _viewModel;
        }

        //private void OnCounterClicked(object sender, EventArgs e)
        //{
        //    count++;

        //    if (count == 1)
        //        CounterBtn.Text = $"Clicked {count} time";
        //    else
        //        CounterBtn.Text = $"Clicked {count} times";

        //    SemanticScreenReader.Announce(CounterBtn.Text);
        //}
    }
}