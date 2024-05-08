using SpectrographWPF.Manager;
using SpectrographWPF.Utils;
using System.Threading.Channels;

namespace SpectrographWPF.FrameData
{
    public class FrameDataServer
    {
        private static FrameDataServer? _instance = null;
        private static readonly object Padlock = new();
        private static readonly Channel<LightFrameData> _channel = Channel.CreateUnbounded<LightFrameData>();
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
        }

        public static void Stop()
        {
            _channel.Writer.Complete();
            IsRunning = false;
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

        public async Task Produce()
        {
            while (true)
            {
                var rawData = _portManager.Update();
                var frameData = new LightFrameData(new FrameData(Conversion.ToSpecifiedText(rawData, Conversion.ConversionType.Hex, System.Text.Encoding.UTF8), true));
                await _writer.WriteAsync(frameData);
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
            if (DPlotUpdate != null)
                DPlotUpdate(await _reader.ReadAsync());
            else
            {
                throw new Exception("delegate is null!");
            }
        }

        public delegate void PlotUpdateDelegate(LightFrameData data);

        public static PlotUpdateDelegate? DPlotUpdate { set; get; }

    }
}
