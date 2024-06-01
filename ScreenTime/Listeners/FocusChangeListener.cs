using ScreenTime.classes;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ScreenTime.Listeners
{
    internal class FocusChangeListener
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint pdwProcessId);

        /*
         * Config constants
         */
        // The delay how long the app should sleep until it checks if a focus has changed and increase the focus seconds
        // Lower the value for more accurate results, but it will cost more performance
        private readonly int sleepDelaySeconds = 1;
        // When the background second should be increased
        // 1 means the background settings are increased every check, 2 means every second check etc.
        // Lower the value for more accurate results, but it will cost more performance
        private readonly int increaseBackgroundSecondsCounter = 2;

        public void Start()
        {
            Thread listener = new(() =>
            {
                int lastProcessId = -1;
                int counter = 1;

                while (true)
                {
                    Thread.Sleep(sleepDelaySeconds * 1000);

                    IntPtr foregroundWindowHandle = GetForegroundWindow();
                    _ = GetWindowThreadProcessId(foregroundWindowHandle, out uint processId);
                    Process foregroundProcess = Process.GetProcessById((int)processId);

                    if (lastProcessId == processId)
                    {
                        OnFocusKeep(lastProcessId, sleepDelaySeconds);
                    }
                    else
                    {
                        OnFocusChange(lastProcessId, (int)processId);
                        lastProcessId = (int)processId;
                    }

                    // Increase background seconds every x checks
                    if (counter >= increaseBackgroundSecondsCounter)
                    {
                        foreach (ScreenTimeApp screenTimeApp in ScreenTimeApp.screenTimeApps.Values)
                        {
                            if (!screenTimeApp.IsAppRunning()) continue;
                            screenTimeApp.IncreaseSecondsInBackground((uint)(sleepDelaySeconds * increaseBackgroundSecondsCounter));
                        }
                        counter = 1;
                    }
                    else
                    {
                        counter++;
                    }
                }
            })
            {
                IsBackground = true
            };

            listener.Start();
        }

        // Called when the periodical check occurs and the process is still the same
        private void OnFocusKeep(int processId, int keptFocusSeconds)
        {
            Process foregroundProcess = Process.GetProcessById(processId);
            ProcessModule? processModule;
            try
            {
                processModule = foregroundProcess.MainModule;
            }
            catch { return; }

            if (processModule == null) return;

            ScreenTimeApp screenTimeApp = ScreenTimeApp.CreateOrGetScreenTimeApp(foregroundProcess.ProcessName, processModule.FileName);
            screenTimeApp.IncreaseSecondsInFocus((uint)keptFocusSeconds);
        }

        // Called when the focus changes from one process to another
        private void OnFocusChange(int lastProcessId, int processId)
        {
            Process foregroundProcess = Process.GetProcessById(processId);
            ProcessModule? processModule;
            try
            {
                processModule = foregroundProcess.MainModule;
            }
            catch { return; }

            if (processModule == null) return;

            ScreenTimeApp screenTimeApp = ScreenTimeApp.CreateOrGetScreenTimeApp(foregroundProcess.ProcessName, processModule.FileName);
            screenTimeApp.IncreaseTimesFocused(1);
        }
    }
}
