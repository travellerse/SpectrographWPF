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

        private void WriteData(string data = "@c0080#@")
        {
            _serialPort.Write(data);
        }

        private string ReadData()
        {
            return _serialPort.ReadExisting();
        }

        public string Update(string data = "@c0080#@")
        {
            WriteData(data);
            //模块返回一帧数据（接收用HEX格式），包括4个帧头（0x3c,0x3f,0x33,0x3f），10568*2 个像素数据（输出10568*2个不带坐标16进制数, 10568个像素，每个像素2个字节，按顺序，高4位在前，低8位在后,如0x08 0xfc），两个帧尾（0x4f,0x4b）
            var buffer = ReadData();
            while (buffer.Length < 10568 * 2 + 6)
            {
                buffer += ReadData();
            }
            return buffer;
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
