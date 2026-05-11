using System.Globalization;
using System.Text;

namespace HPTourist.Pdf.Rendering;

public sealed class PdfPageCanvas(PdfPageSettings settings)
{
    private readonly StringBuilder builder = new();

    public PdfPageSettings Settings { get; } = settings;

    public int CursorY { get; set; }

    public void Text(
        string text,
        int x,
        int y,
        int size,
        PdfFont font = PdfFont.Regular,
        PdfColor? color = null)
    {
        if (color is not null)
        {
            FillColor(color.Value);
        }

        var fontName = font == PdfFont.Bold ? "F2" : "F1";
        builder.Append(
            CultureInfo.InvariantCulture,
            $"BT /{fontName} {size} Tf {x} {y} Td ({PdfTextEncoder.EncodeLiteral(text)}) Tj ET\n");
    }

    public void Rectangle(
        int x,
        int y,
        int width,
        int height,
        bool fill = false,
        bool stroke = false)
    {
        var operation = fill && stroke ? "B" : fill ? "f" : "S";
        builder.Append(CultureInfo.InvariantCulture, $"{x} {y} {width} {height} re {operation}\n");
    }

    public void Line(int x1, int y1, int x2, int y2)
    {
        builder.Append(CultureInfo.InvariantCulture, $"{x1} {y1} m {x2} {y2} l S\n");
    }

    public void FillColor(PdfColor color)
    {
        builder.Append(
            CultureInfo.InvariantCulture,
            $"{color.R / 255d:0.###} {color.G / 255d:0.###} {color.B / 255d:0.###} rg\n");
    }

    public void StrokeColor(PdfColor color)
    {
        builder.Append(
            CultureInfo.InvariantCulture,
            $"{color.R / 255d:0.###} {color.G / 255d:0.###} {color.B / 255d:0.###} RG\n");
    }

    public override string ToString() => builder.ToString();
}

