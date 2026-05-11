namespace HPTourist.Pdf.Rendering;

public sealed record PdfPageSettings(
    int Width = 595,
    int Height = 842,
    int Margin = 48,
    int FooterY = 34,
    int BottomLimit = 82);

