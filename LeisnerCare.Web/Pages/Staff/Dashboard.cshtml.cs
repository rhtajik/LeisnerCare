using LeisnerCare.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LeisnerCare.Web.Services;

namespace LeisnerCare.Web.Pages.Staff;

[Authorize(Roles = "Staff")]
public class DashboardModel : PageModel
{
    private readonly DashboardService _dashboardService;

    public DashboardModel(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public List<PatientAlert> Alerts { get; set; } = new();

    public async Task OnGetAsync()
    {
        Alerts = await _dashboardService.GetRedFlagsAsync();
    }
}