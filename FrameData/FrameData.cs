﻿namespace SpectrographWPF.FrameData
{

    public class FrameData
    {
        public double[] Amplitude { get; } = new double[10550];

        //public PeakData[]? Peaks { get; set; }

        public long Timestamp;

        public bool IsVirtual;

        private const int PreDummy = 64;

        private const int AfterDummy = 14;

        //模块返回一帧数据（接收用HEX格式），包括4个帧头（0x3c,0xc3,0x33,0xcc），10568*2 个像素数据（输出10568*2个不带坐标16进制数, 10568个像素，每个像素2个字节，按顺序，高4位在前，低8位在后,如0x08 0xfc），两个帧尾（0x4f,0x4b）
        //包含空格总长63425
        public FrameData(string data, bool isVirtual)
        {
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            IsVirtual = isVirtual;
            var n = isVirtual ? 21262 : 21142;
            var length = isVirtual ? 63785 : 63425;

            if (data.Length != length) throw new ArgumentException($"Invalid data length:{data.Length}");

            var bytes = data.Split(' ');

            if (bytes[0] != "3C" || bytes[1] != "C3" || bytes[2] != "33" || bytes[3] != "CC") throw new ArgumentException($"Invalid frame head:{bytes[0]} {bytes[1]} {bytes[2]} {bytes[3]}");

            if (bytes[n - 2] != "4F" || bytes[n - 1] != "4B") throw new ArgumentException($"Invalid frame tail:{bytes[n - 2]} {bytes[n - 1]}");

            for (var i = PreDummy; i < (n / 2 - 3) - AfterDummy; i++)
            {
                Amplitude[i - PreDummy] = Convert.ToInt32(bytes[i * 2] + bytes[i * 2 + 1], 16);
            }
            Amplitude = Utils.Algorithm.DataSmooth.Apply(Amplitude, 7);
        }

        public FrameData()
        {
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            IsVirtual = true;
            var random = new Random();
            int num = 12;
            double lambda = Amplitude.Length / num;
            for (var i = 0; i < Amplitude.Length; i++)
            {
                Amplitude[i] = random.Next(630, 650);
                Amplitude[i] += 600 * Math.Sin(2 * Math.PI * i / lambda);
                //Amplitude[i] += (-8.625e-5 * Math.Pow(i - 5275, 2) + 2400);
            }
        }




    }
}
