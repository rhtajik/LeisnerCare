using LeisnerCare.Core.Entities;
using LeisnerCare.Core.Interfaces;
using LeisnerCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Infrastructure.Repositories;

public class ObservationRepository : IObservationRepository
{
    private readonly ApplicationDbContext _context;

    public ObservationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Observation>> GetByPatientIdAsync(int patientId)
    {
        return await _context.Observations
            .Where(o => o.PatientId == patientId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Observation>> GetAllAsync()
    {
        return await _context.Observations
            .Include(o => o.Patient)
                .ThenInclude(p => p.User)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Observation> AddAsync(Observation observation)
    {
        _context.Observations.Add(observation);
        await _context.SaveChangesAsync();
        return observation;
    }
}