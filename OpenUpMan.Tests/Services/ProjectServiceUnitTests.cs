﻿using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using OpenUpMan.Services;

namespace OpenUpMan.Tests.Services;

public class ProjectServiceUnitTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IPhaseRepository> _phaseRepositoryMock;
    private readonly Mock<IArtifactRepository> _artifactRepositoryMock;
    private readonly Mock<ILogger<ProjectService>> _loggerMock;
    private readonly ProjectService _projectService;

    public ProjectServiceUnitTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _phaseRepositoryMock = new Mock<IPhaseRepository>();
        _artifactRepositoryMock = new Mock<IArtifactRepository>();
        _loggerMock = new Mock<ILogger<ProjectService>>();
        _projectService = new ProjectService(_projectRepositoryMock.Object, _phaseRepositoryMock.Object, _artifactRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateProjectAsync_WithValidData_ShouldCreateProjectAndPhases()
    {
        // Arrange
        var projectName = "Test Project";
        var createdBy = 1;
        var projectCode = "TP001";

        _projectRepositoryMock.Setup(repo => repo.GetLastProjectCodeAsync(It.IsAny<CancellationToken>())).ReturnsAsync("TP000");
        _projectRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .Callback<Project, CancellationToken>((p, ct) => p.GetType().GetProperty("Id")!.SetValue(p, 1, null));


        // Act
        var result = await _projectService.CreateProjectAsync(projectName, createdBy, projectCode);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Proyecto creado exitosamente.", result.Message);
        Assert.NotNull(result.Project);
        Assert.Equal(projectName, result.Project.Name);
        Assert.Equal(projectCode, result.Project.Code);

        _projectRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
        _phaseRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Phase>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
        _projectRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(5)); // Once for project, 4 times for phases
    }

    [Fact]
    public async Task CreateProjectAsync_WithEmptyName_ShouldReturnError()
    {
        // Act
        var result = await _projectService.CreateProjectAsync("", 1);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("El nombre es requerido.", result.Message);
        Assert.Null(result.Project);
    }

    [Fact]
    public async Task CreateProjectAsync_WithoutCode_ShouldGenerateCode()
    {
        // Arrange
        var projectName = "Test Project";
        var createdBy = 1;
        _projectRepositoryMock.Setup(repo => repo.GetLastProjectCodeAsync(It.IsAny<CancellationToken>())).ReturnsAsync("TP000");
        _projectRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .Callback<Project, CancellationToken>((p, ct) => p.GetType().GetProperty("Id")!.SetValue(p, 1, null));
        _projectRepositoryMock.Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));


        // Act
        var result = await _projectService.CreateProjectAsync(projectName, createdBy);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Project);
        Assert.False(string.IsNullOrWhiteSpace(result.Project.Code));
    }

    [Fact]
    public async Task CreateProjectAsync_RepositoryThrowsException_ShouldReturnError()
    {
        // Arrange
        var projectName = "Test Project";
        var createdBy = 1;
        _projectRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _projectService.CreateProjectAsync(projectName, createdBy);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Error al crear el proyecto.", result.Message);
        Assert.Null(result.Project);
    }
}
