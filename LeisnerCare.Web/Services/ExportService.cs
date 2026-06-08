using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LeisnerCare.Web.Services;

public class ExportService
{
    private readonly ApplicationDbContext _context;

    public ExportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportSymptomsToCsvAsync(int patientId)
    {
        var symptoms = await _context.Symptoms
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.RecordedAt)
            .ToListAsync();

        var csv = new StringBuilder();
        csv.AppendLine("Dato;Type;Vaerdi;Note");

        foreach (var s in symptoms)
        {
            var value = s.Type == SymptomType.OnOff
                ? (s.Value == 0 ? "ON" : s.Value == 1 ? "OFF" : "DYSKINESI")
                : s.Value.ToString();

            csv.AppendLine($"{s.RecordedAt:dd-MM-yyyy HH:mm};{s.Type};{value};{s.Note}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public async Task<byte[]> ExportMedicationsToCsvAsync(int patientId)
    {
        var medications = await _context.Medications
            .Where(m => m.PatientId == patientId)
            .Include(m => m.Logs)
            .ToListAsync();

        var csv = new StringBuilder();
        csv.AppendLine("Medicin;Dosis;Frekvens;Taget;Effekt;Note");

        foreach (var m in medications)
        {
            foreach (var log in m.Logs.OrderByDescending(l => l.TakenAt))
            {
                csv.AppendLine($"{m.Name};{m.Dosage};{m.Frequency};{log.TakenAt:dd-MM-yyyy HH:mm};{log.Effectiveness};{log.Note}");
            }
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }
}