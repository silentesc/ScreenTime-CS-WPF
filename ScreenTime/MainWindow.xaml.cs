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

            for (int i = 0; i < 100; i++)
            {
                AddElementDynamically($"Hallo {i}");
            }
        }

        private void AddElementDynamically(string text)
        {
            // Create the new TextBlock
            TextBlock newTextBlock = new()
            {
                Text = text,
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
