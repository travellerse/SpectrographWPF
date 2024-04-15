namespace SpectrographWPF.FrameData
{

    public class FrameData
    {
        public int[] Amplitude { get; } = new int[10550];

        public bool IsVirtual;

        private int PreDummy = 64;

        private int AfterDummy = 14;

        public FrameData() { }

        //模块返回一帧数据（接收用HEX格式），包括4个帧头（0x3c,0xc3,0x33,0xcc），10568*2 个像素数据（输出10568*2个不带坐标16进制数, 10568个像素，每个像素2个字节，按顺序，高4位在前，低8位在后,如0x08 0xfc），两个帧尾（0x4f,0x4b）
        //包含空格总长63425
        public FrameData(string data, bool isVirtual)
        {
            IsVirtual = isVirtual;

            if (!IsVirtual)
            {
                if (data.Length != 63425)
                {
                    throw new ArgumentException("Invalid data length");
                }

                var bytes = data.Split(' ');

                var n = bytes.Length;

                for (var i = 0; i < 4; i++)
                {
                    if (bytes[i] != "3C" && bytes[i] != "C3" && bytes[i] != "33" && bytes[i] != "CC")
                    {
                        throw new ArgumentException("Invalid frame head");
                    }
                }

                for (var i = 0; i < 2; i++)
                {
                    if (bytes[10568 * 2 + 6 + i] != "4F" && bytes[10568 * 2 + 6 + i] != "4B")
                    {
                        throw new ArgumentException("Invalid frame tail");
                    }
                }

                for (var i = 0; i < 10568; i++)
                {
                    Amplitude[i] = Convert.ToInt32(bytes[i * 2 + 4] + bytes[i * 2 + 5], 16);
                }
            }
            else
            {
                if (data.Length != 63785)
                {
                    throw new ArgumentException("Invalid data length");
                }

                var bytes = data.Split(' ');

                var n = bytes.Length;

                for (var i = 0; i < 4; i++)
                {
                    if (bytes[i] != "3C" && bytes[i] != "C3" && bytes[i] != "33" && bytes[i] != "CC")
                    {
                        throw new ArgumentException("Invalid frame head");
                    }
                }

                if (bytes[n - 2] != "4F" || bytes[n - 1] != "4B")//-66 -65
                {
                    //throw new ArgumentException("Invalid frame tail:" + bytes[n - 2].ToString() + " " + bytes[n - 1].ToString());
                }

                for (var i = PreDummy; i < 10628 - AfterDummy; i++)
                {
                    Amplitude[i - PreDummy] = Convert.ToInt32(bytes[i * 2] + bytes[i * 2 + 1], 16);
                }
            }



        }

    }
}
