using LeisnerCare.Application.Services;
using LeisnerCare.Core.Entities;
using LeisnerCare.Core.Interfaces;
using Moq;
using Xunit;

namespace LeisnerCare.Tests.Services;

public class ObservationServiceTests
{
    private readonly Mock<IObservationRepository> _mockRepo;
    private readonly Mock<IAuditService> _mockAudit;
    private readonly ObservationService _service;

    public ObservationServiceTests()
    {
        _mockRepo = new Mock<IObservationRepository>();
        _mockAudit = new Mock<IAuditService>();
        _service = new ObservationService(_mockRepo.Object, _mockAudit.Object);
    }

    [Fact]
    public async Task CreateAsync_LogsAuditAndReturnsObservation()
    {
        // Arrange
        var observation = new Observation
        {
            PatientId = 1,
            AuthorId = "user-123",
            AuthorName = "Dr. Hansen",
            Content = "Patienten har rysten i højre hånd",
            IsClinical = true
        };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Observation>()))
                 .ReturnsAsync(new Observation { Id = 5, PatientId = 1, Content = "Patienten har rysten i højre hånd" });

        // Act
        var result = await _service.CreateAsync(observation);

        // Assert
        Assert.Equal(5, result.Id);
        _mockAudit.Verify(a => a.LogAsync(
            "Created",              // 1. action
            "Observation",          // 2. entityType
            5,                      // 3. entityId
            "user-123",             // 4. userId
            "Dr. Hansen",           // 5. userName
            1,                      // 6. patientId 
            "Klinisk: True"),       // 7. details
            Times.Once);
    }

    [Fact]
    public async Task GetByPatientAsync_ReturnsPatientObservations()
    {
        // Arrange
        var observations = new List<Observation>
        {
            new Observation { Id = 1, PatientId = 1, Content = "Note 1" },
            new Observation { Id = 2, PatientId = 1, Content = "Note 2" }
        };
        _mockRepo.Setup(r => r.GetByPatientIdAsync(1)).ReturnsAsync(observations);

        // Act
        var result = await _service.GetByPatientAsync(1);

        // Assert
        Assert.Equal(2, result.Count);
    }
}