
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
        // MOCK DATA ipv database
        request = new PrescriptionRequest
        {
            Id = Guid.NewGuid(),
            Date = DateTime.Now,
            RequestStatus = PrescriptionRequest.Status.Pending,
            Patient = new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "Jan",
                LastName = "de Vries",
                Email = "jan.devries@email.nl",
                DateOfBirth = new DateTime(1985, 3, 12),
                Gender = Gender.Male,
                PracticeId = Guid.NewGuid(),
                PreferredLanguageId = Guid.NewGuid(),
            },
            Medicines = new List<Medicine>
            {
                new Medicine
                {
                    Id = Guid.NewGuid(),
                    Name = "Digoxine Eureco-Pharma",
                    AtcCode = "C01AA05 - Digoxin",
                    ActiveSubstance = "DIGOXINE",
                    PharmaceuticalForm = "Oplossing voor injectie"
                }
            }
        };

        // Prescription alvast koppelen aan patient en request
        prescription.Patient = request.Patient;
        prescription.PrescriptionRequest = request;

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