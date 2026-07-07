using BarcodeScannerLibrary; // Import your custom library namespace
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BarcodeScannerDemo
{
    public partial class BarcodeDemo : Form
    {
        // ADD THIS PROPERTY: Allows the binder to view the tab collection safely
        public TabControl MainTabControl => this.tabControl1;

        private System.Windows.Forms.Timer _statusResetTimer;

        //// Add this line at the top of your MainForm class fields
        //private string _detectedPortName = "None";

        private System.Windows.Forms.Timer _blinkTimer;
        private bool _isBlinkState = false; // Tracks which color to show next

        //Instantiate the single library manager instance
        private BarcodeScannerEngine _scannerEngine;

        private MainFormScannerBinder _scannerBinder;

        public BarcodeDemo()
        {
            InitializeComponent();

            // Initialize the library engine by passing your TabControl UI element
            _scannerEngine = new BarcodeScannerEngine(tabControl1);


            // REFACTORED: We pass the status label element straight into the binder! - MainformScannerBinder.cs handles all the status updates and timers now
            _scannerBinder = new MainFormScannerBinder(this, _scannerEngine, lblScanStatus);


            // Kickstart the automated scanner detection & connection loop
            _scannerEngine.Start();

            // 1. ADD THIS LINE: Automatically trigger focus as soon as the app window opens on screen
            this.Shown += MainForm_Shown;
        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            // 2. Wrap the focus routine in a BeginInvoke block.
            // This forces Windows to wait until all startup components are drawn before placing the cursor.
            this.BeginInvoke(new Action(() =>
            {
                // If you don't have a default tab open on startup, create one first:
                if (tabControl1.TabPages.Count == 0)
                {
                    CreateDefaultStartupTab();
                }
                else
                {
                    // Otherwise, focus the existing active tab immediately
                    FocusFirstTabOnStartup();
                }
            }));
        }
        private void CreateDefaultStartupTab()
        {
            TabPage newTabPage = new TabPage();
            newTabPage.Text = "Inventory Sheet 1";

            // Create your default page form layout
            InventoryPageControl pageForm = new InventoryPageControl();
            //pageForm.TopLevel = false;
            //pageForm.FormBorderStyle = FormBorderStyle.None;
            pageForm.Dock = DockStyle.Fill;
            pageForm.ScannerEngine = this._scannerEngine;

            newTabPage.Controls.Add(pageForm);
            pageForm.Show();

            tabControl1.TabPages.Add(newTabPage);
            tabControl1.SelectedTab = newTabPage;

            // Immediately focus the newly opened text box
            pageForm.FocusManualInputText();
        }

        private void FocusFirstTabOnStartup()
        {
            if (tabControl1.SelectedTab == null) return;

            foreach (Control control in tabControl1.SelectedTab.Controls)
            {
                if (control is InventoryPageControl activePage)
                {
                    activePage.FocusManualInputText();
                    break;
                }
            }
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
