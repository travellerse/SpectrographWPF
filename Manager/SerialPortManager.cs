using System.IO.Ports;

namespace SpectrographWPF.Manager
{
    public class SerialPortManager
    {
        private static SerialPortManager? _instance = null;
        private static readonly object Padlock = new();
        private readonly SerialPort _serialPort = new();
        private bool _isVirtual;

        private SerialPortManager() { }

        public static SerialPortManager Instance
        {
            get
            {
                lock (Padlock)
                {
                    _instance ??= new SerialPortManager();
                    return _instance;
                }
            }
        }

        public void OpenPort(string portName, int baudRate, int dataBits, int stopBits, bool isVirtual)
        {
            _serialPort.PortName = portName;
            _serialPort.BaudRate = baudRate;
            _serialPort.DataBits = dataBits;
            _serialPort.StopBits = (StopBits)stopBits;
            _isVirtual = isVirtual;
            _serialPort.Open();
        }

        public void ClosePort() => _serialPort.Close();

        public void DiscardInBuffer() => _serialPort.DiscardInBuffer();

        public void DiscardOutBuffer() => _serialPort.DiscardOutBuffer();

        public void Write(string data) => _serialPort.Write(data);

        public int Read(byte[] buffer, int offset, int count) => _serialPort.Read(buffer, offset, count);

        public bool IsOpen() => _serialPort.IsOpen;

        public static string[] FindPort() => SerialPort.GetPortNames();

        public byte[]? Update(string data = "@c0080#@")
        {
            try
            {
                DiscardInBuffer();
                DiscardOutBuffer();
                Write(data);
            }
            catch (Exception)
            {
                throw new Exception("Write Error");
            }

            var num = _isVirtual ? 21262 : 21142;
            var buffer = new byte[num];
            var loopCount = 0;
            var alreadyReady = 0;
            int n, lastN = 0;
            while (alreadyReady < num)
            {
                n = _serialPort.BytesToRead;
                if (n >= int.Min(4096, num - alreadyReady))
                {
                    alreadyReady += Read(buffer, alreadyReady, n);
                }
                else if (n == 0 || n == lastN)
                {
                    if (++loopCount > 200)
                    {
                        return null;
                        //throw new Exception("Read Error");
                    }
                }
                lastN = n;
                Thread.Sleep(1);
            }
            return buffer;
        }
    }
}
