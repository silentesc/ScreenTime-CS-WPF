using ScreenTime.Listeners;
using System.Windows;

namespace ScreenTime
{
    public partial class App : Application
    {
        private readonly List<ProcessWatcherBase> processWatcherBases = [];

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Start process listeners

            ProcessStartListener startListener = new();
            startListener.Start();
            processWatcherBases.Add(startListener);

            ProcessExitListener exitListener = new();
            exitListener.Start();
            processWatcherBases.Add(exitListener);

            // Start periodically listeners

            FocusChangeListener focusChangeListener = new();
            focusChangeListener.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            foreach (var processWatcher in processWatcherBases)
            {
                processWatcher.Stop();
            }
        }
    }
}
