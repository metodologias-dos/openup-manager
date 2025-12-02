using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using OpenUpMan.Services;

namespace OpenUpMan.Tests.Services;

public class MicroincrementServiceUnitTests
{
    private readonly Mock<IMicroincrementRepository> _microincrementRepositoryMock;
    private readonly Mock<ILogger<MicroincrementService>> _loggerMock;
    private readonly MicroincrementService _microincrementService;

    public MicroincrementServiceUnitTests()
    {
        _microincrementRepositoryMock = new Mock<IMicroincrementRepository>();
        _loggerMock = new Mock<ILogger<MicroincrementService>>();
        _microincrementService = new MicroincrementService(_microincrementRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateMicroincrementAsync_WithValidData_ShouldCreateMicroincrement()
    {
        // Arrange
        var iterationId = 1;
        var title = "Test Microincrement";
        var authorId = 1;

        // Act
        var result = await _microincrementService.CreateMicroincrementAsync(iterationId, title, null, authorId, "functional");

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Microincremento creado exitosamente.", result.Message);
        Assert.NotNull(result.Microincrement);
        Assert.Equal(title, result.Microincrement.Title);
        _microincrementRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Microincrement>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateMicroincrementAsync_WithEmptyTitle_ShouldReturnError()
    {
        // Act
        var result = await _microincrementService.CreateMicroincrementAsync(1, "", null, 1, "functional");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("El título es requerido.", result.Message);
        Assert.Null(result.Microincrement);
    }

    [Fact]
    public async Task GetMicroincrementByIdAsync_WithExistingMicroincrement_ShouldReturnMicroincrement()
    {
        // Arrange
        var microincrementId = 1;
        var microincrement = new Microincrement(1, "Test", 1);
        _microincrementRepositoryMock.Setup(repo => repo.GetByIdAsync(microincrementId, It.IsAny<CancellationToken>())).ReturnsAsync(microincrement);

        // Act
        var result = await _microincrementService.GetMicroincrementByIdAsync(microincrementId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Microincremento encontrado.", result.Message);
        Assert.Equal(microincrement, result.Microincrement);
    }

    [Fact]
    public async Task GetMicroincrementByIdAsync_WithNonExistingMicroincrement_ShouldReturnError()
    {
        // Arrange
        var microincrementId = 1;
        _microincrementRepositoryMock.Setup(repo => repo.GetByIdAsync(microincrementId, It.IsAny<CancellationToken>())).ReturnsAsync((Microincrement)null!);

        // Act
        var result = await _microincrementService.GetMicroincrementByIdAsync(microincrementId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Microincremento no encontrado.", result.Message);
        Assert.Null(result.Microincrement);
    }
}
