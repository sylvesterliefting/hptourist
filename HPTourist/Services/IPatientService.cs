using HPTourist.Data.Models;

namespace HPTourist.Services;

public interface IPatientService
{
    int GetPatientAge(Patient patient);
}
