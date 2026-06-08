using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Web.Pages.Medications;

[Authorize(Roles = "Patient,Relative,Staff")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<Core.Entities.Medication> Medications { get; set; } = new ();

    public async Task OnGetAsync()
{
    var currentUser = await _userManager.GetUserAsync(User);
    if (currentUser == null) return;

    Core.Entities.Patient? patient = null;

    if (User.IsInRole("Patient"))
    {
        patient = _context.Patients.FirstOrDefault(p => p.UserId == currentUser.Id);
    }
    else if (User.IsInRole("Relative"))
    {
        // ?? NY: Relative finder patient via RelativeUserId
        patient = _context.Patients.FirstOrDefault(p => p.RelativeUserId == currentUser.Id);
    }
    else if (User.IsInRole("Staff"))
    {
        // Staff ser alle medicin (eller specifik patient hvis valgt)
        Medications = await _context.Medications
            .Include(m => m.Logs)
            .ToListAsync();
        return;
    }

    if (patient != null)
    {
        Medications = await _context.Medications
            .Where(m => m.PatientId == patient.Id)
            .Include(m => m.Logs)
            .ToListAsync();
    }
}
}