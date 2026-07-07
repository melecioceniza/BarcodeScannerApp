using System;
using System.Drawing;
using System.Windows.Forms;
using BarcodeScannerLibrary;

namespace BarcodeScannerDemo
{
    public class MainFormScannerBinder : IDisposable
    {
        private readonly BarcodeDemo _form;
        private readonly BarcodeScannerEngine _engine;
        private readonly ToolStripStatusLabel _statusLabel;

        // Timers and state have been pulled entirely out of the MainForm
        private Timer _statusResetTimer;
        private Timer _blinkTimer;
        private bool _isBlinkState = false;

        public MainFormScannerBinder(BarcodeDemo form, BarcodeScannerEngine engine, ToolStripStatusLabel statusLabel)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _statusLabel = statusLabel ?? throw new ArgumentNullException(nameof(statusLabel));

            // Wire up the hardware tracking pipeline events
            _engine.StatusChanged += HandleStatusChanged;
            _engine.DeviceLost += HandleDeviceLost;
            _engine.DeviceRecovered += HandleDeviceRecovered;

            // ADD THIS LINE: Listen for whenever the user changes sheets
            _form.MainTabControl.SelectedIndexChanged += (s, e) => FocusActiveTabManualInput();

            // Automatically run your timer setup routines internally
            InitializeStatusTimers();
        }

        private void InitializeStatusTimers()
        {
            _statusResetTimer = new Timer { Interval = 3000 };
            _statusResetTimer.Tick += (s, e) =>
            {
                _statusResetTimer.Stop();
                _statusLabel.BackColor = SystemColors.Control;
                _statusLabel.ForeColor = SystemColors.ControlText;


                //_statusLabel.Text = $"Scanner Ready ({_engine.DetectedPortName})";

                // Dynamic text fallback based on connection type
                if (_engine.IsScannerOnline)
                {
                    _statusLabel.Text = $"Scanner Ready (COM Port Mode: {_engine.DetectedPortName})";
                }
                else
                {
                    _statusLabel.Text = "Scanner Ready (USB Keyboard Mode)";
                }

            };

            _blinkTimer = new Timer { Interval = 500 };
            _blinkTimer.Tick += (s, e) =>
            {
                _statusLabel.BackColor = _isBlinkState ? Color.Red : Color.MistyRose;
                _statusLabel.ForeColor = _isBlinkState ? Color.White : Color.DarkRed;

                // Play warning tone asynchronously so it doesn't freeze layout drawing
                if (_isBlinkState) System.Threading.Tasks.Task.Run(() => System.Console.Beep(800, 150));

                _isBlinkState = !_isBlinkState;
            };
        }

        private void HandleStatusChanged(object sender, ScanStatusEventArgs e)
        {
            _statusResetTimer.Stop();
            _statusLabel.Text = e.IsOverride ? e.Message : $" [{DateTime.Now:HH:mm:ss}] {e.Message}";

            if (e.IsOverride)
            {
                _statusLabel.BackColor = (Color)e.BackColor;
                _statusLabel.ForeColor = (Color)e.ForeColor;
            }
            else
            {
                _statusLabel.BackColor = e.IsSuccess ? Color.LightGreen : Color.MistyRose;
                _statusLabel.ForeColor = e.IsSuccess ? Color.DarkGreen : Color.DarkRed;
                _statusResetTimer.Start();
            }
        }

        private void HandleDeviceLost(object sender, EventArgs e)
        {
            _statusResetTimer.Stop();
            _statusLabel.Text = $"CRITICAL: Scanner Disconnected! ({_engine.DetectedPortName})";
            _blinkTimer.Start();

            System.Media.SystemSounds.Hand.Play();

            MessageBox.Show("The barcode scanner has been unplugged or lost power! Please reconnect the cable.",
                            "Hardware Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Error);

            FocusActiveTabManualInput();
        }

        //private void FocusActiveTabManualInput()
        //{
        //    if (tabControl1.SelectedTab == null) return;
        //    foreach (Control control in tabControl1.SelectedTab.Controls)
        //    {
        //        //Option 1: If you know the control type, you can cast and call the method directly
        //        //if (control is InventoryPageControl inventoryPage)
        //        //{
        //        //    inventoryPage.FocusManualInputText();
        //        //    break;
        //        //}
        //        //Option 2: If you want to be more generic, you can use reflection to find the method
        //        // The Switch Statement: Automatically matches ANY class implementing our interface
        //        switch (control)
        //        {
        //            case IScannerReceiver activePage:
        //                activePage.FocusManualInputText();
        //                return; // Match found and cursor set. Exit the entire method instantly!
        //        }
        //    }
        //}


        // NEW LOCATION: This logic now runs entirely inside the binder class
        private void FocusActiveTabManualInput()
        {
            // Access the TabControl through the public property we created in Step 1
            TabControl tabControl = _form.MainTabControl;

            if (tabControl == null || tabControl.SelectedTab == null) return;

            // Loop through controls on the active tab layout
            foreach (Control control in tabControl.SelectedTab.Controls)
            {
                // Dynamically match against your universal UIPage class from MelUI
                if (control is InventoryPageControl activePage)
                {
                    activePage.FocusManualInputText();
                    return; // Cursor set, exit instantly
                }
            }
        }
        private void HandleDeviceRecovered(object sender, EventArgs e)
        {
            _blinkTimer.Stop();
        }

        public void Dispose()
        {
            // Clean up background threading resources safely
            _statusResetTimer?.Stop();
            _statusResetTimer?.Dispose();

            _blinkTimer?.Stop();
            _blinkTimer?.Dispose();

            _engine.StatusChanged -= HandleStatusChanged;
            _engine.DeviceLost -= HandleDeviceLost;
            _engine.DeviceRecovered -= HandleDeviceRecovered;
        }
    }
}
