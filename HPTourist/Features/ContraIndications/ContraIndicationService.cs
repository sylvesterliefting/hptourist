using HPTourist.Data.Models;
using HPTourist.Database;
using HPTourist.Services;
using Microsoft.EntityFrameworkCore;

namespace HPTourist.Features.ContraIndications;

public sealed class ContraIndicationService(
    DatabaseContext db,
    IFarmacotherapeutischKompasScraperService fkScraperService) : IContraIndicationService
{
    public async Task<IReadOnlyList<ContraIndicationListItem>> GetForPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var prescriptions = await db.Prescriptions
            .AsNoTracking()
            .Where(prescription => prescription.PatientId == patientId)
            .Include(prescription => prescription.Medicines)
            .OrderByDescending(prescription => prescription.Date)
            .ToListAsync(cancellationToken);

        var medicinesByActiveSubstance = prescriptions
            .SelectMany(prescription => prescription.Medicines.Select(medicine => new PrescribedMedicine(prescription.Date, medicine)))
            .Where(medicine => !string.IsNullOrWhiteSpace(medicine.ActiveSubstance))
            .GroupBy(medicine => medicine.ActiveSubstance.Trim(), StringComparer.OrdinalIgnoreCase)
            .OrderBy(group => group.Key)
            .ToList();

        var contraIndications = new List<ContraIndicationListItem>();
        foreach (var medicineGroup in medicinesByActiveSubstance)
        {
            contraIndications.Add(await CreateListItemAsync(medicineGroup));
        }

        return contraIndications;
    }

    private async Task<ContraIndicationListItem> CreateListItemAsync(IGrouping<string, PrescribedMedicine> medicineGroup)
    {
        var activeSubstance = medicineGroup.Key;
        var text = await fkScraperService.GetContraIndicationAsync(activeSubstance);

        return new ContraIndicationListItem(
            activeSubstance,
            FormatMedicineNames(medicineGroup),
            FormatPrescriptionDates(medicineGroup),
            text);
    }

    private static string FormatMedicineNames(IEnumerable<PrescribedMedicine> medicines)
    {
        return string.Join(", ", medicines
            .Select(medicine => medicine.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order());
    }

    private static string FormatPrescriptionDates(IEnumerable<PrescribedMedicine> medicines)
    {
        return string.Join(", ", medicines
            .Select(medicine => medicine.Date.ToString("dd-MM-yyyy"))
            .Distinct()
            .Order());
    }

    private sealed record PrescribedMedicine(DateTime Date, Medicine Medicine)
    {
        public string Name => Medicine.Name;
        public string ActiveSubstance => Medicine.ActiveSubstance;
    }
}
