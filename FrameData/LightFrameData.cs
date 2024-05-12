using ScottPlot;
using SpectrographWPF.Utils;

namespace SpectrographWPF.FrameData
{
    public class LightFrameData
    {
        public double[] WaveLength { get; private set; } = new double[10550];
        public double[] Value { get; private set; } = new double[10550];
        public Color[] Color { get; private set; } = new Color[10550];
        public long Timestamp;
        public long frame { get; private set; }

        public const double m = 1200;
        public const double F = 150;
        public double eta = 21;//Deg
        public static double x0 = F * Math.Tan(DegToRad(16));
        public static int num = 10550;

        public LightFrameData(FrameData frameData)
        {
            Timestamp = frameData.Timestamp;
            frame = 1;
            for (int i = 0; i < 10550; i++)
            {
                WaveLength[i] = TransLam(GetX(i));
                Value[i] = frameData.Amplitude[i];
                Color[i] = Conversion.RgbCalculator.Calc(WaveLength[i]);
            }
        }

        private double TransLam(double x)
        {
            return 1 / m * (Math.Cos(DegToRad(eta)) - Math.Sin(DegToRad(eta + 14)) * F / Math.Sqrt(x * x + F * F) + Math.Cos(DegToRad(eta + 14)) * x / Math.Sqrt(x * x + F * F)) * 1e6;
        }

        private static double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }

        public static double GetX(int index)
        {
            return x0 + (index - num / 2.0) * 4e-3;
        }

        public void Integral(LightFrameData frameData)
        {
            Timestamp = frameData.Timestamp;
            frame += frameData.frame;
            Color = frameData.Color;
            for (int i = 0; i < Value.Length; i++)
            {
                Value[i] += frameData.Value[i];
            }
        }

        public override string ToString()
        {
            var str = $"WaveLength,Value\n";
            for (int i = 0; i < Value.Length; i++)
            {
                str += $"{WaveLength[i]},{Value[i] / frame}\n";
            }
            return str;
        }

    }
}
