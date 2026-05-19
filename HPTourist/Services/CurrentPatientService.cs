using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace HPTourist.Services;

public sealed class CurrentPatientService(AuthenticationStateProvider authenticationStateProvider) : ICurrentPatientService
{
    public async Task<Guid?> GetPatientIdAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var patientIdValue = authState.User.FindFirstValue("PatientId");

        return Guid.TryParse(patientIdValue, out var patientId)
            ? patientId
            : null;
    }
}
