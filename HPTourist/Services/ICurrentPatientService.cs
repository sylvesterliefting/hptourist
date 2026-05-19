namespace HPTourist.Services;

public interface ICurrentPatientService
{
    Task<Guid?> GetPatientIdAsync();
}
