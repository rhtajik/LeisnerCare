using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Web.Pages.Medications;

[Authorize(Roles = "Patient,Relative")]
public class TodayModel : PageModel
{
    private readonly MedicationService _medicationService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TodayModel(MedicationService medicationService, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _medicationService = medicationService;
        _context = context;
        _userManager = userManager;
    }

    public List<MedicationViewModel> Medications { get; set; } = new ();

    public class MedicationViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public bool IsTakenToday { get; set; }
    public DateTime? TakenAt { get; set; }
    public int? Effectiveness { get; set; }
    public int? LogId { get; set; }
    }

    public async Task OnGetAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return;

        Patient? patient = null;

        if (User.IsInRole("Patient"))
        {
            patient = _context.Patients.FirstOrDefault(p => p.UserId == currentUser.Id);
        }
        else if (User.IsInRole("Relative"))
        {
            patient = _context.Patients.FirstOrDefault(p => p.RelativeUserId == currentUser.Id);
        }

        if (patient != null)
        {
            var meds = await _medicationService.GetPatientMedicationsAsync(patient.Id);

            Medications = meds.Select(m =>
            {
                var todayLog = m.Logs.FirstOrDefault(l => l.TakenAt.Date == DateTime.UtcNow.Date);
                return new MedicationViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency,
                    IsTakenToday = todayLog != null,
                    TakenAt = todayLog?.TakenAt,
                    Effectiveness = todayLog?.Effectiveness,
                    LogId = todayLog?.Id
                };
            }).ToList();
        }
    }

    // ?? NY: Én OnPostAsync med handler parameter
    public async Task<IActionResult> OnPostAsync(int medicationId, int logId, int effectiveness, string handler)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return RedirectToPage();

        Patient? patient = null;

        if (User.IsInRole("Patient"))
        {
            patient = _context.Patients.FirstOrDefault(p => p.UserId == currentUser.Id);
        }
        else if (User.IsInRole("Relative"))
        {
            patient = _context.Patients.FirstOrDefault(p => p.RelativeUserId == currentUser.Id);
        }

        if (patient == null) return RedirectToPage();

        if (handler == "Take")
        {
            var log = new MedicationLog
            {
                MedicationId = medicationId,
                TakenAt = DateTime.UtcNow
            };

            await _medicationService.LogMedicationAsync(log);
            TempData["SuccessMessage"] = "Medicin markeret som taget!";
        }
        else if (handler == "Rate")
        {
            var log = await _context.MedicationLogs.FindAsync(logId);
            if (log != null)
            {
                log.Effectiveness = effectiveness;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Effektivitet gemt!";
            }
        }

        return RedirectToPage();
    }
}