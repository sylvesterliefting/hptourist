namespace HPTourist.Features.MedicationOverview;

public interface IMedicationOverviewPdfRenderer
{
    byte[] Render(MedicationOverviewDocument document);
}

