using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrographWPF.Utils.Algorithm
{
    public class NonMaximumSuppression
    {
        public static List<int> Apply(double[] data)
        {
            var n = data.Length;
            var result = new List<int>();
            for (int i = 1; i < n - 1; i++)
            {
                if (data[i] > data[i - 1] && data[i] > data[i + 1] && data[i]>=75)
                {
                    result.Add(i);
                }
            }
            return result;
        }
    }
}
