using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrographWPF.Utils.Algorithm
{
    public class DataSmooth
    {
        public static double[] Apply(double[] data, int windowSize)
        {
            var n = data.Length;
            var result = new double[n];
            var m = windowSize / 2;
            for (int i = m; i < n - m; i++)
            {
                double sum = 0;
                for (int j = -m; j <= m; j++)
                {
                    sum += data[i + j];
                }
                result[i] = sum / windowSize;
            }
            return result;
        }
    }
}
