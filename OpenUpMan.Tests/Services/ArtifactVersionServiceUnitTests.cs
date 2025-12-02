using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using OpenUpMan.Services;

namespace OpenUpMan.Tests.Services;

public class ArtifactVersionServiceUnitTests
{
    private readonly Mock<IArtifactRepository> _artifactRepositoryMock;
    private readonly Mock<ILogger<ArtifactVersionService>> _loggerMock;
    private readonly ArtifactVersionService _artifactVersionService;

    public ArtifactVersionServiceUnitTests()
    {
        _artifactRepositoryMock = new Mock<IArtifactRepository>();
        _loggerMock = new Mock<ILogger<ArtifactVersionService>>();
        _artifactVersionService = new ArtifactVersionService(_artifactRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateVersionAsync_WithValidData_ShouldCreateVersion()
    {
        // Arrange
        var artifactId = 1;
        var createdBy = 1;
        var notes = "Test notes";
        var artifact = new Artifact(1, 1, "Test Artifact");

        _artifactRepositoryMock.Setup(repo => repo.GetByIdAsync(artifactId, It.IsAny<CancellationToken>())).ReturnsAsync(artifact);
        _artifactRepositoryMock.Setup(repo => repo.AddVersionAsync(It.IsAny<ArtifactVersion>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _artifactVersionService.CreateVersionAsync(artifactId, createdBy, notes);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Versión creada exitosamente.", result.Message);
        Assert.NotNull(result.ArtifactVersion);
        Assert.Equal(1, result.VersionNumber);
        _artifactRepositoryMock.Verify(repo => repo.AddVersionAsync(It.IsAny<ArtifactVersion>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateVersionAsync_WithNonExistingArtifact_ShouldReturnError()
    {
        // Arrange
        var artifactId = 1;
        _artifactRepositoryMock.Setup(repo => repo.GetByIdAsync(artifactId, It.IsAny<CancellationToken>())).ReturnsAsync((Artifact)null!);

        // Act
        var result = await _artifactVersionService.CreateVersionAsync(artifactId, 1);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Artefacto no encontrado.", result.Message);
        Assert.Null(result.ArtifactVersion);
    }

    [Fact]
    public async Task GetLatestVersionAsync_WithExistingVersions_ShouldReturnLatestVersion()
    {
        // Arrange
        var artifactId = 1;
        var latestVersion = new ArtifactVersion(artifactId, 1);
        typeof(ArtifactVersion).GetProperty(nameof(ArtifactVersion.VersionNumber))!.SetValue(latestVersion, 2, null);

        _artifactRepositoryMock.Setup(repo => repo.GetLatestVersionAsync(artifactId, It.IsAny<CancellationToken>())).ReturnsAsync(latestVersion);

        // Act
        var result = await _artifactVersionService.GetLatestVersionAsync(artifactId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Versión encontrada.", result.Message);
        Assert.NotNull(result.ArtifactVersion);
        Assert.Equal(2, result.VersionNumber);
    }

    [Fact]
    public async Task GetLatestVersionAsync_WithNoVersions_ShouldReturnError()
    {
        // Arrange
        var artifactId = 1;
        _artifactRepositoryMock.Setup(repo => repo.GetLatestVersionAsync(artifactId, It.IsAny<CancellationToken>())).ReturnsAsync((ArtifactVersion)null!);

        // Act
        var result = await _artifactVersionService.GetLatestVersionAsync(artifactId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("No se encontró ninguna versión del artefacto.", result.Message);
        Assert.Null(result.ArtifactVersion);
    }
}
