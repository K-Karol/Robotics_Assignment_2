namespace KarolK72.LegoAssignment.UI.Views;

public class MainViewLoader : ContentView
{
	private MainViewDesktop _desktopView;
	private MainViewMobile _mobileView;
	public MainViewLoader(MainViewDesktop desktopView, MainViewMobile mobileView)
	{
		_desktopView = desktopView;
		_mobileView = mobileView;

        DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;

        this.Content = DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Portrait ? _mobileView : _desktopView;
    }
    private void Current_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        if (e.DisplayInfo.Orientation == DisplayOrientation.Landscape)
        {
            this.Content = _desktopView;
        }
        else if (e.DisplayInfo.Orientation == DisplayOrientation.Portrait)
        {
            this.Content = _mobileView;
        }
        else
        {
            this.Content = _mobileView;
        }

    }
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if(DeviceInfo.Current.Idiom == DeviceIdiom.Desktop)
        {
            if (width <= 1150)
            {
                this.Content = _mobileView;
            }
            else
            {
                this.Content = _desktopView;
            }
        }

        
    }
}