namespace SpectrographWPF.FrameData
{
    public class PeakData
    {
        public double Index { get; set; }

        public double Value { get; set; }

        public PeakData() { }

        public PeakData(double index, double value)
        {
            this.Index = index;
            this.Value = value;
        }
    }
}
