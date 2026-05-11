using System.Security.Claims;
using HPTourist.Features.MedicationOverview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HPTourist.Controllers;

[Authorize(Roles = "Patient")]
[Route("patient/medication")]
public sealed class MedicationDownloadController(
    IMedicationOverviewService medicationOverviewService,
    IMedicationOverviewPdfRenderer medicationOverviewPdfRenderer) : Controller
{
    [HttpGet("download")]
    public async Task<IActionResult> DownloadAsync(CancellationToken cancellationToken)
    {
        var patientIdValue = User.FindFirstValue("PatientId");
        if (!Guid.TryParse(patientIdValue, out var patientId))
        {
            return Unauthorized();
        }

        var document = await medicationOverviewService.GetForPatientAsync(patientId, cancellationToken);
        if (document is null)
        {
            return NotFound();
        }

        var pdf = medicationOverviewPdfRenderer.Render(document);
        var fileName = $"medicatieoverzicht-{document.GeneratedAt:yyyyMMdd}.pdf";

        return File(pdf, "application/pdf", fileName);
    }
}
