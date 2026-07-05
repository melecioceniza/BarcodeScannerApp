using System.Windows.Forms;
using BarcodeScannerLibrary; //Import your custom library namespace

namespace BarcodeScannerDemo
{
    //Add the IScannerReceiver inheritance here
    public partial class InventoryPageControl : UserControl, IScannerReceiver
    {
        // Property to store a reference to the global controller
        // Keep a reference to the global engine so your textbox can talk to it
        public BarcodeScannerEngine ScannerEngine { get; set; }
        public InventoryPageControl()
        {
            InitializeComponent();
        }
        //This satisfies the interface. Real scanner inputs drop straight here!
        public void ProcessScanData(string barcode)
        {
            // This handles both automatic serial scans and manual inputs
            // This is now guaranteed to run safely on the UI thread
            txtBarcodeDisplay.Text = barcode;

            // Optional: Trigger your search or database lookup immediately
            ExecuteProductSearch(barcode);
        }

        private void ExecuteProductSearch(string barcode)
        {
            // Your database logic here...
        }

        // Wire this up to the txtBarcode's KeyDown event INSIDE this UserControl
        private void txtManualBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Stop the Windows 'ding' sound

                string typedBarcode = txtBarcodeDisplay.Text;

                // If the scanner system is online, send it to the controller
                if (this.ScannerEngine != null)
                {
                    this.ScannerEngine.ProcessManualInput(typedBarcode);
                }
                else
                {
                    // Fallback: If scanner hardware failed on startup, just process it locally
                    ProcessScanData(typedBarcode);
                }

                txtBarcodeDisplay.Clear(); // Reset for next scan
            }
        }

        // ADD THIS METHOD: Forces the cursor to blink inside the manual textbox
        public void FocusManualInputText()
        {
            if (txtBarcodeDisplay != null && txtBarcodeDisplay.CanFocus)
            {
                txtBarcodeDisplay.Focus();
                txtBarcodeDisplay.SelectAll(); // Highlights text if they already typed something half-way
            }
        }

    }
}
