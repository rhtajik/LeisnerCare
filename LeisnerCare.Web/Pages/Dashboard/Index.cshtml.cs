using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LeisnerCare.Web.Pages.Dashboard;

[Authorize]
public class IndexModel : PageModel
{
    private readonly PatientService _patientService;
    private readonly SymptomService _symptomService;
    private readonly ObservationService _observationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public IndexModel(
        PatientService patientService,
        SymptomService symptomService,
        ObservationService observationService,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _patientService = patientService;
        _symptomService = symptomService;
        _observationService = observationService;
        _userManager = userManager;
        _context = context;
    }

    public ApplicationUser? CurrentUser { get; set; }
    public Patient? PatientProfile { get; set; }
    public List<Symptom> RecentSymptoms { get; set; } = new();
    public List<Observation> RecentObservations { get; set; } = new ();
    public List<Patient> AllPatients { get; set; } = new();         
public List< Patient > FollowedPatients { get; set; } = new();      
public string? PatientName { get; set; }
public string? PatientCpr { get; set; }
public DateTime? PatientBirthDate { get; set; }

public List<MedicationViewModel> RecentMedications { get; set; } = new();

public class MedicationViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsTakenToday { get; set; }
    }

    public bool IsStaff => User.IsInRole("Staff");
    public bool IsPatient => User.IsInRole("Patient");
    public bool IsRelative => User.IsInRole("Relative");

    public async Task OnGetAsync()
{
    CurrentUser = await _userManager.GetUserAsync(User);

    if (IsPatient && CurrentUser != null)
    {
        var allPatients = await _patientService.GetAllAsync();
        PatientProfile = allPatients.FirstOrDefault(p => p.UserId == CurrentUser.Id);

        if (PatientProfile != null)
        {
            RecentSymptoms = await _symptomService.GetPatientHistoryAsync(PatientProfile.Id);
            RecentObservations = await _observationService.GetByPatientAsync(PatientProfile.Id);

            var medications = await _context.Medications
                .Where(m => m.PatientId == PatientProfile.Id)
                .Include(m => m.Logs)
                .ToListAsync();

            RecentMedications = medications.Select(m => new MedicationViewModel
            {
                Id = m.Id,
                Name = m.Name,
                IsTakenToday = m.Logs.Any(l => l.TakenAt.Date == DateTime.UtcNow.Date)
            }).ToList();
        }
    }
    else if (IsStaff)
    {
        AllPatients = await _patientService.GetAllAsync();
    }
    else if (IsRelative && CurrentUser != null)
    {
        var patient = await _context.Patients
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.RelativeUserId == CurrentUser.Id);

        if (patient != null)
        {
            FollowedPatients = new List< Patient > { patient }
            ;  
            PatientName = $"{patient.User?.FirstName} {patient.User?.LastName}";
            PatientCpr = patient.CprNumber;
            PatientBirthDate = patient.DateOfBirth;
        }
    }
}
}