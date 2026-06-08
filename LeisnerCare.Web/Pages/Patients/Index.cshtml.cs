using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeisnerCare.Web.Pages.Patients;

[Authorize(Roles = "Staff")]
public class IndexModel : PageModel
{
    private readonly PatientService _patientService;

    public IndexModel(PatientService patientService)
    {
        _patientService = patientService;
    }

    public List<Patient> Patients { get; set; } = new();

    public async Task OnGetAsync()
    {
        Patients = await _patientService.GetAllAsync();
    }
}