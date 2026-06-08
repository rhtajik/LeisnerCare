using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Web.Services;

public class DashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Symptom>> GetSymptomsLast30DaysAsync(int patientId)
    {
        var fromDate = DateTime.UtcNow.AddDays(-30);
        return await _context.Symptoms
            .Where(s => s.PatientId == patientId && s.RecordedAt >= fromDate)
            .OrderBy(s => s.RecordedAt)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetOnOffDistributionAsync(int patientId)
    {
        var fromDate = DateTime.UtcNow.AddDays(-30);
        var symptoms = await _context.Symptoms
            .Where(s => s.PatientId == patientId && s.Type == SymptomType.OnOff && s.RecordedAt >= fromDate)
            .ToListAsync();

        return new Dictionary<string, int>
        {
            { "ON", symptoms.Count(s => s.Value == 0) },
            { "OFF", symptoms.Count(s => s.Value == 1) },
            { "DYSKINESI", symptoms.Count(s => s.Value == 2) }
        };
    }

    public async Task<List<PatientAlert>> GetRedFlagsAsync()
    {
        var fromDate = DateTime.UtcNow.AddDays(-7);
        var patients = await _context.Patients
            .Include(p => p.User)
            .ToListAsync();

        var alerts = new List<PatientAlert>();

        foreach (var patient in patients)
        {
            var offCount = await _context.Symptoms
                .CountAsync(s => s.PatientId == patient.Id && s.Type == SymptomType.OnOff
                    && s.Value == 1 && s.RecordedAt >= fromDate);

            if (offCount >= 3)
            {
                alerts.Add(new PatientAlert
                {
                    PatientName = $"{patient.User?.FirstName} {patient.User?.LastName}",
                    AlertType = "Mange OFF-perioder",
                    Count = offCount,
                    Severity = "high"
                });
            }
        }

        return alerts;
    }
}

public class PatientAlert
{
    public string PatientName { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public int Count { get; set; }
    public string Severity { get; set; } = string.Empty;
}