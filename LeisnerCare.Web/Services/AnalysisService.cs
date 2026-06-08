using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Web.Services;

public class AnalysisService
{
    private readonly ApplicationDbContext _context;

    public AnalysisService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WeeklyInsight> GetWeeklyInsightAsync(int patientId)
    {
        var fromDate = DateTime.UtcNow.AddDays(-7);

        var symptoms = await _context.Symptoms
            .Where(s => s.PatientId == patientId && s.RecordedAt >= fromDate)
            .ToListAsync();

        var offCount = symptoms.Count(s => s.Type == SymptomType.OnOff && s.Value == 1);
        var tremorAvg = symptoms.Where(s => s.Type == SymptomType.Tremor).Any()
            ? symptoms.Where(s => s.Type == SymptomType.Tremor).Average(s => s.Value)
            : 0;
        var moodAvg = symptoms.Where(s => s.Type == SymptomType.Mood).Any()
            ? symptoms.Where(s => s.Type == SymptomType.Mood).Average(s => s.Value)
            : 0;

        return new WeeklyInsight
        {
            OffPeriods = offCount,
            TremorAverage = Math.Round(tremorAvg, 1),
            MoodAverage = Math.Round(moodAvg, 1),
            TotalRegistrations = symptoms.Count,
            Recommendation = offCount >= 3
                ? "⚠️ Flere OFF-perioder denne uge – kontakt læge"
                : "✅ Stabil uge – fortsæt som planlagt"
        };
    }
}

public class WeeklyInsight
{
    public int OffPeriods { get; set; }
    public double TremorAverage { get; set; }
    public double MoodAverage { get; set; }
    public int TotalRegistrations { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}