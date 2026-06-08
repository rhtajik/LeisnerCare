using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LeisnerCare.Web.Pages.Reports;

[Authorize(Roles = "Staff,Patient,Relative")]
public class PatientReportModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly SymptomService _symptomService;
    private readonly MedicationService _medicationService;
    private readonly UserManager<ApplicationUser> _userManager;

    public PatientReportModel(
        ApplicationDbContext context,
        SymptomService symptomService,
        MedicationService medicationService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _symptomService = symptomService;
        _medicationService = medicationService;
        _userManager = userManager;
    }

    public List<SelectListItem> Patients { get; set; } = new ();

    [BindProperty(SupportsGet = true)]
public int? SelectedPatientId { get; set; }

public Patient? Patient { get; set; }
public List<Symptom> Symptoms { get; set; } = new();
public List< Medication > Medications { get; set; } = new();
public DateTime ReportDate { get; set; } = DateTime.UtcNow;
public bool IsStaff { get; set; } = false;

    public async Task OnGetAsync()
    {
        IsStaff = User.IsInRole("Staff");
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return; // eller RedirectToPage
        }

        if (IsStaff)
        {
            LoadPatients();
            if (SelectedPatientId.HasValue)
            {
                LoadReportData(SelectedPatientId.Value);
            }
        }
        else
        {
            Patient? patient = null;
            if (User.IsInRole("Patient"))
            {
                patient = _context.Patients.FirstOrDefault(p => p.UserId == currentUser.Id);
            }
            else if (User.IsInRole("Relative"))
            {
                patient = _context.Patients.FirstOrDefault(p => p.RelativeUserId == currentUser.Id);
            }

            if (patient != null)
            {
                SelectedPatientId = patient.Id;
                LoadReportData(patient.Id);
            }
        }
    }

    private void LoadPatients()
{
    Patients = _context.Patients
        .Include(p => p.User)
        .Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = $"{p.User!.FirstName} {p.User.LastName} ({p.CprNumber})"
        })
        .ToList();
}

private void LoadReportData(int patientId)
{
    Patient = _context.Patients
        .Include(p => p.User)
        .FirstOrDefault(p => p.Id == patientId);

    Symptoms = _context.Symptoms
        .Where(s => s.PatientId == patientId)
        .OrderByDescending(s => s.RecordedAt)
        .Take(30)
        .ToList();

    Medications = _context.Medications
        .Where(m => m.PatientId == patientId)
        .Include(m => m.Logs)
        .ToList();
}
}