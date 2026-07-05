using System;

using System.Drawing;


namespace BarcodeScannerLibrary
{
    public class ScanStatusEventArgs : EventArgs
    {
        public string Message { get; }
        public bool IsSuccess { get; }

        public Color? BackColor { get; set; }
        public Color? ForeColor { get; set; }


        public bool IsOverride { get; set; }
        public ScanStatusEventArgs(string message, bool isSuccess)
        {
            Message = message; ;
            IsSuccess = isSuccess;
        }
    }
}
