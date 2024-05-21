using ScreenTime.classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScreenTime
{
    public partial class AppInfoWindow : Window
    {
        private StackPanel _stackPanelDynamic;


        internal AppInfoWindow(ScreenTimeApp screenTimeApp, string date)
        {
            InitializeComponent();
            AddAppInfoToMainScreen(screenTimeApp, date);
        }




        private void AddAppInfoToMainScreen(ScreenTimeApp screenTimeApp, string date)
        {

            string appName = screenTimeApp.Name;
            string appPath = screenTimeApp.Path;

            // Get and format time in background
            uint backgroundSeconds = screenTimeApp.SecondsInBackground.GetValueOrDefault(date);

            uint secondsInBackground= backgroundSeconds % 60;
            uint minutesInBackground = backgroundSeconds / 60;
            uint hoursInBackground = minutesInBackground / 60;
            string screenTimeAppTimeInBackground;
            if (hoursInBackground > 0)
                screenTimeAppTimeInBackground = string.Format("{0}h {1}m", hoursInBackground, minutesInBackground % 60);
            else if (minutesInBackground > 0)
                screenTimeAppTimeInBackground = string.Format("{0}m {1}s", minutesInBackground, secondsInBackground);
            else
                screenTimeAppTimeInBackground = string.Format("{0}s", backgroundSeconds);


            // Get and format time in focus
            uint focusSeconds = screenTimeApp.SecondsInFocus.GetValueOrDefault(date);

            uint secondsInFocus = focusSeconds % 60;
            uint minutesInFocus = focusSeconds / 60;
            uint hoursInFocus = minutesInFocus / 60;
            string screenTimeAppTimeInFocus;
            if (hoursInFocus > 0)
                screenTimeAppTimeInFocus = string.Format("{0}h {1}m", hoursInFocus, minutesInFocus % 60);
            else if (minutesInFocus > 0)
                screenTimeAppTimeInFocus = string.Format("{0}m {1}s", minutesInFocus, secondsInFocus);
            else
                screenTimeAppTimeInFocus = string.Format("{0}s", focusSeconds);

            uint timesFocused = screenTimeApp.TimesFocused.GetValueOrDefault(date);
            uint timesOpened = screenTimeApp.TimesOpened.GetValueOrDefault(date);

            AppName.Text = appName;
            SecondsInFocus.Text = screenTimeAppTimeInFocus;
            SecondsInBackground.Text = screenTimeAppTimeInBackground;
        }
    }
}
