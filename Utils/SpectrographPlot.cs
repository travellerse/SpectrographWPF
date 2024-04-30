using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottPlot;
using ScottPlot.Plottables;

namespace SpectrographWPF.Utils
{
    public class SpectrographPlot : SignalXY
    {
        public SpectrographPlot(ISignalXYSource dataSource) : base(dataSource)
        {
        }

        private double MinLength = 0;
        private double MaxLength = 0;
    }
}
