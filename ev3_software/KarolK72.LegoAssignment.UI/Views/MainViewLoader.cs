namespace KarolK72.LegoAssignment.UI.Views;

/// <summary>
/// Handles changing the view baseed on the orientation, device and window size
/// (so mobile/portrait/narrow window gets <see cref="MainViewMobile"/>
/// whilst desktop/landscape gets <see cref="MainViewDesktop"/>
/// </summary>
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

        if (DeviceInfo.Current.Idiom == DeviceIdiom.Desktop)
        {
            if (width <= 1150) // imagine this as a CSS media query for responsive design
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