using HPTourist.Database;
using HPTourist.Data.Models;
using Microsoft.EntityFrameworkCore;
using PR = HPTourist.Data.Models.PrescriptionRequest;

namespace HPTourist.Components.Pages.Dashboard;

public partial class OpenPrescriptionRequests(DatabaseContext databaseContext)
{
    private readonly DatabaseContext databaseContext = databaseContext;
    private List<PR> openRequests = [];

    protected override async Task OnInitializedAsync()
    {
        openRequests = await databaseContext.PrescriptionRequests
            .Where(pr => pr.RequestStatus == PR.Status.Pending)
            .Include(pr => pr.Patient)
            .ToListAsync();
    }
}
