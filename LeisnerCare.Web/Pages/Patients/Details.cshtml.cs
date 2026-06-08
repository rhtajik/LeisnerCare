using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeisnerCare.Web.Pages.Patients;

[Authorize(Roles = "Staff,Relative")]
public class DetailsModel : PageModel
{
    private readonly PatientService _patientService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DetailsModel(PatientService patientService, UserManager<ApplicationUser> userManager)
    {
        _patientService = patientService;
        _userManager = userManager;
    }

    public Patient Patient { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient == null) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);

        // Hvis brugeren er Patient, må de KUN se deres egen profil
        if (User.IsInRole("Patient"))
        {
            if (patient.UserId != currentUser?.Id)
            {
                return Forbid();
            }
        }

        // Hvis brugeren er Relative, må de KUN se deres tilknyttede patient
        if (User.IsInRole("Relative"))
        {
            if (patient.RelativeUserId != currentUser?.Id)
            {
                return Forbid();
            }
        }

        Patient = patient;
        return Page();
    }
}