using SpectrographWPF.Manager;
using SpectrographWPF.Utils;
using System.Threading.Channels;
using System.Windows;

namespace SpectrographWPF.FrameData
{
    public class FrameDataServer
    {
        private static FrameDataServer? _instance = null;
        private static readonly object Padlock = new();
        private static readonly Channel<LightFrameData> _channel = Channel.CreateBounded<LightFrameData>(1);
        public static bool IsRunning { get; private set; }

        private static readonly FrameDataProducer Producer = new(_channel.Writer);
        private static readonly FrameDataConsumer Consumer = new(_channel.Reader);

        private FrameDataServer() { }

        public static FrameDataServer Instance
        {
            get
            {
                lock (Padlock)
                {
                    _instance ??= new FrameDataServer();
                    return _instance;
                }
            }
        }

        public static void Start()
        {
            var pTask = Task.Run(() => Producer.Produce());
            var cTask = Task.Run(() => Consumer.Consume());
            IsRunning = true;
            Producer.IsRunning = true;
            Consumer.IsRunning = true;
        }

        public static void Stop()
        {
            IsRunning = false;
            Producer.IsRunning = false;
            Consumer.IsRunning = false;
            _channel.Reader.ReadAllAsync();
        }
    }

    public class FrameDataProducer
    {
        private readonly ChannelWriter<LightFrameData> _writer;
        private readonly SerialPortManager _portManager;
        public FrameDataProducer(ChannelWriter<LightFrameData> writer)
        {
            _portManager = SerialPortManager.Instance;
            _writer = writer;
        }

        public bool IsRunning { get; set; }

        public async Task Produce()
        {
            while (IsRunning)
            {
                var rawData = _portManager.Update();
                var frameData = new LightFrameData(new FrameData(Conversion.ToSpecifiedText(rawData, Conversion.ConversionType.Hex, System.Text.Encoding.UTF8), true));
                await _writer.WriteAsync(frameData);
                Thread.Sleep(150);
            }
        }
    }

    public class FrameDataConsumer
    {
        private readonly ChannelReader<LightFrameData> _reader;

        public FrameDataConsumer(ChannelReader<LightFrameData> reader)
        {
            _reader = reader;
        }

        public async Task Consume()
        {
            if (DPlotUpdate == null) throw new Exception("delegate is null!");
            while (IsRunning)
            {
                await Application.Current.Dispatcher.BeginInvoke(async () => DPlotUpdate(await _reader.ReadAsync()));
                Thread.Sleep(10);
            }
        }

        public delegate void PlotUpdateDelegate(LightFrameData data);

        public static PlotUpdateDelegate? DPlotUpdate { set; get; }
        public bool IsRunning { get; set; }
    }
}
