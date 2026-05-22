
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using HPTourist.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PR = HPTourist.Data.Models.PrescriptionRequest;

namespace HPTourist.Components.Pages.PrescriptionRequest;


public partial class PrescriptionRequestPage : ComponentBase
{
    [Parameter]
    public Guid id { get; set; }

    private string searchTerm = "";
    private List<Medicine> allMedicines = [];
    private List<Medicine> results = [];
    private int pageNumber = 1;
    private readonly int pageSize = 20;
    private int totalResults = 0;
    private int totalPages = 0;
    private bool searched = false;
    private bool saved = false;
    private string? errorMessage;

    private readonly Prescription prescription = new()
    {
        Id = Guid.NewGuid(),
        Date = DateTime.UtcNow,
        Medicines = []
    };

    private PR? request;

    void AddMedicine(Medicine medicine)
    {
        if (!prescription.Medicines.Any(m => m.Id == medicine.Id))
        {
            prescription.Medicines.Add(medicine);
        }
    }

    void RemoveMedicine(Medicine medicine)
    {
        prescription.Medicines.Remove(medicine);
    }

    async Task Search()
    {
        pageNumber = 1;
        LoadResults();
    }

    async Task PreviousPage()
    {
        pageNumber--;
        LoadResults();
    }

    async Task NextPage()
    {
        pageNumber++;
        LoadResults();
    }

    protected async override Task OnInitializedAsync()
    {
        request = await Db.PrescriptionRequests
            .Include(PrescriptionRequest => PrescriptionRequest.Patient)
            .Include(PrescriptionRequest => PrescriptionRequest.Medicines)
            .FirstOrDefaultAsync(PrescriptionRequest => PrescriptionRequest.Id == id);

        if (request != null)
        {
            prescription.Patient = request.Patient;
            prescription.PrescriptionRequest = request;
        }

        allMedicines = LoadAllMedicines();
        LoadResults();
    }

    async Task SavePrescription()
    {
        if (request == null || !prescription.Medicines.Any())
            return;

        try
        {
            var employee = await Db.Employees.FirstOrDefaultAsync();//TODO: get current employee from auth context
            if (employee == null)
            {
                errorMessage = "Geen employee gevonden in de database.";
                return;
            }

            List<Guid> medicineIds = [.. prescription.Medicines.Select(Medicine => Medicine.Id)];
            var existingIds = await Db.Set<Medicine>()
                .Where(Medicine => medicineIds.Contains(Medicine.Id))
                .Select(Medicine => Medicine.Id)
                .ToHashSetAsync();

            foreach (var medicine in prescription.Medicines)
            {
                if (!existingIds.Contains(medicine.Id))
                    Db.Set<Medicine>().Add(medicine);
                else
                    Db.Entry(medicine).State = EntityState.Unchanged;
            }

            var newPrescription = new Prescription
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Patient = request.Patient,
                PrescriptionRequest = request,
                Employee = employee,
                Medicines = prescription.Medicines
            };

            Db.Prescriptions.Add(newPrescription);

            request.RequestStatus = PR.Status.Processed;

            await Db.SaveChangesAsync();

            saved = true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
    }

    private List<Medicine> LoadAllMedicines()
    {
        var path = Path.Combine(Env.WebRootPath, "data", "medicijnen.csv");
        using var reader = new StreamReader(path);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "|",
            HasHeaderRecord = true,
            BadDataFound = null
        };
        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap<MedicineMap>();
        return [.. csv.GetRecords<Medicine>()];
    }

    void LoadResults()
    {
        searched = true;
        var filtered = allMedicines
            .Where(Medicine =>
                string.IsNullOrWhiteSpace(searchTerm) ||
                Medicine.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                Medicine.ActiveSubstance.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                Medicine.AtcCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();

        totalResults = filtered.Count;
        totalPages = (int)Math.Ceiling(totalResults / (double)pageSize);
        results = filtered.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    }

    private class MedicineMap : ClassMap<Medicine>
    {
        public MedicineMap()
        {
            Map(Medicine => Medicine.Id).Convert(row => Guid.NewGuid());
            Map(Medicine => Medicine.Name).Name("PRODUCTNAAM");
            Map(Medicine => Medicine.AtcCode).Name("ATC");
            Map(Medicine => Medicine.ActiveSubstance).Name("WERKZAMESTOFFEN");
            Map(Medicine => Medicine.PharmaceuticalForm).Name("FARMACEUTISCHEVORM");
        }
    }
}
