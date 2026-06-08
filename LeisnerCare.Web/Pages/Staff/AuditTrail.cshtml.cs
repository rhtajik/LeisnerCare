using LeisnerCare.Core.Interfaces;
using LeisnerCare.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeisnerCare.Web.Pages.Staff;

[Authorize(Roles = "Staff")]
public class AuditTrailModel : PageModel
{
    private readonly IAuditService _auditService;

    public AuditTrailModel(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public List<AuditLog> Logs { get; set; } = new();

    public async Task OnGetAsync()
    {
        Logs = await _auditService.GetAllAsync();
    }
}