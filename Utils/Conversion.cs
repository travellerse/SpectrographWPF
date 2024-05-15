﻿using ScottPlot;
using System.Text;

namespace SpectrographWPF.Utils
{
    public static class Conversion
    {
        public enum ConversionType
        {
            Hex,
            Dec,
            Bin,
            Oct
        }

        public static string ToSpecifiedText(string text, ConversionType conversionType, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(text);
            return conversionType switch
            {
                ConversionType.Hex => BitConverter.ToString(bytes).Replace("-", " "),
                ConversionType.Dec => string.Join(" ", bytes),
                ConversionType.Bin => string.Join(" ", bytes),
                ConversionType.Oct => string.Join(" ", bytes),
                _ => string.Empty,
            };
        }

        public static string ToSpecifiedText(byte[] bytes, ConversionType conversionType, Encoding encoding)
        {
            return conversionType switch
            {
                ConversionType.Hex => BitConverter.ToString(bytes).Replace("-", " "),
                ConversionType.Dec => string.Join(" ", bytes),
                ConversionType.Bin => string.Join(" ", bytes),
                ConversionType.Oct => string.Join(" ", bytes),
                _ => string.Empty,
            };
        }

        public static class RgbCalculator
        {

            const int
                 LEN_MIN = 380,
                 LEN_MAX = 780,
                 LEN_STEP = 5;

            private static readonly double[]
                X = [
                0.000160, 0.000662, 0.002362, 0.007242, 0.019110, 0.043400, 0.084736, 0.140638, 0.204492, 0.264737,
                0.314679, 0.357719, 0.383734, 0.386726, 0.370702, 0.342957, 0.302273, 0.254085, 0.195618, 0.132349,
                0.080507, 0.041072, 0.016172, 0.005132, 0.003816, 0.015444, 0.037465, 0.071358, 0.117749, 0.172953,
                0.236491, 0.304213, 0.376772, 0.451584, 0.529826, 0.616053, 0.705224, 0.793832, 0.878655, 0.951162,
                1.014160, 1.074300, 1.118520, 1.134300, 1.123990, 1.089100, 1.030480, 0.950740, 0.856297, 0.754930,
                0.647467, 0.535110, 0.431567, 0.343690, 0.268329, 0.204300, 0.152568, 0.112210, 0.081261, 0.057930,
                0.040851, 0.028623, 0.019941, 0.013842, 0.009577, 0.006605, 0.004553, 0.003145, 0.002175, 0.001506,
                0.001045, 0.000727, 0.000508, 0.000356, 0.000251, 0.000178, 0.000126, 0.000090, 0.000065, 0.000046,
                0.000033
            ],

                Y = [
                0.000017, 0.000072, 0.000253, 0.000769, 0.002004, 0.004509, 0.008756, 0.014456, 0.021391, 0.029497,
                0.038676, 0.049602, 0.062077, 0.074704, 0.089456, 0.106256, 0.128201, 0.152761, 0.185190, 0.219940,
                0.253589, 0.297665, 0.339133, 0.395379, 0.460777, 0.531360, 0.606741, 0.685660, 0.761757, 0.823330,
                0.875211, 0.923810, 0.961988, 0.982200, 0.991761, 0.999110, 0.997340, 0.982380, 0.955552, 0.915175,
                0.868934, 0.825623, 0.777405, 0.720353, 0.658341, 0.593878, 0.527963, 0.461834, 0.398057, 0.339554,
                0.283493, 0.228254, 0.179828, 0.140211, 0.107633, 0.081187, 0.060281, 0.044096, 0.031800, 0.022602,
                0.015905, 0.011130, 0.007749, 0.005375, 0.003718, 0.002565, 0.001768, 0.001222, 0.000846, 0.000586,
                0.000407, 0.000284, 0.000199, 0.000140, 0.000098, 0.000070, 0.000050, 0.000036, 0.000025, 0.000018,
                0.000013
            ],

                Z = [
                0.000705, 0.002928, 0.010482, 0.032344, 0.086011, 0.197120, 0.389366, 0.656760, 0.972542, 1.282500,
                1.553480, 1.798500, 1.967280, 2.027300, 1.994800, 1.900700, 1.745370, 1.554900, 1.317560, 1.030200,
                0.772125, 0.570060, 0.415254, 0.302356, 0.218502, 0.159249, 0.112044, 0.082248, 0.060709, 0.043050,
                0.030451, 0.020584, 0.013676, 0.007918, 0.003988, 0.001091, 0.000000, 0.000000, 0.000000, 0.000000,
                0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000,
                0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000,
                0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000,
                0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000,
                0.000000
            ];

            static readonly double[]
                MATRIX_SRGB_D65 = [
             3.2404542, -1.5371385, -0.4985314,
            -0.9692660,  1.8760108,  0.0415560,
             0.0556434, -0.2040259,  1.0572252
        ];

            public static Color Calc(double len)
            {
                if (len < LEN_MIN || len > LEN_MAX)
                    return new Color();

                len -= LEN_MIN;
                var index = (int)Math.Floor(len / LEN_STEP);
                var offset = len - LEN_STEP * index;

                var x = Interpolate(X, index, offset);
                var y = Interpolate(Y, index, offset);
                var z = Interpolate(Z, index, offset);

                var m = MATRIX_SRGB_D65;

                var r = m[0] * x + m[1] * y + m[2] * z;
                var g = m[3] * x + m[4] * y + m[5] * z;
                var b = m[6] * x + m[7] * y + m[8] * z;

                r = Clip(GammaCorrect_sRGB(r));
                g = Clip(GammaCorrect_sRGB(g));
                b = Clip(GammaCorrect_sRGB(b));

                return new Color((float)r, (float)g, (float)b);
            }

            static double Interpolate(double[] values, int index, double offset)
            {
                if (offset == 0)
                    return values[index];

                var x0 = index * LEN_STEP;
                var x1 = x0 + LEN_STEP;
                var y0 = values[index];
                var y1 = values[1 + index];

                return y0 + offset * (y1 - y0) / (x1 - x0);
            }

            static double GammaCorrect_sRGB(double c)
            {
                if (c <= 0.0031308)
                    return 12.92 * c;

                var a = 0.055;
                return (1 + a) * Math.Pow(c, 1 / 2.4) - a;
            }

            static double Clip(double c)
            {
                if (c < 0)
                    return 0;
                if (c > 1)
                    return 1;
                return c;
            }
        }
    }
}
