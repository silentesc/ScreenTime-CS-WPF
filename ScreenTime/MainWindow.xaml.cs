using ScreenTime.classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace ScreenTime
{
    public partial class MainWindow : Window
    {
        // Default date format
        private readonly string dateFormat = "dd.MM.yyyy";
        private string dateString;
        // Default sort mode
        private SortMode currentSortMode = SortMode.SECONDS_IN_FOCUS;

        public MainWindow()
        {
            InitializeComponent();
            // Initialize dateString with current date
            dateString = DateTime.Now.ToString(dateFormat);
            // Set screen time apps for main screen
            SetScreenTimeAppsForMainScreen();
            // Set default item in filter combo box
            FilterComboBox.SelectedItem = FilterComboBox.Items[0];

            BlockFutureDates();
        }

        private void BlockFutureDates()
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            DateSelector.BlackoutDates.Add(new CalendarDateRange(tomorrow, DateTime.MaxValue ));

        }

        /*
         * Events
         */

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update sort mode when filter combo box selection changes
            if (FilterComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                Enum.TryParse(selectedItem.Tag.ToString(), out SortMode selectedSortMode);
                currentSortMode = selectedSortMode;
                SetScreenTimeAppsForMainScreen();
            }
        }

        /*
         * Logic
         */
        private void SetScreenTimeAppsForMainScreen()
        {
            // Clear existing screen time apps
            StackPanelDynamic.Children.Clear();
            // Get screen time apps for the selected date and sort mode
            List<ScreenTimeApp> focusedAppsToday = ScreenTimeApp.GetScreenTimeAppsByDateSorted(dateString, currentSortMode, true);

            if (focusedAppsToday.Count > 0)
            {
                uint maxProgressbarValue;
                // Get the maximum screen time in seconds for scaling progress bars
                switch (currentSortMode)
                {
                    case SortMode.SECONDS_IN_FOCUS:
                        maxProgressbarValue = focusedAppsToday.Max(app => app.SecondsInFocus.GetValueOrDefault(dateString));
                        break;
                    case SortMode.SECONDS_IN_BACKGROUND:
                        maxProgressbarValue = focusedAppsToday.Max(app => app.SecondsInBackground.GetValueOrDefault(dateString));
                        break;
                    case SortMode.TIMES_FOCUSED:
                        maxProgressbarValue = focusedAppsToday.Max(app => app.TimesFocused.GetValueOrDefault(dateString));
                        break;
                    case SortMode.TIMES_OPENED:
                        maxProgressbarValue = focusedAppsToday.Max(app => app.TimesOpened.GetValueOrDefault(dateString));
                        break;
                    default:
                        maxProgressbarValue = focusedAppsToday.Max(app => app.SecondsInFocus.GetValueOrDefault(dateString));
                        break;
                }

                foreach (ScreenTimeApp screenTimeApp in focusedAppsToday)
                {
                    // Add each screen time app to the main screen
                    AddScreenTimeAppToMainScreen(screenTimeApp, dateString, maxProgressbarValue);
                }
            }

            // Update the displayed date
            TextBlockDate.Text = dateString;
        }

        private void AddScreenTimeAppToMainScreen(ScreenTimeApp screenTimeApp, string todayDate, uint maxProgressbarValue)
        {
            uint progressValue = 0;
            string screenTimeAppName = screenTimeApp.Name;
            // Truncate long app names
            if (screenTimeAppName.Length > 50) screenTimeAppName = screenTimeAppName[..50] + "...";
            uint screenTimeAppSeconds = 0;
            uint screenTimeAppOpenings = 0;
            uint indicator;

            switch (currentSortMode)
            {
                case SortMode.SECONDS_IN_FOCUS:
                    progressValue = screenTimeApp.SecondsInFocus.GetValueOrDefault(todayDate);
                    screenTimeAppSeconds = screenTimeApp.SecondsInFocus.GetValueOrDefault(todayDate);
                    indicator = 1;
                    break;
                case SortMode.SECONDS_IN_BACKGROUND:
                    progressValue = screenTimeApp.SecondsInBackground.GetValueOrDefault(todayDate);
                    screenTimeAppSeconds = screenTimeApp.SecondsInBackground.GetValueOrDefault(todayDate);
                    indicator = 1;
                    break;
                case SortMode.TIMES_FOCUSED:
                    progressValue = screenTimeApp.TimesFocused.GetValueOrDefault(todayDate);
                    screenTimeAppOpenings = screenTimeApp.TimesFocused.GetValueOrDefault(todayDate);
                    indicator = 0;
                    break;
                case SortMode.TIMES_OPENED:
                    progressValue = screenTimeApp.TimesOpened.GetValueOrDefault(todayDate);
                    screenTimeAppOpenings = screenTimeApp.TimesOpened.GetValueOrDefault(todayDate);
                    indicator = 0;
                    break;
                default:
                    return;
            }

            string appTime;
            string appOpenings;
            System.Windows.Controls.TextBlock textBlockScreenTimeAppSeconds;

            if (indicator == 1)
            {
                uint seconds = screenTimeAppSeconds % 60;
                uint minutes = screenTimeAppSeconds / 60;
                uint hours = minutes / 60;

                if (hours > 0)
                    appTime = string.Format("{0}h {1}m", hours, minutes % 60);
                else if (minutes > 0)
                    appTime = string.Format("{0}m {1}s", minutes, seconds);
                else
                    appTime = string.Format("{0}s", screenTimeAppSeconds);

                textBlockScreenTimeAppSeconds = new System.Windows.Controls.TextBlock()
                {
                    Text = appTime,
                    Foreground = Brushes.White,
                    FontSize = 26
                };
            }
            else
            {
                appOpenings = string.Format("{0}", screenTimeAppOpenings);

                textBlockScreenTimeAppSeconds = new System.Windows.Controls.TextBlock()
                {
                    Text = appOpenings,
                    Foreground = Brushes.White,
                    FontSize = 26
                };
            }

            Grid grid = new();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            // Create the new TextBlocks for app name and time
            System.Windows.Controls.TextBlock textBlockScreenTimeAppName = new()
            {
                Text = screenTimeAppName,
                Foreground = Brushes.White,
                FontSize = 30
            };

            double maxWidth = 750;

            double progressPercentage = (double)progressValue / maxProgressbarValue;
            // Calculate progress width based on the maximum width
            double progressWidth = progressPercentage * maxWidth;

            // Scale width
            if (progressWidth > maxWidth)
            {
                progressWidth = maxWidth;
            }

            // Create the progress rectangle
            Rectangle progressRect = new()
            {
                Style = (Style)FindResource("CustomProgressBarStyle"),
                Width = progressWidth
            };

            // Create the arrow icon
            var arrowIcon = new SymbolIcon
            {
                Symbol = SymbolRegular.TriangleRight12,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Create border for the entire item
            Border border = new()
            {
                Width = 900,
                Style = (Style)FindResource("CustomBorderStyle"),
                Child = grid
            };

            // Define the hover colors
            Brush originalColor = Brushes.White;
            Brush hoverColor = Brushes.DarkGray;

            // Set the original color
            arrowIcon.Foreground = originalColor;

            // MouseEnter event handler for hover effect
            border.MouseEnter += (sender, e) =>
            {
                Cursor = Cursors.Hand;
                arrowIcon.Foreground = hoverColor;
            };

            // Assign a click event handler
            border.MouseDown += (sender, e) =>
            {
                AppInfoWindow appInfoWindow = new(screenTimeApp, todayDate);
                appInfoWindow.ShowDialog();
            };

            // MouseLeave event handler to revert to original color
            border.MouseLeave += (sender, e) =>
            {
                Cursor = Cursors.Arrow;
                arrowIcon.Foreground = originalColor;
            };

            // Add elements to the grid
            Grid.SetRow(textBlockScreenTimeAppName, 0);
            Grid.SetColumn(textBlockScreenTimeAppName, 0);
            Grid.SetColumnSpan(textBlockScreenTimeAppName, 3);

            Grid.SetColumn(textBlockScreenTimeAppSeconds, 1);
            Grid.SetRow(textBlockScreenTimeAppSeconds, 1);

            Grid.SetRow(progressRect, 1);
            Grid.SetColumnSpan(progressRect, 1);
            Grid.SetColumn(progressRect, 0);

            Grid.SetRow(arrowIcon, 0);
            Grid.SetRowSpan(arrowIcon, 2);
            Grid.SetColumn(arrowIcon, 3);

            grid.Children.Add(progressRect);
            grid.Children.Add(textBlockScreenTimeAppName);
            grid.Children.Add(textBlockScreenTimeAppSeconds);
            grid.Children.Add(arrowIcon);

            // Add border to the dynamicStackPanel
            StackPanelDynamic.Children.Add(border);
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is Calendar calendar && calendar.SelectedDate.HasValue)
            {
                dateString = calendar.SelectedDate.Value.ToString(dateFormat);
                SetScreenTimeAppsForMainScreen();
            }
        }

        private void SettingsBtn_Click(object sender, MouseButtonEventArgs e)
        {
            SettingsWindow settingsWindow = new();
            settingsWindow.ShowDialog();
        }

        private void SettingsBtn_Hover(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            SettingsBtn.Foreground = Brushes.DarkGray;
        }

        private void SettingsBtn_Leave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
            SettingsBtn.Foreground = Brushes.White;
        }

        private void TodayBtn_Click(object sender, RoutedEventArgs e)
        {
            // Set dateString to today's date
            dateString = DateTime.Now.ToString(dateFormat);
            // Refresh the screen time apps for the main screen
            SetScreenTimeAppsForMainScreen();

        }

        private void TodayBtn_Hover(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void TodayBtn_Leave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }
    }
}