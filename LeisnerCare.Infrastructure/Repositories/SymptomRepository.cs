using LeisnerCare.Core.Entities;
using LeisnerCare.Core.Interfaces;
using LeisnerCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Infrastructure.Repositories;

public class SymptomRepository : ISymptomRepository
{
    private readonly ApplicationDbContext _context;

    public SymptomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Symptom>> GetByPatientIdAsync(int patientId)
    {
        return await _context.Symptoms
            .Include(s => s.Patient)
                .ThenInclude(p => p.User)
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.RecordedAt)
            .ToListAsync();
    }

    public async Task<List<Symptom>> GetAllAsync()
    {
        return await _context.Symptoms
            .Include(s => s.Patient)
                .ThenInclude(p => p.User)  
            .OrderByDescending(s => s.RecordedAt)
            .ToListAsync();
    }

    public async Task<Symptom> AddAsync(Symptom symptom)
    {
        _context.Symptoms.Add(symptom);
        await _context.SaveChangesAsync();
        return symptom;
    }
}