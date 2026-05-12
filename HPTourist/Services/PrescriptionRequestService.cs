using HPTourist.Data.Models;
using HPTourist.Database;
using Microsoft.EntityFrameworkCore;

namespace HPTourist.Services;

public class PrescriptionRequestService(DatabaseContext databaseContext) : IPrescriptionRequestService
{
    public Task<List<PrescriptionRequest>> GetPrescriptionRequestsByStatusesAsync(List<PrescriptionRequest.Status>? statuses = default, CancellationToken ct = default)
    {
        var prescriptionRequests = databaseContext.PrescriptionRequests.Where(pr => statuses == null || statuses.Contains(pr.RequestStatus));
        return prescriptionRequests.Include(pr => pr.Patient).ToListAsync(ct);
    }
}
