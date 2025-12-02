using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using OpenUpMan.Services;

namespace OpenUpMan.Tests.Services;

public class IterationServiceUnitTests
{
    private readonly Mock<IIterationRepository> _iterationRepositoryMock;
    private readonly Mock<ILogger<IterationService>> _loggerMock;
    private readonly IterationService _iterationService;

    public IterationServiceUnitTests()
    {
        _iterationRepositoryMock = new Mock<IIterationRepository>();
        _loggerMock = new Mock<ILogger<IterationService>>();
        _iterationService = new IterationService(_iterationRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateIterationAsync_WithValidData_ShouldCreateIteration()
    {
        // Arrange
        var phaseId = 1;
        var name = "Test Iteration";
        var goal = "Test Goal";

        // Act
        var result = await _iterationService.CreateIterationAsync(phaseId, name, goal, null, null);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Iteración creada exitosamente.", result.Message);
        Assert.NotNull(result.Iteration);
        Assert.Equal(name, result.Iteration.Name);
        _iterationRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Iteration>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetIterationByIdAsync_WithExistingIteration_ShouldReturnIteration()
    {
        // Arrange
        var iterationId = 1;
        var iteration = new Iteration(1, "Test Iteration");
        _iterationRepositoryMock.Setup(repo => repo.GetByIdAsync(iterationId, It.IsAny<CancellationToken>())).ReturnsAsync(iteration);

        // Act
        var result = await _iterationService.GetIterationByIdAsync(iterationId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Iteración encontrada.", result.Message);
        Assert.Equal(iteration, result.Iteration);
    }

    [Fact]
    public async Task GetIterationByIdAsync_WithNonExistingIteration_ShouldReturnError()
    {
        // Arrange
        var iterationId = 1;
        _iterationRepositoryMock.Setup(repo => repo.GetByIdAsync(iterationId, It.IsAny<CancellationToken>())).ReturnsAsync((Iteration)null!);

        // Act
        var result = await _iterationService.GetIterationByIdAsync(iterationId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Iteración no encontrada.", result.Message);
        Assert.Null(result.Iteration);
    }

    [Fact]
    public async Task UpdateIterationAsync_WithValidData_ShouldUpdateIteration()
    {
        // Arrange
        var iterationId = 1;
        var iteration = new Iteration(1, "Old Name");
        var newName = "New Name";
        var newGoal = "New Goal";
        var newPercentage = 50;

        _iterationRepositoryMock.Setup(repo => repo.GetByIdAsync(iterationId, It.IsAny<CancellationToken>())).ReturnsAsync(iteration);

        // Act
        var result = await _iterationService.UpdateIterationAsync(iterationId, newName, newGoal, null, null, newPercentage);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Iteración actualizada exitosamente.", result.Message);
        Assert.NotNull(result.Iteration);
        Assert.Equal(newName, result.Iteration.Name);
        Assert.Equal(newGoal, result.Iteration.Goal);
        Assert.Equal(newPercentage, result.Iteration.CompletionPercentage);
        _iterationRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Iteration>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
