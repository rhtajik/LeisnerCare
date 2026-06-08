using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Core.Interfaces;
using LeisnerCare.Infrastructure.Data;
using LeisnerCare.Infrastructure.Repositories;
using LeisnerCare.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LeisnerCare.Web.Services;
using QuestPDF.Infrastructure;


QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);


// ========== SERVICES (før Build!) ==========
builder.Services.AddRazorPages();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.ReturnUrlParameter = "returnUrl";

});

// Repository + Service 
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<PatientService>();

builder.Services.AddScoped<ISymptomRepository, SymptomRepository>();
builder.Services.AddScoped<SymptomService>();

builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
builder.Services.AddScoped<MedicationService>();

builder.Services.AddScoped<IObservationRepository, ObservationRepository>();
builder.Services.AddScoped<ObservationService>();


builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<DashboardService>();

builder.Services.AddScoped<AnalysisService>();

builder.Services.AddScoped<ExportService>();

builder.Services.AddScoped<PdfReportService>();

builder.Services.AddScoped<ConsentService>();

// ============================================

var app = builder.Build();

// HTTP pipeline (efter Build!)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// Seed roller
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRoles(services);
}

app.Run();

static async Task SeedRoles(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Patient", "Relative", "Staff" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}