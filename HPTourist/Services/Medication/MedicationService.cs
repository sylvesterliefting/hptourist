using HPTourist.Data.DTOs.Medication;
using HPTourist.Data.Models;
using HPTourist.Database;
using HPTourist.Services.DateTime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HPTourist.Services.Medication;

public class MedicationService : IMedicationService
{
    private readonly DatabaseContext _db;
    private readonly IMedicationMapper _mapper;
    private readonly IDateTimeService _clock;
    private readonly IStringLocalizer<MedicationService> _localizer;

    public MedicationService(
        DatabaseContext db,
        IMedicationMapper mapper,
        IDateTimeService clock,
        IStringLocalizer<MedicationService> localizer)
    {
        _db = db;
        _mapper = mapper;
        _clock = clock;
        _localizer = localizer;
    }

    public async Task<List<PrescriptionDto>> GetPrescriptionsAsync(Guid patientId, CancellationToken ct = default)
    {
        var prescriptions = await _db.Prescriptions
            .AsNoTracking()
            .Where(p => p.PatientId == patientId)
            .Include(p => p.Medicines)
            .OrderByDescending(p => p.Date)
            .ToListAsync(ct);

        return prescriptions.Select(_mapper.MapToDto).ToList();
    }

    public async Task<List<PrescriptionRequestDto>> GetPrescriptionRequestsAsync(Guid patientId, CancellationToken ct = default)
    {
        var requests = await _db.PrescriptionRequests
            .AsNoTracking()
            .Where(r => r.PatientId == patientId)
            .Include(r => r.Medicines)
            .OrderByDescending(r => r.Date)
            .ToListAsync(ct);

        return requests.Select(_mapper.MapToDto).ToList();
    }

    public async Task<PrescriptionRequestDto?> GetPrescriptionRequestByMedicineIdAsync(Guid patientId, Guid medicineId, CancellationToken ct = default)
    {
        var request = await _db.PrescriptionRequests
            .AsNoTracking()
            .Include(r => r.Medicines)
            .Where(r => r.PatientId == patientId)
            .SingleOrDefaultAsync(r => r.Medicines.Any(m => m.Id == medicineId), ct);

        return request is not null ? _mapper.MapToDto(request) : null;
    }

    public async Task AddPrescriptionRequestAsync(Guid patientId, MedicineForm form, CancellationToken ct = default)
    {
        if (!await _db.Patients.AnyAsync(p => p.Id == patientId, ct))
        {
            throw new MedicationBusinessException(_localizer["PatientNotFound"]);
        }

        var request = new PrescriptionRequest
        {
            PatientId = patientId,
            Medicines = [ _mapper.MapToEntity(form) ],
            RequestStatus = PrescriptionRequest.Status.Pending,
            Date = _clock.UtcNow
        };

        _db.PrescriptionRequests.Add(request);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateMedicineAsync(Guid patientId, Guid medicineId, MedicineForm form, CancellationToken ct = default)
    {
        var request = await _db.PrescriptionRequests
            .Include(r => r.Medicines)
            .Where(r => r.PatientId == patientId)
            .SingleOrDefaultAsync(r => r.Medicines.Any(m => m.Id == medicineId), ct);

        if (request is null)
        {
            throw new MedicationNotFoundException(_localizer["MedicationNotFound"]);
        }

        if (request.RequestStatus != PrescriptionRequest.Status.Pending)
        {
            throw new MedicationBusinessException(_localizer["OnlyPendingCanBeModified"]);
        }

        var medicine = request.Medicines.Single(m => m.Id == medicineId);
        _mapper.ApplyToEntity(medicine, form);

        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteMedicineAsync(Guid patientId, Guid prescriptionRequestId, Guid medicineId, CancellationToken ct = default)
    {
        var request = await _db.PrescriptionRequests
            .Include(r => r.Medicines)
            .SingleOrDefaultAsync(r => r.Id == prescriptionRequestId && r.PatientId == patientId, ct);

        if (request is null)
        {
            throw new MedicationNotFoundException(_localizer["MedicationNotFound"]);
        }

        if (request.RequestStatus != PrescriptionRequest.Status.Pending)
        {
            throw new MedicationBusinessException(_localizer["OnlyPendingCanBeDeleted"]);
        }

        var medicine = request.Medicines.SingleOrDefault(m => m.Id == medicineId);
        if (medicine is null)
        {
            throw new MedicationNotFoundException(_localizer["MedicationNotFound"]);
        }

        if (request.Medicines.Count == 1)
        {
            _db.PrescriptionRequests.Remove(request);
        }
        else
        {
            _db.Medicines.Remove(medicine);
        }

        await _db.SaveChangesAsync(ct);
    }
}
