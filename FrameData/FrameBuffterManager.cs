namespace SpectrographWPF.FrameData
{
    public class FrameBuffterManager
    {
        //FIFO
        public Queue<FrameData> Frames = new();

        public FrameBuffterManager() { }

        public void AddFrameData(FrameData frameData) { Frames.Enqueue(frameData); }

        public void OutPutBuffer()
        {

        }

        private void PlotUpdate(FrameData frameData)
        {
        }

    }
}
