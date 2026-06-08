using LeisnerCare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Web.Services;

public class ConsentService
{
    private readonly ApplicationDbContext _context;

    public ConsentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasConsentAsync(string userId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.UserId == userId);

        return patient?.ConsentGiven ?? false;
    }

    public async Task<string?> GetConsentDateAsync(string userId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.UserId == userId);

        return patient?.ConsentDate?.ToString("dd-MM-yyyy HH:mm");
    }

    public async Task GiveConsentAsync(string userId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (patient != null)
        {
            patient.ConsentGiven = true;
            patient.ConsentDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task WithdrawConsentAsync(string userId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (patient != null)
        {
            patient.ConsentGiven = false;
            patient.ConsentDate = null;
            await _context.SaveChangesAsync();
        }
    }
}