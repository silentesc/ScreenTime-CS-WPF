using ScreenTime.classes;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace ScreenTime.Listeners
{
    internal class ProcessStartListener : ProcessWatcherBase
    {
        protected override string QueryString => "SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'";

        protected override void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            int processId = Convert.ToInt32(((ManagementBaseObject)e.NewEvent["TargetInstance"])["ProcessId"]);
            Process process = Process.GetProcessById(processId);
            ProcessModule? processModule = null;

            try
            {
                processModule = process.MainModule;
            }
            catch { }

            if (processModule == null) return;

            ScreenTimeApp screenTimeApp = ScreenTimeApp.CreateOrGetScreenTimeApp(process.ProcessName, processModule.FileName);
            screenTimeApp.IncreaseTimesOpened();
        }
    }
}
