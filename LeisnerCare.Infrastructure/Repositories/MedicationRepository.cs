using LeisnerCare.Core.Entities;
using LeisnerCare.Core.Interfaces;
using LeisnerCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Infrastructure.Repositories;

public class MedicationRepository : IMedicationRepository
{
    private readonly ApplicationDbContext _context;

    public MedicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Medication>> GetByPatientIdAsync(int patientId)
    {
        return await _context.Medications
            .Include(m => m.Logs)
            .Where(m => m.PatientId == patientId)
            .ToListAsync();
    }

    public async Task<Medication?> GetByIdAsync(int id)
    {
        return await _context.Medications
            .Include(m => m.Logs)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Medication> AddAsync(Medication medication)
    {
        _context.Medications.Add(medication);
        await _context.SaveChangesAsync();
        return medication;
    }

    public async Task<MedicationLog> AddLogAsync(MedicationLog log)
    {
        _context.MedicationLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }
}