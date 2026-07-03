using System;

namespace BarcodeScannerLibrary
{
    public interface IScannerReceiver
    {
        void ProcessScanData(string scanData);
    }
}
