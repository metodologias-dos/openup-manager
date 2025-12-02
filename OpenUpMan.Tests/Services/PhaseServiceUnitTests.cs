using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using OpenUpMan.Services;

namespace OpenUpMan.Tests.Services;

public class PhaseServiceUnitTests
{
    private readonly Mock<IPhaseRepository> _phaseRepositoryMock;
    private readonly Mock<ILogger<PhaseService>> _loggerMock;
    private readonly PhaseService _phaseService;

    public PhaseServiceUnitTests()
    {
        _phaseRepositoryMock = new Mock<IPhaseRepository>();
        _loggerMock = new Mock<ILogger<PhaseService>>();
        _phaseService = new PhaseService(_phaseRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreatePhaseAsync_WithValidData_ShouldCreatePhase()
    {
        // Arrange
        var projectId = 1;
        var name = "Test Phase";
        var orderIndex = 1;

        // Act
        var result = await _phaseService.CreatePhaseAsync(projectId, name, orderIndex);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Fase creada exitosamente.", result.Message);
        Assert.NotNull(result.Phase);
        Assert.Equal(name, result.Phase.Name);
        _phaseRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Phase>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreatePhaseAsync_WithEmptyName_ShouldReturnError()
    {
        // Act
        var result = await _phaseService.CreatePhaseAsync(1, "", 1);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("El nombre de la fase es requerido.", result.Message);
        Assert.Null(result.Phase);
    }

    [Fact]
    public async Task GetPhaseByIdAsync_WithExistingPhase_ShouldReturnPhase()
    {
        // Arrange
        var phaseId = 1;
        var phase = new Phase(1, "Test Phase");
        _phaseRepositoryMock.Setup(repo => repo.GetByIdAsync(phaseId, It.IsAny<CancellationToken>())).ReturnsAsync(phase);

        // Act
        var result = await _phaseService.GetPhaseByIdAsync(phaseId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Fase encontrada.", result.Message);
        Assert.Equal(phase, result.Phase);
    }

    [Fact]
    public async Task GetPhaseByIdAsync_WithNonExistingPhase_ShouldReturnError()
    {
        // Arrange
        var phaseId = 1;
        _phaseRepositoryMock.Setup(repo => repo.GetByIdAsync(phaseId, It.IsAny<CancellationToken>())).ReturnsAsync((Phase)null!);

        // Act
        var result = await _phaseService.GetPhaseByIdAsync(phaseId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Fase no encontrada.", result.Message);
        Assert.Null(result.Phase);
    }
}
