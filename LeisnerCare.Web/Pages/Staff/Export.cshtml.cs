using LeisnerCare.Infrastructure.Data;
using LeisnerCare.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeisnerCare.Web.Pages.Staff;

[Authorize(Roles = "Staff")]
public class ExportModel : PageModel
{
    private readonly ExportService _exportService;
    private readonly PdfReportService _pdfService;  // NY
    private readonly ApplicationDbContext _context;

    public ExportModel(ExportService exportService, PdfReportService pdfService, ApplicationDbContext context)
    {
        _exportService = exportService;
        _pdfService = pdfService;  // NY
        _context = context;
    }

    public List<SelectListItem> Patients { get; set; } = new();

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

    // EKSISTERENDE CSV handlers
    public async Task<IActionResult> OnPostSymptomsAsync(int patientId)
    {
        var data = await _exportService.ExportSymptomsToCsvAsync(patientId);
        return File(data, "text/csv", $"symptomer_{patientId}_{DateTime.Now:yyyyMMdd}.csv");
    }

    public async Task<IActionResult> OnPostMedicationsAsync(int patientId)
    {
        var data = await _exportService.ExportMedicationsToCsvAsync(patientId);
        return File(data, "text/csv", $"medicin_{patientId}_{DateTime.Now:yyyyMMdd}.csv");
    }

    // NY PDF handler
    public async Task<IActionResult> OnPostPdfReportAsync(int patientId)
    {
        var pdf = await _pdfService.GeneratePatientReportAsync(patientId);
        return File(pdf, "application/pdf", $"ugeoversigt_{patientId}_{DateTime.Now:yyyyMMdd}.pdf");
    }
}