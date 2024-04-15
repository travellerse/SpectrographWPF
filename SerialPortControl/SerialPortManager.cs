using System.IO.Ports;

namespace SpectrographWPF.SerialPortControl
{
    public class SerialPortManager
    {
        private SerialPort _serialPort;
        private string[]? _portList;

        public SerialPortManager()
        {
            _serialPort = new SerialPort();
        }

        public void OpenPort(string portName, int baudRate = 921600)
        {
            _serialPort.PortName = portName;
            _serialPort.BaudRate = baudRate;
            _serialPort.Open();
        }

        public void ClosePort()
        {
            _serialPort.Close();
        }

        public void WriteData(string data = "@c0080#@")
        {
            _serialPort.Write(data);
        }

        public string ReadData()
        {
            return _serialPort.ReadExisting();
        }

        public string[] FindPort()
        {
            _portList = SerialPort.GetPortNames();
            return _portList;
        }

        public bool IsOpen()
        {
            return _serialPort.IsOpen;
        }
    }
}
