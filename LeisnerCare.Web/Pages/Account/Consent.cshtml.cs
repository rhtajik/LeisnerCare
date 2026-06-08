using LeisnerCare.Core.Entities;
using LeisnerCare.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeisnerCare.Web.Pages.Account;

[Authorize(Roles = "Patient")]
public class ConsentModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ConsentService _consentService;

    public ConsentModel(
        UserManager<ApplicationUser> userManager,
        ConsentService consentService)
    {
        _userManager = userManager;
        _consentService = consentService;
    }

    public bool HasConsent { get; set; }
    public string? ConsentDate { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            HasConsent = await _consentService.HasConsentAsync(user.Id);
            ConsentDate = await _consentService.GetConsentDateAsync(user.Id);
        }
    }

    public async Task<IActionResult> OnPostGiveConsentAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        await _consentService.GiveConsentAsync(user.Id);

        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostWithdrawConsentAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        await _consentService.WithdrawConsentAsync(user.Id);

        return RedirectToPage("/Index");
    }
}