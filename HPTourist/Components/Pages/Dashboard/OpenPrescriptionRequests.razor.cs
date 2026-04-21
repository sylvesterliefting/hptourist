using HPTourist.Database;
using HPTourist.Data.Models;

namespace HPTourist.Components.Pages.Dashboard;

public partial class OpenPrescriptionRequests(DatabaseContext databaseContext)
{
    private readonly DatabaseContext databaseContext = databaseContext;
    private List<PrescriptionRequest> openRequests = [];

    protected override void OnInitialized()
    {
        openRequests = GetOpenPrescriptionRequests();
    }

    private List<PrescriptionRequest> GetOpenPrescriptionRequests()
    {
        return [.. databaseContext.PrescriptionRequests.Where(pr => pr.RequestStatus == PrescriptionRequest.Status.Pending)];
    }
}
