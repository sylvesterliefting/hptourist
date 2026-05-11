namespace HPTourist.Pdf.Rendering;

public readonly record struct PdfColor(int R, int G, int B)
{
    public static PdfColor White => new(255, 255, 255);
    public static PdfColor Black => new(0, 0, 0);
}

