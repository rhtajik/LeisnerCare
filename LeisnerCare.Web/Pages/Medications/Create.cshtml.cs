using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeisnerCare.Web.Pages.Medications;

[Authorize(Roles = "Staff")]
public class CreateModel : PageModel
{
    private readonly MedicationService _medicationService;
    private readonly ApplicationDbContext _context;

    public CreateModel(MedicationService medicationService, ApplicationDbContext context)
    {
        _medicationService = medicationService;
        _context = context;
    }

    [BindProperty]
    public MedicationInput Input { get; set; } = new();

    public List<SelectListItem> Patients { get; set; } = new();

    public class MedicationInput
    {
        public int PatientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public TimeSpan? TimeOfDay { get; set; }
    }

    public void OnGet()
    {
        Patients = _context.Patients
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.CprNumber
            })
            .ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var medication = new Medication
        {
            PatientId = Input.PatientId,
            Name = Input.Name,
            Dosage = Input.Dosage,
            Frequency = Input.Frequency,
            TimeOfDay = Input.TimeOfDay,
            StartDate = DateTime.UtcNow
        };

        await _medicationService.CreateAsync(medication);
        return RedirectToPage("/Staff/MedicationOverview");
    }
}