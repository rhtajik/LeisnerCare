using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeisnerCare.Web.Pages.Observations;

[Authorize(Roles = "Patient,Relative,Staff")]
public class IndexModel : PageModel
{
    private readonly ObservationService _observationService;
    private readonly ApplicationDbContext _context;

    public IndexModel(ObservationService observationService, ApplicationDbContext context)
    {
        _observationService = observationService;
        _context = context;
    }

    public List<Observation> Observations { get; set; } = new();
    public bool CanCreate { get; set; } = false;
    public bool IsStaff { get; set; } = false;

    public async Task OnGetAsync()
    {
        IsStaff = User.IsInRole("Staff");
        CanCreate = !User.IsInRole("Patient"); // Patienter læser kun

        if (User.IsInRole("Staff"))
        {
            Observations = await _observationService.GetAllAsync();
        }
        else
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var patient = _context.Patients.FirstOrDefault(p => p.UserId == userId);
            if (patient != null)
            {
                Observations = await _observationService.GetByPatientAsync(patient.Id);
            }
        }
    }
}