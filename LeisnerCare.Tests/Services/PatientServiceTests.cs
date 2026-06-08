using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Core.Interfaces;
using Moq;
using Xunit;

namespace LeisnerCare.Tests.Services;

public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _mockRepo;
    private readonly PatientService _service;

    public PatientServiceTests()
    {
        _mockRepo = new Mock<IPatientRepository>();
        _service = new PatientService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListOfPatients()
    {
        // Arrange
        var patients = new List<Patient>
        {
            new Patient { Id = 1, CprNumber = "1234567890" },
            new Patient { Id = 2, CprNumber = "0987654321" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(patients);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("1234567890", result[0].CprNumber);
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsPatient()
    {
        // Arrange
        var patient = new Patient { Id = 1, CprNumber = "1234567890" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(patient);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("1234567890", result.CprNumber);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Patient?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CallsRepositoryAdd()
    {
        // Arrange
        var newPatient = new Patient { CprNumber = "1111111111" };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Patient>())).ReturnsAsync(newPatient);

        // Act
        var result = await _service.CreateAsync(newPatient);

        // Assert
        Assert.NotNull(result);
        _mockRepo.Verify(r => r.AddAsync(newPatient), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}