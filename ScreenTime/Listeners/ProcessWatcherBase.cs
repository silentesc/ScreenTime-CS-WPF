using System.Management;
using System.Runtime.InteropServices;

namespace Testing
{
    internal abstract class ProcessWatcherBase
    {
        protected ManagementEventWatcher? watcher;
        protected abstract string QueryString { get; }

        protected abstract void Watcher_EventArrived(object sender, EventArrivedEventArgs e);

        public void Start()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            watcher = new ManagementEventWatcher(new WqlEventQuery(QueryString));
            watcher.EventArrived += Watcher_EventArrived;
            watcher.Start();
        }

        public void Stop()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            if (watcher != null)
            {
                watcher.Stop();
                watcher.Dispose();
            }
        }
    }
}
