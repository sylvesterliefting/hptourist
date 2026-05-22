using HPTourist.Data.Models;

namespace HPTourist.Services;

public interface IPrescriptionRequestService
{
    Task<List<PrescriptionRequest>> GetPrescriptionRequestsByStatusesAsync(int pageNumber, int pageSize, List<PrescriptionRequest.Status>? statuses = default, CancellationToken ct = default);
    Task<int> GetPrescriptionRequestsByStatusesCountAsync(List<PrescriptionRequest.Status>? statuses = default);
}
