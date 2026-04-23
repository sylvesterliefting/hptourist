
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
    private int pageSize = 30;
    private int totalResults = 0;
    private int totalPages = 0;
    private bool searched = false;

    public Prescription prescription = new()
    {
        Id = Guid.NewGuid(),
        Date = DateTime.Now,
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
        // PrescriptionRequest inclusief Patient en Medicines ophalen
        request = await Db.PrescriptionRequests
            .Include(r => r.Patient)
            .Include(r => r.Medicines)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request != null)
        {
            // Prescription alvast koppelen aan patient en request
            prescription.Patient = request.Patient;
            prescription.PrescriptionRequest = request;
        }

        await LoadResults();
    }

    async Task SavePrescription()
    {
        if (request == null || !prescription.Medicines.Any())
            return;

        prescription.Date = DateTime.Now;
        prescription.Patient = request.Patient;
        prescription.PrescriptionRequest = request;
        // TODO: Koppel Employee indien nodig

        Db.Prescriptions.Add(prescription);
        await Db.SaveChangesAsync();
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