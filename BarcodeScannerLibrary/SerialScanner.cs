using System;
using System.IO.Ports;

namespace BarcodeScannerLibrary
{
    public class SerialScanner : IDisposable
    {
        private SerialPort _scannerPort;
        public event EventHandler<string> BarcodeScanned;
        public string ActivePortName { get; private set; }

        public void Initialize(string portName, int baudRate = 9600)
        {
            this.ActivePortName = portName;

           _scannerPort = new SerialPort
           {
               PortName = portName,
                BaudRate = baudRate,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                NewLine = "\r\n", // Standard carriage return and line feed for scanners
           };
            _scannerPort.DataReceived += ScannerPort_DataReceived;
            try
            {
                _scannerPort.Open();
            }
            catch (Exception ex)
            {

                throw new Exception($"Failed to open scanner serial port {portName}. Error: {ex.Message}", ex);
            }
           
        }

        private void ScannerPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string scanData = _scannerPort.ReadLine().Trim();
                BarcodeScanned?.Invoke(this, scanData);
            }
            catch (Exception ex)
            {
                // Handle exceptions related to reading from the serial port
                System.Diagnostics.Debug.WriteLine($"Scanner read failure: {ex.Message}");
            }
        }
        public void Dispose()
        {
            // Dispose of unmanaged resources here
            if (_scannerPort != null)
            {
                _scannerPort.DataReceived -= ScannerPort_DataReceived;
                if (_scannerPort.IsOpen)
                {
                    _scannerPort.Close();
                }
                _scannerPort.Dispose();
            }
        }
    }
}
