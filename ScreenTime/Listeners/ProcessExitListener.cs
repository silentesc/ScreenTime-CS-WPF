using System.Management;
using System.Runtime.InteropServices;

namespace ScreenTime.Listeners
{
    internal class ProcessExitListener : ProcessWatcherBase
    {
        protected override string QueryString => "SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'";

        protected override void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            int processId = Convert.ToInt32(((ManagementBaseObject)e.NewEvent["TargetInstance"])["ProcessId"]);
            string? processName = ((ManagementBaseObject)e.NewEvent["TargetInstance"])["Name"].ToString();
        }
    }
}
