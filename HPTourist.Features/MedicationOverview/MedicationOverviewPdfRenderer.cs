using System.Globalization;
using HPTourist.Pdf.Rendering;

namespace HPTourist.Features.MedicationOverview;

public sealed class MedicationOverviewPdfRenderer(PdfDocumentWriter pdfDocumentWriter) : IMedicationOverviewPdfRenderer
{
    private const int RowHeight = 24;
    private const int TableHeaderHeight = 22;
    private const string PracticeBrandName = "Huisartsenpraktijk Tourist Doctor Amsterdam";

    private static readonly PdfPageSettings PageSettings = new();
    private static readonly PdfColor Primary = new(28, 88, 140);
    private static readonly PdfColor LightText = new(225, 239, 249);
    private static readonly PdfColor Border = new(218, 226, 234);
    private static readonly PdfColor Muted = new(96, 110, 124);
    private static readonly PdfColor CardBackground = new(246, 249, 252);
    private static readonly PdfColor TableStripe = new(248, 250, 252);
    private static readonly PdfColor BodyText = new(32, 43, 54);
    private static readonly IReadOnlyList<PdfTableColumn> MedicationTableColumns =
    [
        new("Datum", 56, 10),
        new("Medicatie", 112, 25, PdfFont.Bold),
        new("Werkzame stof", 244, 23),
        new("Vorm", 364, 13),
        new("ATC", 430, 10),
        new("Status", 486, 14)
    ];

    private static readonly PdfTableStyle MedicationTableStyle = new(
        PageSettings.Margin,
        PageSettings.Width - PageSettings.Margin,
        TableHeaderHeight,
        RowHeight,
        Primary,
        PdfColor.White,
        TableStripe,
        new PdfColor(226, 232, 238),
        BodyText);

    public byte[] Render(MedicationOverviewDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var pages = BuildPages(document);
        return pdfDocumentWriter.Write(pages);
    }

    private static IReadOnlyList<PdfPageCanvas> BuildPages(MedicationOverviewDocument document)
    {
        var pages = new List<PdfPageCanvas>();
        var page = CreatePage(document);
        pages.Add(page);
        var tableRenderer = CreateMedicationTableRenderer();

        DrawPatientCard(page, document);
        page.CursorY -= 22;

        if (document.Sections.All(section => section.Rows.Count == 0))
        {
            DrawSectionTitle(page, "Medicatie");
            DrawEmptyState(page, "Er is nog geen medicatie bekend.");
            DrawFooters(pages);
            return pages;
        }

        foreach (var section in document.Sections)
        {
            EnsureSpace(ref page, pages, document, TableHeaderHeight + RowHeight + 34);
            DrawSectionTitle(page, section.Title);
            tableRenderer.ResetRowStyle();

            if (section.Rows.Count == 0)
            {
                DrawEmptyState(page, $"Geen {section.Title.ToLowerInvariant()}.");
                continue;
            }

            tableRenderer.DrawHeader(page);
            foreach (var row in section.Rows)
            {
                EnsureSpace(ref page, pages, document, RowHeight + 8, tableRenderer);
                tableRenderer.DrawRow(page, ToTableValues(row));
            }

            page.CursorY -= 14;
        }

        DrawFooters(pages);
        return pages;
    }

    private static PdfPageCanvas CreatePage(MedicationOverviewDocument document)
    {
        var page = new PdfPageCanvas(PageSettings);
        DrawHeader(page, document);
        return page;
    }

    private static void EnsureSpace(
        ref PdfPageCanvas page,
        List<PdfPageCanvas> pages,
        MedicationOverviewDocument document,
        int requiredHeight,
        PdfTableRenderer? continuedTable = null)
    {
        if (page.CursorY - requiredHeight >= PageSettings.BottomLimit)
        {
            return;
        }

        page = CreatePage(document);
        pages.Add(page);

        if (continuedTable is not null)
        {
            continuedTable.DrawHeader(page);
        }
    }

    private static void DrawHeader(PdfPageCanvas page, MedicationOverviewDocument document)
    {
        page.FillColor(Primary);
        page.Rectangle(0, PageSettings.Height - 74, PageSettings.Width, 74, fill: true);

        page.Text("Medicatieoverzicht", PageSettings.Margin, 780, 22, PdfFont.Bold, PdfColor.White);
        page.Text(PracticeBrandName, PageSettings.Margin, 757, 10, PdfFont.Regular, LightText);
        page.Text($"Gegenereerd: {document.GeneratedAt:dd-MM-yyyy HH:mm}", PageSettings.Margin, 741, 9, PdfFont.Regular, LightText);

        DrawBrandMark(page);

        page.StrokeColor(new PdfColor(210, 225, 236));
        page.Line(PageSettings.Margin, PageSettings.Height - 92, PageSettings.Width - PageSettings.Margin, PageSettings.Height - 92);
        page.CursorY = PageSettings.Height - 124;
    }

    private static void DrawBrandMark(PdfPageCanvas page)
    {
        const int x = 472;
        const int y = 760;

        page.FillColor(PdfColor.White);
        page.Rectangle(x, y, 76, 44, fill: true);
        page.FillColor(Primary);
        page.Rectangle(x + 4, y + 4, 28, 36, fill: true);
        page.Text("HP", x + 8, y + 17, 14, PdfFont.Bold, PdfColor.White);
        page.Text("Tourist", x + 38, y + 24, 10, PdfFont.Bold, Primary);
        page.Text("Doctor", x + 38, y + 12, 8, PdfFont.Regular, new PdfColor(72, 91, 105));
    }

    private static void DrawPatientCard(PdfPageCanvas page, MedicationOverviewDocument document)
    {
        page.FillColor(CardBackground);
        page.Rectangle(PageSettings.Margin, page.CursorY - 76, PageSettings.Width - PageSettings.Margin * 2, 76, fill: true);
        page.StrokeColor(Border);
        page.Rectangle(PageSettings.Margin, page.CursorY - 76, PageSettings.Width - PageSettings.Margin * 2, 76, stroke: true);

        page.Text("Patientgegevens", PageSettings.Margin + 16, page.CursorY - 20, 12, PdfFont.Bold, Primary);
        page.Text($"Naam: {document.Patient.FullName}", PageSettings.Margin + 16, page.CursorY - 41, 10);
        page.Text($"Geboortedatum: {document.Patient.DateOfBirth:dd-MM-yyyy}", PageSettings.Margin + 16, page.CursorY - 58, 10);
        page.Text($"Praktijk: {Truncate(document.Patient.PracticeName, 30)}", 310, page.CursorY - 41, 10);
        page.Text("Document: downloadbaar medicatieoverzicht", 310, page.CursorY - 58, 10);

        page.CursorY -= 98;
    }

    private static void DrawSectionTitle(PdfPageCanvas page, string title)
    {
        page.Text(title, PageSettings.Margin, page.CursorY, 14, PdfFont.Bold, Primary);
        page.CursorY -= 24;
    }

    private static IReadOnlyList<string> ToTableValues(MedicationOverviewRow row)
    {
        return
        [
            row.Date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
            row.Name,
            row.ActiveSubstance,
            row.PharmaceuticalForm,
            row.AtcCode,
            row.Status
        ];
    }

    private static void DrawEmptyState(PdfPageCanvas page, string message)
    {
        page.FillColor(CardBackground);
        page.Rectangle(PageSettings.Margin, page.CursorY - 44, PageSettings.Width - PageSettings.Margin * 2, 44, fill: true);
        page.StrokeColor(Border);
        page.Rectangle(PageSettings.Margin, page.CursorY - 44, PageSettings.Width - PageSettings.Margin * 2, 44, stroke: true);
        page.Text(message, PageSettings.Margin + 16, page.CursorY - 27, 10, PdfFont.Regular, new PdfColor(72, 91, 105));
        page.CursorY -= 58;
    }

    private static void DrawFooters(IReadOnlyList<PdfPageCanvas> pages)
    {
        for (var index = 0; index < pages.Count; index++)
        {
            var page = pages[index];

            page.StrokeColor(Border);
            page.Line(PageSettings.Margin, PageSettings.FooterY + 18, PageSettings.Width - PageSettings.Margin, PageSettings.FooterY + 18);
            page.Text("HP Tourist - vertrouwelijk medisch overzicht", PageSettings.Margin, PageSettings.FooterY, 8, PdfFont.Regular, Muted);
            page.Text($"Pagina {index + 1} van {pages.Count}", PageSettings.Width - PageSettings.Margin - 72, PageSettings.FooterY, 8, PdfFont.Regular, Muted);
        }
    }

    private static PdfTableRenderer CreateMedicationTableRenderer()
    {
        return new PdfTableRenderer(MedicationTableColumns, MedicationTableStyle);
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

