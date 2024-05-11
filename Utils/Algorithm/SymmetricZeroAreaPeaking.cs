using SpectrographWPF.FrameData;

namespace SpectrographWPF.Utils.Algorithm
{
    public class SymmetricZeroAreaPeaking
    {
        private int W;//窗口宽度W=2m+1
        private int m;//窗口半宽度
        private double HL;
        private double HG;

        private int threshold;
        private int percent;

        private double d;

        public SymmetricZeroAreaPeaking(int W = 300, int HL = 100, int HG = 200, int percent = 70, int threshold = 75)
        {
            this.W = W;
            this.HL = HL;
            this.HG = HG;
            m = W / 2;
            for (int i = -m; i <= m; ++i)
            {
                d += G(i);
            }
            d /= W;
            this.percent = percent;
            this.threshold = threshold;
        }

        public double WindowFunction(double j)
        {
            return G(j) - d;
        }

        public double G(double i)
        //Voigt
        {
            double k = 0.5;
            return 2 * k * HL / (4 * i * i + HL * HL) / Math.PI + (1 - k) * Math.Sqrt(4 * Math.Log(2)) / Math.Sqrt(Math.PI) / HG * Math.Exp(-4 * Math.Log(2) * i * i / HG / HG);
        }

        public PeakData[] Apply(LightFrameData data)
        {
            var n = data.Value.Length;
            var Y = new double[n];
            var deltaY = new double[n];
            var Ss = new double[n];
            var Score = new double[n];

            var temp = new int[1024];
            PeakData[] peaks;
            var num = 0;

            for (int i = m; i < n - m; i++)
            {
                double sumY = 0;
                double sumdeltaY = 0;
                for (int j = -m; j <= m; j++)
                {
                    double C = WindowFunction(j);
                    sumY += data.Value[i + j] * C;
                    sumdeltaY += data.Value[i + j] * Math.Pow(C, 2);
                }
                Y[i] = sumY;
                deltaY[i] = sumdeltaY;
                Ss[i] = Y[i] / deltaY[i];
            }

            var IMax = data.Value.Max();
            var SSMax = Ss.Max();

            for (int i = m; i < n - m; i++)
            {
                Score[i] = percent * Ss[i] / SSMax + (100 - percent) * data.Value[i] / IMax;
            }
            //求Score极值
            for (int i = m + 1; i < n - m - 1; i++)
            {
                if (Score[i] >= Score[i - 1] && Score[i] >= Score[i + 1] && Score[i] >= threshold)
                {
                    temp[num++] = i;
                }
            }

            peaks = new PeakData[num];
            for (int i = 0; i < num; ++i)
            {
                peaks[i] = new PeakData(data.WaveLength[temp[i]], data.Value[temp[i]]);
            }

            return peaks;
        }



    }
}
