using System;

namespace BarcodeScannerLibrary
{
    public interface IScannerReceiver
    {
        // Handles both automatic hardware scans and manual entry routing
        void ProcessScanData(string barcode);

        // Forces the cursor to blink inside the manual textbox layout
        void FocusManualInputText();

        // Connects the page instance back to your central library engine
        BarcodeScannerEngine ScannerEngine { get; set; }
    }
}
