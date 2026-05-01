
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using HPTourist.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace HPTourist.Components.Pages;


public partial class PrescriptionRequestPage : ComponentBase
{
    [Parameter]
    public Guid id { get; set; }

    private string searchTerm = "";
    private List<Medicine> results = [];
    private int pageNumber = 1;
    private int pageSize = 20;
    private int totalResults = 0;
    private int totalPages = 0;
    private bool searched = false;
    private bool saved = false;
    private string? errorMessage;

    public Prescription prescription = new()
    {
        Id = Guid.NewGuid(),
        Date = DateTime.UtcNow,
        Medicines = []
    };

    private PrescriptionRequest? request;

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
        await LoadResults();
    }

    async Task PreviousPage()
    {
        pageNumber--;
        await LoadResults();
    }

    async Task NextPage()
    {
        pageNumber++;
        await LoadResults();
    }

    protected async override Task OnInitializedAsync()
    {
        // PrescriptionRequest uit de database ophalen
        request = await Db.PrescriptionRequests
            .Include(r => r.Patient)
            .Include(r => r.Medicines)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request != null)
        {
            prescription.Patient = request.Patient;
            prescription.PrescriptionRequest = request;
        }

        await LoadResults();
    }



    async Task SavePrescription()
    {
        if (request == null || !prescription.Medicines.Any())
            return;

        try
        {
            var employee = await Db.Employees.FirstOrDefaultAsync();
            if (employee == null)
            {
                errorMessage = "Geen employee gevonden in de database.";
                return;
            }

            foreach (var medicine in prescription.Medicines)
            {
                var exists = await Db.Set<Medicine>().AnyAsync(m => m.Id == medicine.Id);
                if (!exists)
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
            await Db.SaveChangesAsync();

            request.RequestStatus = PrescriptionRequest.Status.Processed;
            await Db.SaveChangesAsync();

            saved = true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
    }

    async Task LoadResults()
    {
        searched = true;
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

        var allMatches = csv.GetRecords<Medicine>()
            .Where(m =>
                string.IsNullOrWhiteSpace(searchTerm) ||
                m.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                m.ActiveSubstance.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                m.AtcCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();

        totalResults = allMatches.Count;
        totalPages = (int)Math.Ceiling(totalResults / (double)pageSize);
        results = allMatches.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    }

    private class MedicineMap : ClassMap<Medicine>
    {
        public MedicineMap()
        {
            Map(m => m.Id).Convert(row => Guid.NewGuid());
            Map(m => m.Name).Name("PRODUCTNAAM");
            Map(m => m.AtcCode).Name("ATC");
            Map(m => m.ActiveSubstance).Name("WERKZAMESTOFFEN");
            Map(m => m.PharmaceuticalForm).Name("FARMACEUTISCHEVORM");
        }
    }
}