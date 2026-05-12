using HPTourist.Data.Models;
using HPTourist.Services;
using Microsoft.AspNetCore.Components;

namespace HPTourist.Components.Shared;

public partial class PrescriptionRequests(IPrescriptionRequestService prescriptionRequestService, IPatientService patientService)
{
    [Parameter]
    public List<PrescriptionRequest.Status>? StatusFilter { get; set; }

    private List<PrescriptionRequest>? prescriptionRequests = null;

    protected override async Task OnInitializedAsync()
    {
        prescriptionRequests = await prescriptionRequestService.GetPrescriptionRequestsByStatusesAsync(StatusFilter);
    }
}
