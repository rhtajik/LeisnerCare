using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeisnerCare.Web.Pages.Staff;

[Authorize(Roles = "Staff")]
public class SymptomOverviewModel : PageModel
{
    private readonly SymptomService _symptomService;

    public SymptomOverviewModel(SymptomService symptomService)
    {
        _symptomService = symptomService;
    }

    public List<Symptom> Symptoms { get; set; } = new();

    public async Task OnGetAsync()
    {
        Symptoms = await _symptomService.GetAllAsync();
    }
}