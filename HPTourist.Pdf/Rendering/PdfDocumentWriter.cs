using System.Text;

namespace HPTourist.Pdf.Rendering;

public sealed class PdfDocumentWriter
{
    public byte[] Write(IReadOnlyList<PdfPageCanvas> pages)
    {
        ArgumentNullException.ThrowIfNull(pages);

        if (pages.Count == 0)
        {
            throw new ArgumentException("A PDF document must contain at least one page.", nameof(pages));
        }

        var objects = BuildObjects(pages);

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true) { NewLine = "\n" };
        var offsets = new List<long> { 0 };

        writer.Write("%PDF-1.4\n");
        for (var index = 0; index < objects.Count; index++)
        {
            writer.Flush();
            offsets.Add(stream.Position);
            writer.Write($"{index + 1} 0 obj\n{objects[index]}\nendobj\n");
        }

        writer.Flush();
        var xrefOffset = stream.Position;
        writer.Write($"xref\n0 {objects.Count + 1}\n");
        writer.Write("0000000000 65535 f \n");

        foreach (var offset in offsets.Skip(1))
        {
            writer.Write($"{offset:0000000000} 00000 n \n");
        }

        writer.Write("trailer\n");
        writer.Write($"<< /Size {objects.Count + 1} /Root 1 0 R >>\n");
        writer.Write($"startxref\n{xrefOffset}\n%%EOF");
        writer.Flush();

        return stream.ToArray();
    }

    private static List<string> BuildObjects(IReadOnlyList<PdfPageCanvas> pages)
    {
        var objects = new List<string>
        {
            "<< /Type /Catalog /Pages 2 0 R >>"
        };

        var pageObjectNumbers = Enumerable.Range(0, pages.Count)
            .Select(index => 3 + index * 2)
            .ToList();

        objects.Add($"<< /Type /Pages /Count {pages.Count} /Kids [{string.Join(" ", pageObjectNumbers.Select(number => $"{number} 0 R"))}] >>");

        for (var index = 0; index < pages.Count; index++)
        {
            var page = pages[index];
            var pageObjectNumber = 3 + index * 2;
            var contentObjectNumber = pageObjectNumber + 1;
            var stream = page.ToString();

            objects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {page.Settings.Width} {page.Settings.Height}] /Resources << /Font << /F1 << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> /F2 << /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >> >> >> /Contents {contentObjectNumber} 0 R >>");
            objects.Add($"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}\nendstream");
        }

        return objects;
    }
}

