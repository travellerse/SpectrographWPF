namespace SpectrographWPF.FrameData
{
    public class PeakData
    {
        public double Index { get; private set; }

        public double Value { get; private set; }

        public double ss { get; private set; }

        public PeakData() { }

        public PeakData(double index, double value, double ss)
        {
            this.Index = index;
            this.Value = value;
            this.ss = ss;
        }
    }
}
