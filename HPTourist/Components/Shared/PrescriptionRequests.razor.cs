using HPTourist.Data.Models;
using HPTourist.Services;
using Microsoft.AspNetCore.Components;

namespace HPTourist.Components.Shared;

public partial class PrescriptionRequests(IPrescriptionRequestService prescriptionRequestService, IPatientService patientService, IHttpContextAccessor httpContextAccessor)
{
    [Parameter, EditorRequired]
    public int PageNumber { get; set; }

    [Parameter, EditorRequired]
    public string PageParameterName { get; set; }

    [Parameter]
    public List<PrescriptionRequest.Status>? StatusFilter { get; set; }

    public const int PageSize = 10;

    private List<PrescriptionRequest>? prescriptionRequests = null;
    private int totalPrescriptionRequests = 0;
    private int totalPages = 0;

    protected override async Task OnInitializedAsync()
    {
        prescriptionRequests = await prescriptionRequestService.GetPrescriptionRequestsByStatusesAsync(PageNumber, PageSize, StatusFilter);
        totalPrescriptionRequests = await prescriptionRequestService.GetPrescriptionRequestsByStatusesCountAsync(StatusFilter);
        totalPages = (int)Math.Floor((double)(totalPrescriptionRequests - 1) / PageSize);
    }

    protected PrescriptionRequestPage GetPrescriptionRequestPage(int pageNumber)
    {
        return new PrescriptionRequestPage
        {
            Number = pageNumber,
            Url = httpContextAccessor.HttpContext?.Request.Path.Value + "?" + PageParameterName + "=" + pageNumber ?? ""
        };
    }

    protected int GetPreviousPageNumber()
    {
        return Math.Max(0, PageNumber - 1);
    }

    protected int GetNextPageNumber()
    {
        return Math.Min(totalPages, PageNumber + 1);
    }

    public sealed class PrescriptionRequestPage
    {
        public int Number { get; set; } = default;
        public string Url { get; set; } = default!;
    }
}
