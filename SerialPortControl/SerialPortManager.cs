using System.IO.Ports;

namespace SpectrographWPF.SerialPortControl
{
    public class SerialPortManager
    {
        private SerialPort _serialPort;
        private string[]? _portList;
        public int OrderFramePerSecond { get; } = 4;

        public SerialPortManager()
        {
            _serialPort = new SerialPort();
        }

        public void OpenPort(string portName, int baudRate = 921600, int dataBits = 8, int stopBits = 1)
        {
            _serialPort.PortName = portName;
            _serialPort.BaudRate = baudRate;
            _serialPort.DataBits = dataBits;
            _serialPort.StopBits = (StopBits)stopBits;
            _serialPort.ReadBufferSize = 1048576;
            _serialPort.WriteBufferSize = 1048576;
            _serialPort.Open();
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
        }

        public void ClosePort()
        {
            _serialPort.Close();
        }

        public byte[] Update(bool isVirtual, string data = "@c0080#@")
        {
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
            _serialPort.Write(data);

            byte[] buffer;
            if (isVirtual)
            {
                buffer = new byte[21262];
                int loopCount = 0;
                while (_serialPort.BytesToRead < 12288)//16384
                {
                    Thread.Sleep(1);
                    loopCount++;
                    if (loopCount > 20)
                    {
                        throw new ArgumentException(_serialPort.BytesToRead.ToString());
                    }
                }
                var n = _serialPort.BytesToRead;
                var alreadyRead = _serialPort.Read(buffer, 0, n);
                //_serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
                loopCount = 0;
                while (alreadyRead + _serialPort.BytesToRead < 21262)//4878
                {
                    Thread.Sleep(1);
                    loopCount++;
                    if (loopCount > 20)
                    {
                        throw new ArgumentException((alreadyRead + _serialPort.BytesToRead).ToString());
                    }
                }
                n = _serialPort.BytesToRead;
                //21262
                _serialPort.Read(buffer, alreadyRead, n);
            }
            else
            {
                Thread.Sleep(200);
                while (_serialPort.BytesToRead < 21142)
                {
                    Thread.Sleep(1);
                }
                var n = _serialPort.BytesToRead;
                buffer = new byte[n];
                var alreadyRead = _serialPort.Read(buffer, 0, n);
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
