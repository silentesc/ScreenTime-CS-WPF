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

        private readonly int sleepDelayMillis = 1000;

        public void Start()
        {
            Thread listener = new(() =>
            {
                int lastProcessId = -1;

                while (true)
                {
                    Thread.Sleep(sleepDelayMillis);

                    IntPtr foregroundWindowHandle = GetForegroundWindow();
                    _ = GetWindowThreadProcessId(foregroundWindowHandle, out uint processId);
                    Process foregroundProcess = Process.GetProcessById((int)processId);

                    if (lastProcessId == processId) continue;
                    OnFocusChange(lastProcessId, (int)processId);
                    lastProcessId = (int)processId;
                }
            })
            {
                IsBackground = true
            };

            listener.Start();
        }

        private void OnFocusChange(int lastProcessId, int processId)
        {
            Process foregroundProcess = Process.GetProcessById(processId);

            Debug.WriteLine($"Process changed to {foregroundProcess.ProcessName}");
        }
    }
}
