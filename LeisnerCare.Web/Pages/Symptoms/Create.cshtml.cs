using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeisnerCare.Web.Pages.Symptoms;

[Authorize(Roles = "Patient")]
public class CreateModel : PageModel
{
    private readonly SymptomService _symptomService;
    private readonly ApplicationDbContext _context;

    public CreateModel(SymptomService symptomService, ApplicationDbContext context)
    {
        _symptomService = symptomService;
        _context = context;
    }

    [BindProperty]
    public SymptomInput Input { get; set; } = new();

    public class SymptomInput
    {
        public SymptomType Type { get; set; }
        public int Value { get; set; }
        public string? Note { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        // Slå PatientId op direkte fra databasen via UserId
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var patient = _context.Patients.FirstOrDefault(p => p.UserId == userId);

        if (patient == null)
        {
            ModelState.AddModelError(string.Empty, "Patientprofil ikke fundet. Kontakt personale.");
            return Page();
        }

        var symptom = new Symptom
        {
            PatientId = patient.Id,
            Type = Input.Type,
            Value = Input.Value,
            Note = Input.Note
        };

        await _symptomService.CreateAsync(symptom);
        return RedirectToPage("./History");
    }
}