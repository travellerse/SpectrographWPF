using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrographWPF.FrameData
{
    public class PeakData
    {
        public double Frequency { get; set; }
        public double Amplitude { get; set; }
        public double Width { get; set; }
        public double Area { get; set; }
        public double SNR { get; set; }
        public double FWHM { get; set; }
    }
}
