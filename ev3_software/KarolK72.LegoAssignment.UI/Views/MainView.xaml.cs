namespace KarolK72.LegoAssignment.UI.Views;

public partial class MainView : ContentPage
{
	public MainView(MainViewLoader mainViewLoader)
	{
		InitializeComponent();
		Content = mainViewLoader;

    }
}