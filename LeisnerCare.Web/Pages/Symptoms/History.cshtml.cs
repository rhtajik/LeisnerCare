using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeisnerCare.Web.Pages.Symptoms;

[Authorize(Roles = "Patient,Staff")]
public class HistoryModel : PageModel
{
    private readonly SymptomService _symptomService;
    private readonly ApplicationDbContext _context;

    public HistoryModel(SymptomService symptomService, ApplicationDbContext context)
    {
        _symptomService = symptomService;
        _context = context;
    }

    public List<Symptom> Symptoms { get; set; } = new();

    public async Task OnGetAsync()
    {
        if (User.IsInRole("Staff"))
        {
            Symptoms = await _symptomService.GetAllAsync();
        }
        else
        {
            // Slå PatientId op fra databasen
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var patient = _context.Patients.FirstOrDefault(p => p.UserId == userId);

            if (patient != null)
            {
                Symptoms = await _symptomService.GetPatientHistoryAsync(patient.Id);
            }
        }
    }
}