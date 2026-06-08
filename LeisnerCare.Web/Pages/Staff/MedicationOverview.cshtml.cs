using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Web.Pages.Staff;

[Authorize(Roles = "Staff")]
public class MedicationOverviewModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public MedicationOverviewModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<MedicationViewModel> Medications { get; set; } = new();

    public class MedicationViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string MedicationName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public bool TakenToday { get; set; }
        public int? Effectiveness { get; set; }
    }

    public async Task OnGetAsync()
    {
        var meds = await _context.Medications
            .Include(m => m.Patient)
                .ThenInclude(p => p.User)
            .Include(m => m.Logs)
            .ToListAsync();

        Medications = meds.Select(m =>
        {
            var todayLog = m.Logs.FirstOrDefault(l => l.TakenAt.Date == DateTime.UtcNow.Date);
            return new MedicationViewModel
            {
                Id = m.Id,
                PatientName = $"{m.Patient?.User?.FirstName} {m.Patient?.User?.LastName}",
                MedicationName = m.Name,
                Dosage = m.Dosage,
                Frequency = m.Frequency,
                TakenToday = todayLog != null,
                Effectiveness = todayLog?.Effectiveness
            };
        }).ToList();
    }
}