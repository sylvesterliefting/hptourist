using System.Globalization;
using System.Text;

namespace HPTourist.Pdf.Rendering;

public static class PdfTextEncoder
{
    public static string EncodeLiteral(string value)
    {
        return ToAscii(value)
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("(", "\\(", StringComparison.Ordinal)
            .Replace(")", "\\)", StringComparison.Ordinal);
    }

    private static string ToAscii(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            builder.Append(character is >= ' ' and <= '~' ? character : '?');
        }

        return builder.ToString();
    }
}

