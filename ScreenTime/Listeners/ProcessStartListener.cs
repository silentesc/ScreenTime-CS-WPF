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
            Process? process = null;
            ProcessModule? processModule = null;

            try
            {
                process = Process.GetProcessById(processId);
                processModule = process.MainModule;
            }
            catch { }

            if (process == null || processModule == null) return;

            if (ScreenTimeApp.screenTimeApps.TryGetValue(processModule.FileName, out ScreenTimeApp? screenTimeApp) && screenTimeApp != null)
            {
                screenTimeApp.IncreaseTimesOpened(1);
            }
        }
    }
}
