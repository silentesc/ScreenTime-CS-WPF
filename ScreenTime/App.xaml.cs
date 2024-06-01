﻿using ScreenTime.classes;
using ScreenTime.Listeners;
using ScreenTime.utils;
using System.Windows;

namespace ScreenTime
{
    public partial class App : Application
    {
        private readonly List<ProcessWatcherBase> processWatcherBases = [];

        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            foreach (var processWatcher in processWatcherBases)
            {
                processWatcher.Stop();
            }

            StorageUtils.SaveAppsToFile(ScreenTimeApp.screenTimeApps.Values.ToList());

            e.Handled = true;
        }
    }
}
