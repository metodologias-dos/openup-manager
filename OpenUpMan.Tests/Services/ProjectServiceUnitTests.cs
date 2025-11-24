using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using OpenUpMan.Services;
using Xunit;

namespace OpenUpMan.Tests.Services
{
    public class ProjectServiceUnitTests
    {
        private static ILogger<ProjectService> CreateMockLogger()
        {
            return new Mock<ILogger<ProjectService>>().Object;
        }

        #region CreateProject Tests

        [Fact]
        public async Task CreateProject_ReturnsSuccess_WhenNewIdentifier()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync((Project?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Project>(), default)).Returns(Task.CompletedTask).Verifiable();
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask).Verifiable();

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());
            var ownerId = Guid.NewGuid();

            var result = await service.CreateProjectAsync("PROY-001", "Proyecto Test", DateTime.UtcNow, ownerId, "Descripción test");

            Assert.True(result.Success);
            Assert.Equal(ProjectServiceResultType.Success, result.ResultType);
            Assert.Contains("exitosa", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.Project);
            Assert.Equal("PROY-001", result.Project.Identifier);
            Assert.Equal("Proyecto Test", result.Project.Name);
            mockRepo.Verify(r => r.AddAsync(It.Is<Project>(p => p.Identifier == "PROY-001"), default), Times.Once);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task CreateProject_ReturnsError_WhenIdentifierExists()
        {
            var ownerId = Guid.NewGuid();
            var existing = new Project("PROY-001", "Existing Project", DateTime.UtcNow, ownerId);
            
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync(existing);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.CreateProjectAsync("PROY-001", "New Project", DateTime.UtcNow, ownerId);

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.AlreadyExists, result.ResultType);
            Assert.Contains("existe", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<Project>(), default), Times.Never);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Theory]
        [InlineData("", "Project Name", "Invalid identifier")]
        [InlineData("  ", "Project Name", "Invalid identifier")]
        [InlineData("PROY-001", "", "Invalid name")]
        [InlineData("PROY-001", "  ", "Invalid name")]
        public async Task CreateProject_ReturnsError_WhenParametersInvalid(string identifier, string name, string reason)
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            var service = new ProjectService(mockRepo.Object, CreateMockLogger());
            var ownerId = Guid.NewGuid();

            var result = await service.CreateProjectAsync(identifier, name, DateTime.UtcNow, ownerId);

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.Error, result.ResultType);
            Assert.Contains("vacío", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
        }

        [Fact]
        public async Task CreateProject_ReturnsError_WhenOwnerIdIsEmpty()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.CreateProjectAsync("PROY-001", "Project Name", DateTime.UtcNow, Guid.Empty);

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.Error, result.ResultType);
            Assert.Contains("propietario", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
        }

        [Fact]
        public async Task CreateProject_ReturnsError_WhenDatabaseThrowsException()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default))
                .ThrowsAsync(new Exception("Database connection failed"));

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());
            var ownerId = Guid.NewGuid();

            var result = await service.CreateProjectAsync("PROY-001", "Test Project", DateTime.UtcNow, ownerId);

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.Error, result.ResultType);
            Assert.Contains("Error", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
        }

        [Fact]
        public async Task CreateProject_SetsCorrectDefaultState()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync((Project?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Project>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());
            var ownerId = Guid.NewGuid();

            var result = await service.CreateProjectAsync("PROY-001", "Test Project", DateTime.UtcNow, ownerId);

            Assert.True(result.Success);
            Assert.NotNull(result.Project);
            Assert.Equal(ProjectState.CREATED, result.Project.State);
        }

        #endregion

        #region GetProjectByIdentifier Tests

        [Fact]
        public async Task GetProjectByIdentifier_ReturnsSuccess_WhenProjectExists()
        {
            var ownerId = Guid.NewGuid();
            var existingProject = new Project("PROY-001", "Test Project", DateTime.UtcNow, ownerId);

            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync(existingProject);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.GetProjectByIdentifierAsync("PROY-001");

            Assert.True(result.Success);
            Assert.Equal(ProjectServiceResultType.Success, result.ResultType);
            Assert.Contains("encontrado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.Project);
            Assert.Equal("PROY-001", result.Project.Identifier);
            mockRepo.Verify(r => r.GetByIdentifierAsync("PROY-001", default), Times.Once);
        }

        [Fact]
        public async Task GetProjectByIdentifier_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-999", default)).ReturnsAsync((Project?)null);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.GetProjectByIdentifierAsync("PROY-999");

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.NotFound, result.ResultType);
            Assert.Contains("encontró", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
            mockRepo.Verify(r => r.GetByIdentifierAsync("PROY-999", default), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public async Task GetProjectByIdentifier_ReturnsError_WhenIdentifierIsEmpty(string identifier)
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.GetProjectByIdentifierAsync(identifier);

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.Error, result.ResultType);
            Assert.Contains("vacío", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
        }

        [Fact]
        public async Task GetProjectByIdentifier_ReturnsError_WhenDatabaseThrowsException()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default))
                .ThrowsAsync(new Exception("Database connection failed"));

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.GetProjectByIdentifierAsync("PROY-001");

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.Error, result.ResultType);
            Assert.Contains("Error", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
        }

        #endregion

        #region GetProjectsByOwner Tests

        [Fact]
        public async Task GetProjectsByOwner_ReturnsSuccess_WhenProjectsExist()
        {
            var ownerId = Guid.NewGuid();
            var projects = new List<Project>
            {
                new Project("PROY-001", "Project 1", DateTime.UtcNow, ownerId),
                new Project("PROY-002", "Project 2", DateTime.UtcNow, ownerId)
            };

            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByOwnerAsync(ownerId, default)).ReturnsAsync(projects);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.GetProjectsByOwnerAsync(ownerId);

            Assert.True(result.Success);
            Assert.Equal(ProjectServiceResultType.Success, result.ResultType);
            Assert.Contains("exitosa", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.Projects);
            Assert.Equal(2, result.Projects.Count());
            mockRepo.Verify(r => r.GetByOwnerAsync(ownerId, default), Times.Once);
        }

        [Fact]
        public async Task GetProjectsByOwner_ReturnsSuccess_WhenNoProjectsExist()
        {
            var ownerId = Guid.NewGuid();
            var projects = new List<Project>();

            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByOwnerAsync(ownerId, default)).ReturnsAsync(projects);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.GetProjectsByOwnerAsync(ownerId);

            Assert.True(result.Success);
            Assert.Equal(ProjectServiceResultType.Success, result.ResultType);
            Assert.NotNull(result.Projects);
            Assert.Empty(result.Projects);
            mockRepo.Verify(r => r.GetByOwnerAsync(ownerId, default), Times.Once);
        }

        [Fact]
        public async Task GetProjectsByOwner_ReturnsError_WhenOwnerIdIsEmpty()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.GetProjectsByOwnerAsync(Guid.Empty);

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.Error, result.ResultType);
            Assert.Contains("válido", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Projects);
        }

        [Fact]
        public async Task GetProjectsByOwner_ReturnsError_WhenDatabaseThrowsException()
        {
            var ownerId = Guid.NewGuid();
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByOwnerAsync(ownerId, default))
                .ThrowsAsync(new Exception("Database connection failed"));

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.GetProjectsByOwnerAsync(ownerId);

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.Error, result.ResultType);
            Assert.Contains("Error", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Projects);
        }

        #endregion

        #region UpdateProject Tests

        [Fact]
        public async Task UpdateProject_ReturnsSuccess_WhenProjectExists()
        {
            var ownerId = Guid.NewGuid();
            var existingProject = new Project("PROY-001", "Old Name", DateTime.UtcNow, ownerId, "Old description");

            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync(existingProject);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask).Verifiable();

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());
            var newStartDate = DateTime.UtcNow.AddDays(10);

            var result = await service.UpdateProjectAsync("PROY-001", "New Name", "New description", newStartDate);

            Assert.True(result.Success);
            Assert.Equal(ProjectServiceResultType.Success, result.ResultType);
            Assert.Contains("actualizado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.Project);
            Assert.Equal("New Name", result.Project.Name);
            Assert.Equal("New description", result.Project.Description);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateProject_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-999", default)).ReturnsAsync((Project?)null);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.UpdateProjectAsync("PROY-999", "New Name", "New description", DateTime.UtcNow);

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.NotFound, result.ResultType);
            Assert.Contains("encontró", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Theory]
        [InlineData("", "New Name")]
        [InlineData("  ", "New Name")]
        [InlineData("PROY-001", "")]
        [InlineData("PROY-001", "  ")]
        public async Task UpdateProject_ReturnsError_WhenParametersInvalid(string identifier, string name)
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.UpdateProjectAsync(identifier, name, "Description", DateTime.UtcNow);

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.Error, result.ResultType);
            Assert.Contains("vacío", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
        }

        [Fact]
        public async Task UpdateProject_ReturnsError_WhenDatabaseThrowsException()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default))
                .ThrowsAsync(new Exception("Database connection failed"));

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.UpdateProjectAsync("PROY-001", "New Name", "Description", DateTime.UtcNow);

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.Error, result.ResultType);
            Assert.Contains("Error", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
        }

        #endregion

        #region ChangeProjectState Tests

        [Theory]
        [InlineData(ProjectState.ACTIVE)]
        [InlineData(ProjectState.ARCHIVED)]
        [InlineData(ProjectState.CLOSED)]
        public async Task ChangeProjectState_ReturnsSuccess_WhenProjectExists(ProjectState newState)
        {
            var ownerId = Guid.NewGuid();
            var existingProject = new Project("PROY-001", "Test Project", DateTime.UtcNow, ownerId);

            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync(existingProject);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask).Verifiable();

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.ChangeProjectStateAsync("PROY-001", newState);

            Assert.True(result.Success);
            Assert.Equal(ProjectServiceResultType.Success, result.ResultType);
            Assert.Contains("actualizado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.Project);
            Assert.Equal(newState, result.Project.State);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task ChangeProjectState_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-999", default)).ReturnsAsync((Project?)null);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.ChangeProjectStateAsync("PROY-999", ProjectState.ACTIVE);

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.NotFound, result.ResultType);
            Assert.Contains("encontró", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public async Task ChangeProjectState_ReturnsError_WhenIdentifierIsEmpty(string identifier)
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.ChangeProjectStateAsync(identifier, ProjectState.ACTIVE);

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.Error, result.ResultType);
            Assert.Contains("vacío", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
        }

        [Fact]
        public async Task ChangeProjectState_ReturnsError_WhenDatabaseThrowsException()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default))
                .ThrowsAsync(new Exception("Database connection failed"));

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            var result = await service.ChangeProjectStateAsync("PROY-001", ProjectState.ACTIVE);

            Assert.False(result.Success);
            Assert.Equal(ProjectServiceResultType.Error, result.ResultType);
            Assert.Contains("Error", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task CreateProject_AcceptsNullDescription()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync((Project?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Project>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());
            var ownerId = Guid.NewGuid();

            var result = await service.CreateProjectAsync("PROY-001", "Test Project", DateTime.UtcNow, ownerId, null);

            Assert.True(result.Success);
            Assert.NotNull(result.Project);
            Assert.Null(result.Project.Description);
        }

        [Fact]
        public async Task CreateProject_AcceptsEmptyDescription()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync((Project?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Project>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());
            var ownerId = Guid.NewGuid();

            var result = await service.CreateProjectAsync("PROY-001", "Test Project", DateTime.UtcNow, ownerId, "");

            Assert.True(result.Success);
            Assert.NotNull(result.Project);
        }

        [Theory]
        [InlineData("PROY-001")]
        [InlineData("PROJECT-123")]
        [InlineData("PRJ_001")]
        [InlineData("P-2024-001")]
        public async Task CreateProject_AcceptsVariousIdentifierFormats(string identifier)
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync(identifier, default)).ReturnsAsync((Project?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Project>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());
            var ownerId = Guid.NewGuid();

            var result = await service.CreateProjectAsync(identifier, "Test Project", DateTime.UtcNow, ownerId);

            Assert.True(result.Success);
            Assert.Equal(identifier, result.Project?.Identifier);
        }

        [Fact]
        public async Task CreateProject_HandlesFutureStartDate()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync((Project?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Project>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());
            var ownerId = Guid.NewGuid();
            var futureDate = DateTime.UtcNow.AddDays(30);

            var result = await service.CreateProjectAsync("PROY-001", "Future Project", futureDate, ownerId);

            Assert.True(result.Success);
            Assert.NotNull(result.Project);
            Assert.Equal(futureDate.Date, result.Project.StartDate.Date);
        }

        [Fact]
        public async Task CreateProject_HandlesPastStartDate()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync((Project?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Project>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());
            var ownerId = Guid.NewGuid();
            var pastDate = DateTime.UtcNow.AddDays(-30);

            var result = await service.CreateProjectAsync("PROY-001", "Past Project", pastDate, ownerId);

            Assert.True(result.Success);
            Assert.NotNull(result.Project);
            Assert.Equal(pastDate.Date, result.Project.StartDate.Date);
        }

        [Fact]
        public async Task UpdateProject_UpdatesUpdatedAtTimestamp()
        {
            var ownerId = Guid.NewGuid();
            var existingProject = new Project("PROY-001", "Old Name", DateTime.UtcNow, ownerId);
            var initialUpdatedAt = existingProject.UpdatedAt;

            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync(existingProject);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            // Wait a bit to ensure timestamp difference
            await Task.Delay(10);

            var result = await service.UpdateProjectAsync("PROY-001", "New Name", "New description", DateTime.UtcNow);

            Assert.True(result.Success);
            Assert.NotNull(result.Project);
            Assert.NotNull(result.Project.UpdatedAt);
            Assert.NotEqual(initialUpdatedAt, result.Project.UpdatedAt);
        }

        [Fact]
        public async Task ChangeProjectState_UpdatesUpdatedAtTimestamp()
        {
            var ownerId = Guid.NewGuid();
            var existingProject = new Project("PROY-001", "Test Project", DateTime.UtcNow, ownerId);
            var initialUpdatedAt = existingProject.UpdatedAt;

            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync(existingProject);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());

            // Wait a bit to ensure timestamp difference
            await Task.Delay(10);

            var result = await service.ChangeProjectStateAsync("PROY-001", ProjectState.ACTIVE);

            Assert.True(result.Success);
            Assert.NotNull(result.Project);
            Assert.NotNull(result.Project.UpdatedAt);
            Assert.NotEqual(initialUpdatedAt, result.Project.UpdatedAt);
        }

        [Theory]
        [InlineData("Project with special chars: @#$%")]
        [InlineData("Proyecto con ñ y acentos: José")]
        [InlineData("项目名称")] // Chinese characters
        [InlineData("プロジェクト")] // Japanese characters
        public async Task CreateProject_HandlesSpecialCharactersInName(string projectName)
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync((Project?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Project>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new ProjectService(mockRepo.Object, CreateMockLogger());
            var ownerId = Guid.NewGuid();

            var result = await service.CreateProjectAsync("PROY-001", projectName, DateTime.UtcNow, ownerId);

            Assert.True(result.Success);
            Assert.NotNull(result.Project);
            Assert.Equal(projectName, result.Project.Name);
        }

        #endregion
    }
}

