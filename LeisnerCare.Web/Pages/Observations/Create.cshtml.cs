using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Core.Interfaces;
using LeisnerCare.Infrastructure.Data;
using LeisnerCare.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LeisnerCare.Web.Pages.Observations;

[Authorize(Roles = "Relative,Staff")]
public class CreateModel : PageModel
{
    private readonly ObservationService _observationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public CreateModel(
        ObservationService observationService,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IAuditService auditService)
    {
        _observationService = observationService;
        _userManager = userManager;
        _context = context;
        _auditService = auditService;
    }

    [BindProperty]
    public ObservationInput Input { get; set; } = new();

    public List<SelectListItem> Patients { get; set; } = new ();
    public bool IsStaff { get; set; } = false;
public bool IsRelative { get; set; } = false;
public int? FixedPatientId { get; set; } // ?? NY: Relative har kun én patient

public class ObservationInput
{
    public int PatientId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsClinical { get; set; } = false;
    }

    public async Task OnGetAsync()
    {
        IsStaff = User.IsInRole("Staff");
        IsRelative = User.IsInRole("Relative");
        await LoadPatientsAsync();
    }

    private async Task LoadPatientsAsync()
    {
        if (IsRelative)
        {
            // Relative ser KUN deres egen tilknyttede patient
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return;

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.RelativeUserId == currentUser.Id);

            if (patient != null)
            {
                Patients = new List< SelectListItem >
                {
                    new SelectListItem
                    {
                        Value = patient.Id.ToString(),
                        Text = patient.CprNumber
                    }
                }
                ;
                FixedPatientId = patient.Id;
                Input.PatientId = patient.Id; // Forudfyld
            }
        }
        else
        {
            // Staff ser alle
            Patients = await _context.Patients
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.CprNumber
                })
                .ToListAsync();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            IsStaff = User.IsInRole("Staff");
            IsRelative = User.IsInRole("Relative");
            await LoadPatientsAsync();
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage();

        // ?? SIKKERHEDSTJEK: Relative må KUN skrive til deres egen patient
        if (User.IsInRole("Relative"))
        {
            var relativePatient = await _context.Patients
                .FirstOrDefaultAsync(p => p.RelativeUserId == user.Id);

            if (relativePatient == null || Input.PatientId != relativePatient.Id)
            {
                return Forbid();
            }
        }

        var observation = new Observation
        {
            PatientId = Input.PatientId,
            AuthorId = user.Id,
            AuthorName = $"{user.FirstName} {user.LastName}",
            AuthorRole = user.Role.ToString(),
            Content = Input.Content,
            IsClinical = IsStaff && Input.IsClinical
        };

        await _observationService.CreateAsync(observation);

        await _auditService.LogAsync(
            "Created",
            "Observation",
            observation.Id,
            user.Id,
            user.UserName ?? "Unknown",
            observation.PatientId,
            $"Oprettet observation: {observation.Content?.Substring(0, Math.Min(50, observation.Content?.Length ?? 0))}"
        );

        TempData["SuccessMessage"] = "Observation gemt succesfuldt!";
        return RedirectToPage("./Index");
    }
}