namespace SpectrographWPF.FrameData
{
    public class PeakData
    {
        public double index { get; set; }

        public double value { get; set; }

        public PeakData() { }

        public PeakData(double index, double value)
        {
            this.index = index;
            this.value = value;
        }
    }
}
