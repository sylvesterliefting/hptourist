using HPTourist.Data.Models;

namespace HPTourist.Services;

public class PatientService : IPatientService
{
    public int GetPatientAge(Patient patient)
    {
        var dateOfBirth = patient.DateOfBirth;
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}
