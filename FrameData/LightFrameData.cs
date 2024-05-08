namespace SpectrographWPF.FrameData
{
    public class LightFrameData
    {
        public double[] WaveLength { get; } = new double[10550];
        public double[] Value { get; } = new double[10550];

        public const double m = 1200;
        public const double F = 150;
        public double eta = 21;//Deg
        public static double x0 = F * Math.Tan(DegToRad(16));
        public static int num = 10550;

        public LightFrameData(FrameData frame)
        {
            for (int i = 0; i < 10550; i++)
            {
                WaveLength[i] = TransLam(GetX(i));
                Value[i] = frame.Amplitude[i];
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
    }
}
