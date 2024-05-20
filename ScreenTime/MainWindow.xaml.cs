using ScreenTime.classes;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScreenTime
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<ScreenTimeApp> apps = ScreenTimeApp.GetScreenTimeAppsByDateSorted("09.05.2024", SortMode.SECONDS_IN_FOCUS, true);

            foreach (ScreenTimeApp app in apps)
            {
                Debug.WriteLine(app.Name + " | " + app.SecondsInFocus.GetValueOrDefault("09.05.2024"));
            }

            foreach (ScreenTimeApp screenTimeApp in ScreenTimeApp.screenTimeApps.Values)
            {
                AddScreenTimeAppToMainScreen(screenTimeApp);
            }
        }

        private void AddScreenTimeAppToMainScreen(ScreenTimeApp screenTimeApp)
        {
            // Create the new TextBlock
            TextBlock newTextBlock = new()
            {
                Text = screenTimeApp.Name,
                Foreground = Brushes.White,
                FontSize = 30
            };

            // Add to the dynamicStackPanel
            StackPanelDynamic.Children.Add(newTextBlock);
        }

        private void Image_MouseDown_Back(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void Image_MouseDown_Forward(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
