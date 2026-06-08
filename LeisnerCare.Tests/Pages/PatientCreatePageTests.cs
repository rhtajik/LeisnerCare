using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Core.Interfaces;
using LeisnerCare.Web.Pages.Patients;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace LeisnerCare.Tests.Pages;

public class PatientCreatePageTests
{
    private readonly Mock<IPatientRepository> _mockRepo;
    private readonly PatientService _patientService;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly CreateModel _pageModel;

    public PatientCreatePageTests()
    {
        // Brug rigtig PatientService med mock'et repository
        _mockRepo = new Mock<IPatientRepository>();
        _patientService = new PatientService(_mockRepo.Object);

        // UserManager mock
        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _pageModel = new CreateModel(_patientService, _mockUserManager.Object);
    }

    [Fact]
    public void OnGet_DoesNotThrow()
    {
        // Act
        _pageModel.OnGet();

        // Assert
        Assert.NotNull(_pageModel.Input);
    }

    [Fact]
    public async Task OnPostAsync_InvalidModel_ReturnsPage()
    {
        // Arrange
        _pageModel.ModelState.AddModelError("Input.CprNumber", "CPR er påkrævet");

        // Act
        var result = await _pageModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.False(_pageModel.ModelState.IsValid);
    }

    [Fact]
    public async Task OnPostAsync_ValidInput_CreatesUserPatientAndRedirects()
    {
        // Arrange
        _pageModel.Input = new CreateModel.PatientInput
        {
            Email = "test@example.com",
            Password = "Test123!",
            FirstName = "Hans",
            LastName = "Hansen",
            CprNumber = "1234567890",
            DateOfBirth = new DateTime(1960, 5, 15),
            DiagnosisDate = new DateTime(2020, 3, 10),
            ContactPhone = "12345678",
            ConsentGiven = true
        };

        _mockUserManager
            .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Patient"))
            .ReturnsAsync(IdentityResult.Success);

        _mockRepo
            .Setup(r => r.AddAsync(It.IsAny<Patient>()))
            .ReturnsAsync(new Patient { Id = 1, CprNumber = "1234567890" });

        // Act
        var result = await _pageModel.OnPostAsync();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", redirectResult.PageName);

        _mockUserManager.Verify(u => u.CreateAsync(
            It.Is<ApplicationUser>(user => user.Email == "test@example.com"),
            "Test123!"), Times.Once);

        _mockUserManager.Verify(u => u.AddToRoleAsync(
            It.IsAny<ApplicationUser>(), "Patient"), Times.Once);

        _mockRepo.Verify(r => r.AddAsync(
            It.Is<Patient>(p => p.CprNumber == "1234567890" && p.ConsentGiven == true)), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_UserCreationFails_ReturnsPageWithErrors()
    {
        // Arrange
        _pageModel.Input = new CreateModel.PatientInput
        {
            Email = "test@example.com",
            Password = "weak",
            FirstName = "Hans",
            LastName = "Hansen",
            CprNumber = "1234567890",
            DateOfBirth = new DateTime(1960, 5, 15),
            DiagnosisDate = new DateTime(2020, 3, 10)
        };

        var errors = new[] { new IdentityError { Description = "Password too weak" } };
        _mockUserManager
            .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        var result = await _pageModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Contains(_pageModel.ModelState, m => m.Value!.Errors.Any());
    }
}