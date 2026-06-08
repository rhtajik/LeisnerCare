using LeisnerCare.Infrastructure.Data;
using LeisnerCare.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeisnerCare.Web.Pages.Reports;

[Authorize(Roles = "Patient,Staff")]
public class WeeklyModel : PageModel
{
    private readonly AnalysisService _analysisService;
    private readonly ApplicationDbContext _context;

    public WeeklyModel(AnalysisService analysisService, ApplicationDbContext context)
    {
        _analysisService = analysisService;
        _context = context;
    }

    public WeeklyInsight? Insight { get; set; }

    public async Task OnGetAsync()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var patient = _context.Patients.FirstOrDefault(p => p.UserId == userId);

        if (patient != null)
        {
            Insight = await _analysisService.GetWeeklyInsightAsync(patient.Id);
        }
    }
}