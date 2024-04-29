using SpectrographWPF.FrameData;

namespace SpectrographWPF.Utils.Algorithm
{
    public class SymmetricZeroAreaPeaking
    {


        public int W;//窗口宽度W=2m+1
        public int m;//窗口半宽度
        public double H = 10;

        public SymmetricZeroAreaPeaking(int W)
        {
            this.W = W;
            m = W / 2;
        }

        public double WindowFunction(double x)
        //高斯线型
        {
            double d = 0;
            for (int i = -m; i <= m; ++i)
            {
                d += f(i);
            }

            d /= W;
            return f(x) - d;
        }

        public double f(double x)
        {
            return Math.Exp(-4 * Math.Log(2) * (x * x / H / H));
        }

        public PeakData[] Apply(LightFrameData data)
        {
            var n = data.Value.Length;
            var temp = new double[n];

            for (int i = 0; i < n; i++)
            {
                if (i < m || i > n - m - 1)
                {
                    temp[i] = data.Value[i];
                }
                else
                {
                    double sum = 0;
                    for (int j = i - m; j <= i + m; j++)
                    {
                        sum += data.Value[j] * WindowFunction(j - i);
                    }
                    temp[i] = sum / W;
                }
            }

            for (int i = 0; i < n; ++i)
            {
                double Ssi = 0;

            }
        }



    }
}
