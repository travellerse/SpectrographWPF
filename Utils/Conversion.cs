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

    }
}
