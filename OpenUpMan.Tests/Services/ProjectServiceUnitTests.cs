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

        private static IUserRepository CreateMockUserRepository()
        {
            return new Mock<IUserRepository>().Object;
        }

        #region CreateProject Tests

        [Fact]
        public async Task CreateProject_ReturnsSuccess_WhenNewIdentifier()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync((Project?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Project>(), default)).Returns(Task.CompletedTask).Verifiable();
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask).Verifiable();

            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());
            var ownerId = Guid.NewGuid();

            var result = await service.CreateProjectAsync("PROY-001", "Proyecto Test", DateTime.UtcNow, ownerId, "Descripción test");

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
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

            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());

            var result = await service.CreateProjectAsync("PROY-001", "New Project", DateTime.UtcNow, ownerId);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("existe", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<Project>(), default), Times.Never);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Theory]
        [InlineData("", "Project Name")]
        [InlineData("  ", "Project Name")]
        [InlineData("PROY-001", "")]
        [InlineData("PROY-001", "  ")]
        public async Task CreateProject_ReturnsError_WhenParametersInvalid(string identifier, string name)
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());
            var ownerId = Guid.NewGuid();

            var result = await service.CreateProjectAsync(identifier, name, DateTime.UtcNow, ownerId);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("requerido", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
        }

        [Fact]
        public async Task CreateProject_SetsCorrectDefaultState()
        {
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdentifierAsync("PROY-001", default)).ReturnsAsync((Project?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Project>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());
            var ownerId = Guid.NewGuid();

            var result = await service.CreateProjectAsync("PROY-001", "Test Project", DateTime.UtcNow, ownerId);

            Assert.True(result.Success);
            Assert.NotNull(result.Project);
            Assert.Equal(ProjectState.CREATED, result.Project.State);
        }

        #endregion

        #region GetProjectById Tests

        [Fact]
        public async Task GetProjectById_ReturnsSuccess_WhenProjectExists()
        {
            var ownerId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var existingProject = new Project("PROY-001", "Test Project", DateTime.UtcNow, ownerId);

            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, default)).ReturnsAsync(existingProject);

            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());

            var result = await service.GetProjectByIdAsync(projectId);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("encontrado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.Project);
            mockRepo.Verify(r => r.GetByIdAsync(projectId, default), Times.Once);
        }

        [Fact]
        public async Task GetProjectById_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            var projectId = Guid.NewGuid();
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, default)).ReturnsAsync((Project?)null);

            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());

            var result = await service.GetProjectByIdAsync(projectId);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("encontrado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
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

            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());

            var result = await service.GetProjectByIdentifierAsync("PROY-001");

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
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

            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());

            var result = await service.GetProjectByIdentifierAsync("PROY-999");

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("encontrado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
            mockRepo.Verify(r => r.GetByIdentifierAsync("PROY-999", default), Times.Once);
        }

        #endregion

        #region GetProjectsByOwner Tests

        [Fact]
        public async Task GetProjectsByOwner_ReturnsProjects_WhenProjectsExist()
        {
            var ownerId = Guid.NewGuid();
            var projects = new List<Project>
            {
                new Project("PROY-001", "Project 1", DateTime.UtcNow, ownerId),
                new Project("PROY-002", "Project 2", DateTime.UtcNow, ownerId)
            };

            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByOwnerAsync(ownerId, default)).ReturnsAsync(projects);

            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());

            var result = await service.GetProjectsByOwnerAsync(ownerId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            mockRepo.Verify(r => r.GetByOwnerAsync(ownerId, default), Times.Once);
        }

        [Fact]
        public async Task GetProjectsByOwner_ReturnsEmpty_WhenNoProjectsExist()
        {
            var ownerId = Guid.NewGuid();
            var projects = new List<Project>();

            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByOwnerAsync(ownerId, default)).ReturnsAsync(projects);

            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());

            var result = await service.GetProjectsByOwnerAsync(ownerId);

            Assert.NotNull(result);
            Assert.Empty(result);
            mockRepo.Verify(r => r.GetByOwnerAsync(ownerId, default), Times.Once);
        }

        #endregion

        #region UpdateProject Tests

        [Fact]
        public async Task UpdateProject_ReturnsSuccess_WhenProjectExists()
        {
            var ownerId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var existingProject = new Project("PROY-001", "Old Name", DateTime.UtcNow, ownerId, "Old description");

            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, default)).ReturnsAsync(existingProject);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Project>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask).Verifiable();

            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());
            var newStartDate = DateTime.UtcNow.AddDays(10);

            var result = await service.UpdateProjectAsync(projectId, "New Name", "New description", newStartDate);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("actualizado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.Project);
            Assert.Equal("New Name", result.Project.Name);
            Assert.Equal("New description", result.Project.Description);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateProject_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            var projectId = Guid.NewGuid();
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, default)).ReturnsAsync((Project?)null);

            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());

            var result = await service.UpdateProjectAsync(projectId, "New Name", "New description", DateTime.UtcNow);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("encontrado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
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
            var projectId = Guid.NewGuid();
            var existingProject = new Project("PROY-001", "Test Project", DateTime.UtcNow, ownerId);

            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, default)).ReturnsAsync(existingProject);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Project>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask).Verifiable();

            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());

            var result = await service.ChangeProjectStateAsync(projectId, newState);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("actualizado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.Project);
            Assert.Equal(newState, result.Project.State);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task ChangeProjectState_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            var projectId = Guid.NewGuid();
            var mockRepo = new Mock<IProjectRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, default)).ReturnsAsync((Project?)null);

            var service = new ProjectService(mockRepo.Object, CreateMockUserRepository(), CreateMockLogger());

            var result = await service.ChangeProjectStateAsync(projectId, ProjectState.ACTIVE);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("encontrado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.Project);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        #endregion
    }
}

