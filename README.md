# BarcodeScannerLibrary
USB Barcode Scanner  Library using com port

## 🚀 Key Features
- **Easy Integration**: Quickly integrate barcode scanning functionality into your applications.
- **Cross-Platform Support**: Compatible with Windows, macOS, and Linux.
- **Multiple Barcode Formats**: Supports various barcode formats including QR codes, Code 128,
- Code 39, EAN-13, and more.
- **Real-Time Scanning**: Provides real-time scanning capabilities for efficient data capture.
- **Customizable Settings**: Configure scanner settings such as scan mode, beep sound, and
- LED indicators.
- **Error Handling**: Robust error handling to manage scanner disconnections and read errors.
- **Open Source**: Free to use and modify under the MIT License.

## 📦 Technical Specifications
- **Programming Language**: C#
- **Framework**: .NET Framework 4.7.2 or later
- **Dependencies**: System.IO.Ports for serial communication
- **Supported Barcode Types**: QR Code, Code 128, Code 39, EAN-13, UPC-A, UPC-E, Data Matrix, PDF417
- **Supported Platforms**: Windows, macOS, Linux (via .NET Core)
- **License**: MIT License

## 🪟 User Interface
The library provides a simple and intuitive user interface for configuring and managing the barcode scanner. Users can
 easily set up the scanner, view scanned data, and customize settings through a graphical interface.

 ## Core Functionality
 The library offers core functionality for barcode scanning, including:
 - **Scanner Initialization**: Initialize the barcode scanner and establish a connection via the COM port.
	- Connect to the scanner and prepare it for scanning operations.
	- Inform the user of the scanner's status and readiness.
	- Handle any connection errors or issues during initialization.

## 🔧 Technical Details
- **Serial Communication**: Utilizes the System.IO.Ports namespace for serial communication with the
- barcode scanner.
- **Event Handling**: Implements event-driven programming to handle scanner events such as data received,
- connection status changes, and error notifications.
- **Threading**: Uses background threads to manage scanner operations without blocking the main application thread
- - **Data Processing**: Processes scanned barcode data and provides it to the application in a structured format.
	- Validate and parse the scanned data to ensure accuracy and consistency.
	

