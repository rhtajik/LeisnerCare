using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeisnerCare.Web.Pages.Patients;

[Authorize(Roles = "Staff")]
public class EditModel : PageModel
{
    private readonly PatientService _patientService;
    private readonly UserManager<ApplicationUser> _userManager;

    public EditModel(PatientService patientService, UserManager<ApplicationUser> userManager)
    {
        _patientService = patientService;
        _userManager = userManager;
    }

    [BindProperty]
    public Patient Patient { get; set; } = null!;

    public List<SelectListItem> RelativeOptions { get; set; } = new ();

    public async Task<IActionResult> OnGetAsync(int id)
{
    var patient = await _patientService.GetByIdAsync(id);
    if (patient == null) return NotFound();

    Patient = patient;
    await LoadRelativesAsync();
    return Page();
}

public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
    {
        await LoadRelativesAsync();
        return Page();
    }

    var existing = await _patientService.GetByIdAsync(Patient.Id);
    if (existing == null) return NotFound();

    // Opdater kun tilladte felter
    existing.CprNumber = Patient.CprNumber;
    existing.DateOfBirth = Patient.DateOfBirth;
    existing.DiagnosisDate = Patient.DiagnosisDate;
    existing.ContactPhone = Patient.ContactPhone;
    existing.EmergencyContactName = Patient.EmergencyContactName;
    existing.EmergencyContactPhone = Patient.EmergencyContactPhone;
    existing.RelativeUserId = Patient.RelativeUserId;

    await _patientService.UpdateAsync(existing);

    return RedirectToPage("./Index");
}

private async Task LoadRelativesAsync()
{
    var relatives = await _userManager.GetUsersInRoleAsync("Relative");
    RelativeOptions = relatives
        .Select(r => new SelectListItem
        {
            Value = r.Id,
            Text = $"{r.FirstName} {r.LastName} ({r.Email})"
        })
        .ToList();

    // Tilføj "Ingen" øverst
    RelativeOptions.Insert(0, new SelectListItem
    {
        Value = "",
        Text = "-- Ingen pårørende --"
    });
}
}