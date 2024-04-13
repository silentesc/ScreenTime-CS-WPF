using ScreenTime.Classes;
using ScreenTime.Listeners;
using ScreenTime.utils;
using System.Diagnostics;
using System.Windows;

namespace ScreenTime
{
    public partial class App : Application
    {
        private readonly List<ProcessWatcherBase> processWatcherBases = [];

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load apps from file into apps
            StorageUtils.LoadAppsFromFile();

            // Load processes into apps
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    if (process.MainModule == null) continue;

                    string processName = process.ProcessName;
                    string path = process.MainModule.FileName;

                    ScreenTimeApp.Create(processName, path, 0, 0);
                }
                catch (Exception) { }
            }

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

            // Stop watchers
            foreach (var processWatcher in processWatcherBases)
            {
                processWatcher.Stop();
            }

            // Save apps to json file
            StorageUtils.SaveAppsToFile(ScreenTimeApp.apps);
        }
    }
}
