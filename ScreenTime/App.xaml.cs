using ScreenTime.classes;
using ScreenTime.Listeners;
using ScreenTime.utils;
using System.Windows;

namespace ScreenTime
{
    public partial class App : Application
    {
        private readonly List<ProcessWatcherBase> processWatcherBases = [];
        private Mutex? mutex;

        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Prevent app from being opened twice
            const string appMutedName = "ScreenTime-1718450821";
            mutex = new Mutex(true, appMutedName, out bool createdNewAppMutex);

            if (!createdNewAppMutex)
            {
                MessageBox.Show("App already running");
                Current.Shutdown();
                return;
            }

            StorageUtils.LoadAppsFromFile();
            ScreenTimeApp.MergePossibleNameConflicts();

            // Start process listeners
            ProcessStartListener startListener = new();
            startListener.Start();
            processWatcherBases.Add(startListener);

            //ProcessExitListener exitListener = new();
            //exitListener.Start();
            //processWatcherBases.Add(exitListener);

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

            StorageUtils.SaveAppsToFile(ScreenTimeApp.screenTimeApps.Values.ToList());

            mutex?.ReleaseMutex();
            mutex?.Dispose();
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            foreach (var processWatcher in processWatcherBases)
            {
                processWatcher.Stop();
            }

            StorageUtils.SaveAppsToFile(ScreenTimeApp.screenTimeApps.Values.ToList());

            mutex?.ReleaseMutex();
            mutex?.Dispose();

            e.Handled = true;
        }
    }
}
