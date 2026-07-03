using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BarcodeScannerLibrary
{
    public class ScanStatusEventArgs : EventArgs
    {
        public string Message { get; }
        public bool IsSuccess { get; }
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public bool IsOverride { get; set; }
        public ScanStatusEventArgs(string message, bool isSuccess)
        {
            Message = message; ;
            IsSuccess = isSuccess;
        }
    }
}
