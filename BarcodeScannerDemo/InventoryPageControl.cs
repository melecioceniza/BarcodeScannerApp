using System.Windows.Forms;
using BarcodeScannerLibrary; //Import your custom library namespace

namespace BarcodeScannerDemo
{
    //Add the IScannerReceiver inheritance here
    public partial class InventoryPageControl : UserControl, IScannerReceiver
    {
        // Property to store a reference to the global controller
        // Keep a reference to the global engine so your textbox can talk to it
        // Satisfies the engine tracking property requirement
        public BarcodeScannerEngine ScannerEngine { get; set; }

        // These fields point to the controls your pages will use
        protected TextBox ManualInputTextBox;
        protected TextBox BarcodeDisplayTextBox;


        public InventoryPageControl()
        {
            InitializeComponent();

            // Link your UI designer controls to the master UIPage properties
            this.ManualInputTextBox = this.txtBarcodeDisplay;
            this.BarcodeDisplayTextBox = this.txtBarcodeDisplay;

            // Wire up the shared keypress tracker
            this.txtBarcodeDisplay.KeyDown += SharedManualInput_KeyDown;

        }
        // This satisfies the interface. Real scanner inputs drop straight here!
        // Shared method to handle data processing (Overridden by specific pages)
        public void ProcessScanData(string barcode)
        {
            // This handles both automatic serial scans and manual inputs
            // This is now guaranteed to run safely on the UI thread
            //txtBarcodeDisplay.Text = barcode;
            if (BarcodeDisplayTextBox != null)
            {
                BarcodeDisplayTextBox.Text = barcode;
            }

            // Optional: Trigger your search or database lookup immediately
            ExecuteProductSearch(barcode);
        }

        private void ExecuteProductSearch(string barcode)
        {
            // Your database logic here...
        }

        // ADD THIS METHOD: Forces the cursor to blink inside the manual textbox
        public void FocusManualInputText()
        {
            if (ManualInputTextBox != null && ManualInputTextBox.CanFocus)
            {
                ManualInputTextBox.Focus();

                // Safely handle textbox text highlighting using a pattern match
                if (ManualInputTextBox is TextBox standardBox)
                {
                    standardBox.SelectAll();
                }
                else
                {
                    // Fallback to trigger 'SelectAll' on custom controls via reflection
                    ManualInputTextBox.GetType().GetMethod("SelectAll")?.Invoke(ManualInputTextBox, null);
                }
            }
        }

        // Wire this up to the txtBarcode's KeyDown event INSIDE this UserControl
        // Refactored KeyDown Logic: Centrally handles manual Enter keypress routing
        protected void SharedManualInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Stop the Windows 'ding' sound

                string typedBarcode = ManualInputTextBox.Text;

                // If the scanner system is online, send it to the controller
                if (this.ScannerEngine != null)
                {
                    this.ScannerEngine.ProcessManualInput(typedBarcode);
                }
                else
                {
                    // Fallback: If scanner hardware failed on startup, just process it locally
                    ProcessScanData(typedBarcode); // Local fallback if offline
                }

                ManualInputTextBox.Clear(); // Reset for next scan
            }
        }



    }
}
