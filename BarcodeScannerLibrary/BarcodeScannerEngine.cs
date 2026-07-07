using System;
using System.Drawing;
using System.Management;
using System.Windows.Forms;

namespace BarcodeScannerLibrary
{
    public class BarcodeScannerEngine : IDisposable
    {
        private SerialScanner _scanner;
        private HardwareWatcher _hardwareWatcher;
        private readonly TabControl _tabControl;
        private string _detectedPortName = "None";

        public event EventHandler<ScanStatusEventArgs> StatusChanged;
        public event EventHandler DeviceLost;
        public event EventHandler DeviceRecovered;

        public string DetectedPortName => _detectedPortName;
        public bool IsScannerOnline => _scanner != null;

        public BarcodeScannerEngine(TabControl tabControl)
        {
            _tabControl = tabControl ?? throw new ArgumentNullException(nameof(tabControl));

            _hardwareWatcher = new HardwareWatcher(_tabControl);
            _hardwareWatcher.ScannerDisconnected += HardwareWatcher_ScannerDisconnected;
            _hardwareWatcher.ScannerConnected += HardwareWatcher_ScannerConnected;
            _hardwareWatcher.StartWatching();
        }

        public bool Start()
        {
            try
            {
                RaiseStatusChanged("Detecting scanner hardware...", Color.LightYellow, Color.DarkGoldenrod);

                string scannerPort = AutoDetectScannerPort();
                if (string.IsNullOrEmpty(scannerPort))
                {
                    //RaiseStatusChanged("Scanner Offline: Device not found", Color.MistyRose, Color.DarkRed);
                    //return false;
                    // MODIFIED: Do not return false! Let the app stay green in Keyboard Wedge fallback mode
                    RaiseStatusChanged("Scanner Ready (USB Keyboard Mode)", Color.FromKnownColor(KnownColor.Control), Color.Black);
                    return true;
                }

                _detectedPortName = scannerPort;
                RaiseStatusChanged($"Connecting to scanner on {scannerPort}...", Color.LightYellow, Color.DarkGoldenrod);

                _scanner = new SerialScanner();
                _scanner.Initialize(scannerPort, 9600);
                _scanner.BarcodeScanned += HandleBarcodeScanned;

                RaiseStatusChanged($"Scanner Ready ({scannerPort})", Color.FromKnownColor(KnownColor.Control), Color.Black);
                return true;
            }
            catch (Exception)
            {
                RaiseStatusChanged($"Scanner Offline ({_detectedPortName})", Color.MistyRose, Color.DarkRed);
                return false;
            }
        }

        private void HandleBarcodeScanned(object sender, string barcode)
        {
            if (_tabControl.IsDisposed || !_tabControl.IsHandleCreated) return;
            if (string.IsNullOrWhiteSpace(barcode)) return;

            _tabControl.BeginInvoke(new Action(() =>
            {
                try
                {
                    RouteBarcodeToActiveTab(barcode.Trim());
                    StatusChanged?.Invoke(this, new ScanStatusEventArgs($"Successfully scanned: {barcode}", true));
                }
                catch (Exception uiEx)
                {
                    StatusChanged?.Invoke(this, new ScanStatusEventArgs($"Scan failed: {uiEx.Message}", false));
                }
            }));
        }

        public void ProcessManualInput(string barcode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(barcode)) throw new ArgumentException("Barcode cannot be empty.");
                RouteBarcodeToActiveTab(barcode.Trim());
                StatusChanged?.Invoke(this, new ScanStatusEventArgs($"Manual entry success: {barcode}", true));
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, new ScanStatusEventArgs($"Manual entry failed: {ex.Message}", false));
            }
        }

        private void RouteBarcodeToActiveTab(string barcode)
        {
            TabPage activeTab = _tabControl.SelectedTab;
            if (activeTab == null) throw new InvalidOperationException("No active tab selected.");

            bool receiverFound = false;
            foreach (Control control in activeTab.Controls)
            {
                if (control is IScannerReceiver receiver)
                {
                    receiverFound = true;
                    receiver.ProcessScanData(barcode);
                    break;
                }
            }

            if (!receiverFound) throw new InvalidOperationException($"Tab '{activeTab.Text}' cannot accept scans.");
        }

        private void HardwareWatcher_ScannerDisconnected(object sender, EventArgs e)
        {
            StopScannerStream();
            DeviceLost?.Invoke(this, EventArgs.Empty);
        }

        private void HardwareWatcher_ScannerConnected(object sender, EventArgs e)
        {
            if (_scanner == null)
            {
                Start();
                DeviceRecovered?.Invoke(this, EventArgs.Empty);
            }
        }

        private string AutoDetectScannerPort()
        {
            string genericFallbackPort = null;

            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM%'"))
                {
                    foreach (var device in searcher.Get())
                    {
                        string desc = device["Caption"]?.ToString();
                        if (string.IsNullOrWhiteSpace(desc)) continue;

                        // 1. Core Check: Look for premium hardware brands
                        if (desc.IndexOf("Honeywell", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            desc.IndexOf("Zebra", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            desc.IndexOf("Symbol", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            desc.IndexOf("Datalogic", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            desc.IndexOf("Scanner", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            int start = desc.IndexOf("(COM", StringComparison.OrdinalIgnoreCase) + 1;
                            int end = desc.IndexOf(")", start);
                            return desc.Substring(start, end - start).Trim(); // Premium device found! Exit immediately.
                        }

                        // 2. Fallback Check: Save generic serial ports if no brand is found
                        // ADD ANY WORD YOU FOUND IN DEVICE MANAGER TO THIS IF-STATEMENT
                        if (genericFallbackPort == null &&
                           (desc.IndexOf("USB Serial", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            desc.IndexOf("Prolific", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            desc.IndexOf("FTDI", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            desc.IndexOf("Silicon Labs", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            desc.IndexOf("CH34", StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            int start = desc.IndexOf("(COM", StringComparison.OrdinalIgnoreCase) + 1;
                            int end = desc.IndexOf(")", start);
                            genericFallbackPort = desc.Substring(start, end - start).Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WMI Search Failed: {ex.Message}");
            }

            // If no direct brand matched, drop back to the generic USB serial port string found
            return genericFallbackPort;
        }

        private void RaiseStatusChanged(string msg, Color bg, Color fg)
        {
            StatusChanged?.Invoke(this, new ScanStatusEventArgs(msg, true) { BackColor = bg, ForeColor = fg, IsOverride = true });
        }

        public void StopScannerStream()
        {
            if (_scanner != null)
            {
                _scanner.BarcodeScanned -= HandleBarcodeScanned;
                _scanner.Dispose();
                _scanner = null;
            }
        }

        public void Dispose()
        {
            StopScannerStream();
            _hardwareWatcher?.Dispose();
        }
    }
}
