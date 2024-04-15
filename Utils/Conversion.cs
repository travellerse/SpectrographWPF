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
            switch (conversionType)
            {
                case ConversionType.Hex:
                    return BitConverter.ToString(bytes).Replace("-", " ");
                case ConversionType.Dec:
                    return string.Join(" ", bytes);
                case ConversionType.Bin:
                    return string.Join(" ", bytes);
                case ConversionType.Oct:
                    return string.Join(" ", bytes);
                default:
                    return string.Empty;
            }
        }
    }
}
