using System;
using System.Management;
using System.Windows.Forms;

namespace BarcodeScannerLibrary
{
    public class HardwareWatcher : IDisposable
    {
        private ManagementEventWatcher _insertWatcher;
        private ManagementEventWatcher _removeWatcher;
        private readonly Control _syncControl;

        public event EventHandler ScannerDisconnected;
        public event EventHandler ScannerConnected;

        public HardwareWatcher(Control syncControl)
        {
            _syncControl = syncControl ?? throw new ArgumentNullException(nameof(syncControl));
        }

        public void StartWatching()
        {
            try
            {
                var removeQuery = new WqlEventQuery("__InstanceDeletionEvent",
                    new TimeSpan(0, 0, 1),
                    "TargetInstance ISA 'Win32_PnPEntity'");

                _removeWatcher = new ManagementEventWatcher(removeQuery);
                _removeWatcher.EventArrived += DeviceRemoved_EventArrived;
                _removeWatcher.Start();

                var insertQuery = new WqlEventQuery("__InstanceCreationEvent",
                    new TimeSpan(0, 0, 1),
                    "TargetInstance ISA 'Win32_PnPEntity'");

                _insertWatcher = new ManagementEventWatcher(insertQuery);
                _insertWatcher.EventArrived += DeviceInserted_EventArrived;
                _insertWatcher.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HardwareWatcher query setup error: {ex.Message}");
            }
        }

        private void DeviceRemoved_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (_syncControl.InvokeRequired)
            {
                _syncControl.BeginInvoke(new Action(() => DeviceRemoved_EventArrived(sender, e)));
                return;
            }

            var targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            string caption = targetInstance["Caption"]?.ToString() ?? "";

            if (caption.Contains("(COM") && IsTargetHardwareBrand(caption))
            {
                ScannerDisconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        private void DeviceInserted_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (_syncControl.InvokeRequired)
            {
                _syncControl.BeginInvoke(new Action(() => DeviceInserted_EventArrived(sender, e)));
                return;
            }

            var targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            string caption = targetInstance["Caption"]?.ToString() ?? "";

            if (caption.Contains("(COM") && IsTargetHardwareBrand(caption))
            {
                ScannerConnected?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool IsTargetHardwareBrand(string caption)
        {
            return caption.IndexOf("Scanner", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   caption.IndexOf("Honeywell", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   caption.IndexOf("Zebra", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   caption.IndexOf("Symbol", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   caption.IndexOf("Datalogic", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   caption.IndexOf("USB Serial", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public void Dispose()
        {
            _removeWatcher?.Stop();
            _removeWatcher?.Dispose();
            _insertWatcher?.Stop();
            _insertWatcher?.Dispose();
        }
    }
}
