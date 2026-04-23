using HPTourist.Database;
using HPTourist.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HPTourist.Components.Shared;

public partial class PrescriptionRequests(DatabaseContext databaseContext)
{
    private readonly DatabaseContext databaseContext = databaseContext;
    private List<PrescriptionRequest> prescriptionRequests = [];

    protected override void OnInitialized()
    {
        prescriptionRequests = GetPrescriptionRequests();
    }

    private List<PrescriptionRequest> GetPrescriptionRequests()
    {
        return [.. databaseContext.PrescriptionRequests.Where(pr => pr.RequestStatus == PrescriptionRequest.Status.Pending).Include(pr => pr.Patient)];
    }
}
