using ScreenTime.classes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ScreenTime
{
    public partial class AppInfoWindow : Window
    {
        internal AppInfoWindow(ScreenTimeApp screenTimeApp, string date)
        {
            InitializeComponent();
            AddAppInfoToMainScreen(screenTimeApp, date);
        }

        private void AddAppInfoToMainScreen(ScreenTimeApp screenTimeApp, string date)
        {

            string appName = screenTimeApp.Name;

            // Truncate long app names
            if (appName.Length > 15) appName = appName[..15] + "...";

            string appPath = screenTimeApp.Path;
            // Truncate long path names
            if (appPath.Length > 30) appPath = appPath[..30] + "...";

            // Get and format time in background
            uint backgroundSeconds = screenTimeApp.SecondsInBackground.GetValueOrDefault(date);

            uint secondsInBackground = backgroundSeconds % 60;
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

            PathTextBlock.Text = appPath;
            AppInfWindow.Title = appName;
            TimesFocusedTextBlock.Text = timesFocused.ToString();
            TimesOpenedTextBlock.Text = timesOpened.ToString();
            AppNameTextBlock.Text = appName;
            SecondsInFocusTextBlock.Text = screenTimeAppTimeInFocus;
            SecondsInBackgroundTextBlock.Text = screenTimeAppTimeInBackground;

            ShowInFolderBtn.MouseDown += (s, e) =>
            {
                System.Diagnostics.Process.Start("explorer.exe", $"/select, \"{appPath}\"");
            };

            ShowInFolderBtn.MouseEnter += (s, e) =>
            {
                Cursor = Cursors.Hand;

                ShowInFolderBtn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8A8A8A"));
            };

            ShowInFolderBtn.MouseLeave += (s, e) =>
            {
                Cursor = Cursors.Arrow;

                ShowInFolderBtn.Foreground = new SolidColorBrush(Colors.White);
            };
        }
    }
}
