namespace SpectrographWPF.FrameData
{

    public class FrameData
    {
        public int[] Amplitude { get; } = new int[10568 * 2];

        public byte[] FrameHead { get; } = [0x3c, 0xc3, 0x33, 0xcc];

        public byte[] FrameTail { get; } = [0x4f, 0x4b];

        public FrameData() { }

        //模块返回一帧数据（接收用HEX格式），包括4个帧头（0x3c,0xc3,0x33,0xcc），10568*2 个像素数据（输出10568*2个不带坐标16进制数, 10568个像素，每个像素2个字节，按顺序，高4位在前，低8位在后,如0x08 0xfc），两个帧尾（0x4f,0x4b）
        public FrameData(byte[] data)
        {
            if (data.Length != 10568 * 2 + 6)
            {
                throw new ArgumentException("Invalid data length");
            }

            for (int i = 0; i < 4; i++)
            {
                if (data[i] != FrameHead[i])
                {
                    throw new ArgumentException("Invalid frame head");
                }
            }

            for (int i = 0; i < 2; i++)
            {
                if (data[10568 * 2 + 4 + i] != FrameTail[i])
                {
                    throw new ArgumentException("Invalid frame tail");
                }
            }
            for (int i = 0; i < 10568 * 2; i += 2)
            {
                Amplitude[i] = (data[i + 4] << 4) + (data[i + 4 + 1] & 0x0f);
                Amplitude[i + 1] = (data[i + 4 + 1] >> 4) + (data[i + 4 + 2] << 4);
            }
        }

    }
}
