using ScreenTime.classes;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/*
 * TODO:
 * - Better UI (Time marker, not aligned to center so it doesn't jump)
 * - Ability to change app name so its not bound to process name
 * - Refactor code (clean code, date stuff not string)
 */

namespace ScreenTime
{
    public partial class MainWindow : Window
    {
        private readonly string dateFormat = "dd.MM.yyyy";
        private string dateString;
        public MainWindow()
        {
            InitializeComponent();

            dateString = DateTime.Now.ToString(dateFormat);
            SetScreenTimeAppsForMainScreen();
        }

        /*
         * Events
         */
        private void Image_MouseDown_Back(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DateTime date = DateTime.ParseExact(dateString, dateFormat, CultureInfo.InvariantCulture);
            date = date.AddDays(-1);
            dateString = date.ToString(dateFormat);

            SetScreenTimeAppsForMainScreen();
        }

        private void Image_MouseDown_Forward(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DateTime date = DateTime.ParseExact(dateString, dateFormat, CultureInfo.InvariantCulture);
            date = date.AddDays(1);
            dateString = date.ToString(dateFormat);

            SetScreenTimeAppsForMainScreen();
        }

        /*
         * Logic
         */
        private void SetScreenTimeAppsForMainScreen()
        {
            StackPanelDynamic.Children.Clear();

            List<ScreenTimeApp> focusedAppsToday = ScreenTimeApp.GetScreenTimeAppsByDateSorted(dateString, SortMode.SECONDS_IN_FOCUS, true);

            foreach (ScreenTimeApp screenTimeApp in focusedAppsToday)
            {
                AddScreenTimeAppToMainScreen(screenTimeApp, dateString);
            }

            TextBlockDate.Text = dateString;
        }

        private void AddScreenTimeAppToMainScreen(ScreenTimeApp screenTimeApp, string todayDate)
        {
            string screenTimeAppName = screenTimeApp.Name;
            if (screenTimeAppName.Length > 16) screenTimeAppName = screenTimeAppName[..16] + "...";

            uint screenTimeAppSecondsInFocus = screenTimeApp.SecondsInFocus.GetValueOrDefault(todayDate);
            uint secondsInFocus = screenTimeAppSecondsInFocus % 60;
            uint minutesInFocus = screenTimeAppSecondsInFocus / 60;
            uint hoursInFocus = minutesInFocus / 60;
            string screenTimeAppTimeInFocus;
            if (hoursInFocus > 0)
                screenTimeAppTimeInFocus = string.Format("{0}h {1}m", hoursInFocus, minutesInFocus);
            else if (minutesInFocus > 0)
                screenTimeAppTimeInFocus = string.Format("{0}m {1}s", minutesInFocus, secondsInFocus);
            else
                screenTimeAppTimeInFocus = string.Format("{0}s", screenTimeAppSecondsInFocus);

            Grid grid = new();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            // Create the new TextBlock
            TextBlock textBlockScreenTimeAppName = new()
            {
                Text = screenTimeAppName,
                Foreground = Brushes.White,
                FontSize = 30
            };
            TextBlock textBlockScreenTimeAppSecondsInFocus = new()
            {
                Text = screenTimeAppTimeInFocus,
                Foreground = Brushes.White,
                FontSize = 30
            };

            Grid.SetRow(textBlockScreenTimeAppName, 0);
            Grid.SetRow(textBlockScreenTimeAppSecondsInFocus, 1);
            grid.Children.Add(textBlockScreenTimeAppName);
            grid.Children.Add(textBlockScreenTimeAppSecondsInFocus);

            // Add to the dynamicStackPanel
            StackPanelDynamic.Children.Add(grid);
        }
    }
}
