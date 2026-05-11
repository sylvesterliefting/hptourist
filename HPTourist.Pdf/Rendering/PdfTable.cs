namespace HPTourist.Pdf.Rendering;

public sealed record PdfTableColumn(
    string Header,
    int X,
    int MaxCharacters,
    PdfFont BodyFont = PdfFont.Regular);

public sealed record PdfTableStyle(
    int Left,
    int Right,
    int HeaderHeight,
    int RowHeight,
    PdfColor HeaderBackground,
    PdfColor HeaderText,
    PdfColor StripeBackground,
    PdfColor Border,
    PdfColor BodyText);

public sealed class PdfTableRenderer(IReadOnlyList<PdfTableColumn> columns, PdfTableStyle style)
{
    private int rowIndex;

    public int HeaderHeight => style.HeaderHeight;

    public int RowHeight => style.RowHeight;

    public void DrawHeader(PdfPageCanvas page)
    {
        page.FillColor(style.HeaderBackground);
        page.Rectangle(
            style.Left,
            page.CursorY - style.HeaderHeight,
            style.Right - style.Left,
            style.HeaderHeight,
            fill: true);

        foreach (var column in columns)
        {
            page.Text(column.Header, column.X, page.CursorY - 15, 8, PdfFont.Bold, style.HeaderText);
        }

        page.CursorY -= style.HeaderHeight;
    }

    public void DrawRow(PdfPageCanvas page, IReadOnlyList<string> values)
    {
        if (values.Count != columns.Count)
        {
            throw new ArgumentException("Table row value count must match the table column count.", nameof(values));
        }

        page.FillColor(rowIndex % 2 == 0 ? PdfColor.White : style.StripeBackground);
        page.Rectangle(style.Left, page.CursorY - style.RowHeight, style.Right - style.Left, style.RowHeight, fill: true);
        page.StrokeColor(style.Border);
        page.Line(style.Left, page.CursorY - style.RowHeight, style.Right, page.CursorY - style.RowHeight);

        for (var index = 0; index < columns.Count; index++)
        {
            var column = columns[index];
            page.Text(Truncate(values[index], column.MaxCharacters), column.X, page.CursorY - 16, 8, column.BodyFont, style.BodyText);
        }

        rowIndex++;
        page.CursorY -= style.RowHeight;
    }

    public void ResetRowStyle()
    {
        rowIndex = 0;
    }

    private static string Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "-";
        }

        var trimmed = value.Trim();
        return trimmed.Length <= maxLength ? trimmed : $"{trimmed[..(maxLength - 3)]}...";
    }
}

