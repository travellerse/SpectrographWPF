using System.IO.Ports;

namespace SpectrographWPF.SerialPortControl
{
    public class SerialPortManager
    {
        private SerialPort _serialPort = new();
        private string[]? _portList;
        public int OrderFramePerSecond { get; } = 4;

        public void OpenPort(string portName, int baudRate, int dataBits, int stopBits)
        {
            _serialPort.PortName = portName;
            _serialPort.BaudRate = baudRate;
            _serialPort.DataBits = dataBits;
            _serialPort.StopBits = (StopBits)stopBits;
            _serialPort.Open();
        }

        public void ClosePort() => _serialPort.Close();

        public void DiscardInBuffer() => _serialPort.DiscardInBuffer();

        public void DiscardOutBuffer() => _serialPort.DiscardOutBuffer();

        public void Write(string data) => _serialPort.Write(data);

        public int Read(byte[] buffer, int offset, int count) => _serialPort.Read(buffer, offset, count);

        public bool IsOpen() => _serialPort.IsOpen;

        public byte[] Update(bool isVirtual, string data = "@c0080#@")
        {
            DiscardInBuffer();
            DiscardOutBuffer();
            Write(data);

            var num = isVirtual ? 21262 : 21142;
            var buffer = new byte[num];
            var loopCount = 0;
            var alreadyReady = 0;
            int n;
            while (alreadyReady < num)
            {
                n = _serialPort.BytesToRead;
                if (n >= int.Min(4096, num - alreadyReady))
                {
                    alreadyReady += Read(buffer, alreadyReady, n);
                }
                else if (n == 0)
                {
                    if (++loopCount > 20)
                    {
                        throw new Exception("Read Error");
                    }
                }
                Thread.Sleep(1);
            }
            return buffer;
        }

        public string[] FindPort()
        {
            _portList = SerialPort.GetPortNames();
            return _portList;
        }
    }
}
