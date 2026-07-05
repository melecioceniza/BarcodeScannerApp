using System;
using System.Drawing;
using System.Windows.Forms;
using BarcodeScannerLibrary; // Import your custom library namespace

namespace BarcodeScannerDemo
{
    public partial class BarcodeDemo : Form
    {
        private System.Windows.Forms.Timer _statusResetTimer;

        //// Add this line at the top of your MainForm class fields
        //private string _detectedPortName = "None";

        private System.Windows.Forms.Timer _blinkTimer;
        private bool _isBlinkState = false; // Tracks which color to show next

        //Instantiate the single library manager instance
        private BarcodeScannerEngine _scannerEngine;

        public BarcodeDemo()
        {
            InitializeComponent();

            InitializeStatusTimers();

            // Initialize the library engine by passing your TabControl UI element
            _scannerEngine = new BarcodeScannerEngine(tabControl1);

            // Subscribe to the library pipeline notification triggers
            _scannerEngine.StatusChanged += Engine_StatusChanged;
            _scannerEngine.DeviceLost += Engine_DeviceLost;
            _scannerEngine.DeviceRecovered += Engine_DeviceRecovered;

            // Kickstart the automated scanner detection & connection loop
            _scannerEngine.Start();
        }
        private void InitializeStatusTimers()
        {
            _statusResetTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            _statusResetTimer.Tick += (s, e) => {
                _statusResetTimer.Stop();
                lblScanStatus.BackColor = SystemColors.Control;
                lblScanStatus.ForeColor = SystemColors.ControlText;
                lblScanStatus.Text = $"Scanner Ready ({_scannerEngine.DetectedPortName})";
            };

            _blinkTimer = new System.Windows.Forms.Timer { Interval = 500 };
            _blinkTimer.Tick += (s, e) => {
                lblScanStatus.BackColor = _isBlinkState ? Color.Red : Color.MistyRose;
                lblScanStatus.ForeColor = _isBlinkState ? Color.White : Color.DarkRed;
                if (_isBlinkState) System.Console.Beep(800, 150);
                _isBlinkState = !_isBlinkState;
            };
        }
        // Automatically formats status text and background colors dictated by the library
        private void Engine_StatusChanged(object sender, ScanStatusEventArgs e)
        {
            _statusResetTimer.Stop();
            lblScanStatus.Text = e.IsOverride ? e.Message : $" [{DateTime.Now:HH:mm:ss}] {e.Message}";

            if (e.IsOverride)
            {
                lblScanStatus.BackColor = (Color)e.BackColor;
                lblScanStatus.ForeColor = (Color)e.ForeColor;
            }
            else
            {
                lblScanStatus.BackColor = e.IsSuccess ? Color.LightGreen : Color.MistyRose;
                lblScanStatus.ForeColor = e.IsSuccess ? Color.DarkGreen : Color.DarkRed;
                _statusResetTimer.Start();


            }
        }

        // Turns on visual alarms if the library loses physical hardware connection
        private void Engine_DeviceLost(object sender, EventArgs e)
        {
            _statusResetTimer.Stop();
            lblScanStatus.Text = $"CRITICAL: Scanner Disconnected! ({_scannerEngine.DetectedPortName})";
            _blinkTimer.Start();

            System.Media.SystemSounds.Hand.Play();
            MessageBox.Show("The barcode scanner has been unplugged! Use manual entries.", "Hardware Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            FocusActiveTabManualInput();
        }
        private void Engine_DeviceRecovered(object sender, EventArgs e)
        {
            _blinkTimer.Stop(); // Clear the alarms, the engine auto-reconnected silently!
        }
        private void StatusResetTimer_Tick(object sender, EventArgs e)
        {
            _statusResetTimer.Stop();
            lblScanStatus.BackColor = Color.FromKnownColor(KnownColor.Control);
            lblScanStatus.ForeColor = Color.Black;
            lblScanStatus.Text = $"Scanner Ready ({_scannerEngine.DetectedPortName})";
        }
        // Inject the engine reference when generating tabs so manual entries function
        private void btnAddTab_Click(object sender, EventArgs e)
        {
            TabPage newTabPage = new TabPage();
            newTabPage.Text = $"Sheet {tabControl1.TabPages.Count + 1}";

            InventoryPageControl pageContent = new InventoryPageControl();
            pageContent.Dock = DockStyle.Fill;

            // Pass the MainForm's scanner engine down into the new page instance
            pageContent.ScannerEngine = this._scannerEngine;

            newTabPage.Controls.Add(pageContent);
            tabControl1.TabPages.Add(newTabPage);
            tabControl1.SelectedTab = newTabPage;
        }
        private void FocusActiveTabManualInput()
        {
            if (tabControl1.SelectedTab == null) return;
            foreach (Control control in tabControl1.SelectedTab.Controls)
            {
                if (control is InventoryPageControl inventoryPage)
                {
                    inventoryPage.FocusManualInputText();
                    break;
                }
            }
        }
        private void btnCloseCurrentTab_Click(object sender, EventArgs e)
        {
            // 1. Double check that there is an active tab currently selected
            if (tabControl1.SelectedTab != null)
            {
                // 2. Identify the active TabPage container
                TabPage currentTab = tabControl1.SelectedTab;

                // 3. Loop through and cleanly unload any UserControls inside this tab first
                foreach (Control control in currentTab.Controls)
                {
                    if (control is InventoryPageControl inventoryPage)
                    {
                        // Disconnect our global engine pointer to free up reference links
                        inventoryPage.ScannerEngine = null;
                    }
                }

                // 4. Remove the TabPage from your TabControl UI element
                tabControl1.TabPages.Remove(currentTab);

                // 5. CRITICAL FOR MEMORY: Explicitly dispose the tab page and its child textboxes
                currentTab.Dispose();

                // 6. Optional: Provide a minor update on the status strip bar
                lblScanStatus.Text = $" [{DateTime.Now:HH:mm:ss}] Sheet closed successfully.";
            }
        }
    }
}
