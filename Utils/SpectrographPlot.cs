using ScottPlot;
using ScottPlot.Plottables;

namespace SpectrographWPF.Utils
{
    public class SpectrographPlot : SignalXY
    {
        public SpectrographPlot(ISignalXYSource dataSource) : base(dataSource)
        {
        }
        /*
        private double MinLength = 0;
        private double MaxLength = 0;


        public void Render(RenderPack rp)
        {
            base.Render(rp);

        }
        */
    }
}
