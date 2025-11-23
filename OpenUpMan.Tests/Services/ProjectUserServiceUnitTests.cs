using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using OpenUpMan.Services;
using Xunit;

namespace OpenUpMan.Tests.Services
{
    public class ProjectUserServiceUnitTests
    {
        private static ILogger<ProjectUserService> CreateMockLogger()
        {
            return new Mock<ILogger<ProjectUserService>>().Object;
        }

        #region AddUserToProject Tests

        [Fact]
        public async Task AddUserToProject_ReturnsSuccess_WhenUserNotInProject()
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync((ProjectUser?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<ProjectUser>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;
            var mockUserRepo = new Mock<IUserRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockProjectRepo, mockUserRepo, CreateMockLogger());

            var result = await service.AddUserToProjectAsync(projectId, userId, ProjectUserPermission.VIEWER, ProjectUserRole.AUTOR);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("exitosa", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.ProjectUser);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<ProjectUser>(), default), Times.Once);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task AddUserToProject_ReturnsError_WhenUserAlreadyInProject()
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingPU = new ProjectUser(projectId, userId, ProjectUserPermission.VIEWER, ProjectUserRole.AUTOR);

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync(existingPU);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;
            var mockUserRepo = new Mock<IUserRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockProjectRepo, mockUserRepo, CreateMockLogger());

            var result = await service.AddUserToProjectAsync(projectId, userId);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("asignado", result.Message, StringComparison.OrdinalIgnoreCase);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<ProjectUser>(), default), Times.Never);
        }

        #endregion

        #region RemoveUserFromProject Tests

        [Fact]
        public async Task RemoveUserFromProject_ReturnsSuccess_WhenUserIsInProject()
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingPU = new ProjectUser(projectId, userId, ProjectUserPermission.VIEWER, ProjectUserRole.AUTOR);

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync(existingPU);
            mockRepo.Setup(r => r.RemoveAsync(existingPU, default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;
            var mockUserRepo = new Mock<IUserRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockProjectRepo, mockUserRepo, CreateMockLogger());

            var result = await service.RemoveUserFromProjectAsync(projectId, userId);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("exitosa", result.Message, StringComparison.OrdinalIgnoreCase);
            mockRepo.Verify(r => r.RemoveAsync(existingPU, default), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromProject_ReturnsError_WhenUserNotInProject()
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync((ProjectUser?)null);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;
            var mockUserRepo = new Mock<IUserRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockProjectRepo, mockUserRepo, CreateMockLogger());

            var result = await service.RemoveUserFromProjectAsync(projectId, userId);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("asignado", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region ChangeUserRole Tests

        [Theory]
        [InlineData(ProjectUserRole.AUTOR)]
        [InlineData(ProjectUserRole.REVISOR)]
        [InlineData(ProjectUserRole.PO)]
        [InlineData(ProjectUserRole.SM)]
        [InlineData(ProjectUserRole.DESARROLLADOR)]
        [InlineData(ProjectUserRole.TESTER)]
        [InlineData(ProjectUserRole.ADMIN)]
        public async Task ChangeUserRole_ReturnsSuccess_WhenUserIsInProject(ProjectUserRole newRole)
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingPU = new ProjectUser(projectId, userId, ProjectUserPermission.VIEWER, ProjectUserRole.AUTOR);

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync(existingPU);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;
            var mockUserRepo = new Mock<IUserRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockProjectRepo, mockUserRepo, CreateMockLogger());

            var result = await service.ChangeUserRoleAsync(projectId, userId, newRole);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("actualizado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.ProjectUser);
            Assert.Equal(newRole, result.ProjectUser.Role);
        }

        [Fact]
        public async Task ChangeUserRole_ReturnsError_WhenUserNotInProject()
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync((ProjectUser?)null);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;
            var mockUserRepo = new Mock<IUserRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockProjectRepo, mockUserRepo, CreateMockLogger());

            var result = await service.ChangeUserRoleAsync(projectId, userId, ProjectUserRole.ADMIN);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("asignado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.ProjectUser);
        }

        #endregion

        #region ChangeUserPermissions Tests

        [Theory]
        [InlineData(ProjectUserPermission.VIEWER)]
        [InlineData(ProjectUserPermission.EDITOR)]
        [InlineData(ProjectUserPermission.OWNER)]
        public async Task ChangeUserPermissions_ReturnsSuccess_WhenUserIsInProject(ProjectUserPermission newPermission)
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingPU = new ProjectUser(projectId, userId, ProjectUserPermission.VIEWER, ProjectUserRole.AUTOR);

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync(existingPU);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;
            var mockUserRepo = new Mock<IUserRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockProjectRepo, mockUserRepo, CreateMockLogger());

            var result = await service.ChangeUserPermissionsAsync(projectId, userId, newPermission);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("actualizado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.ProjectUser);
            Assert.Equal(newPermission, result.ProjectUser.Permissions);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task ChangeUserPermissions_ReturnsError_WhenUserNotInProject()
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync((ProjectUser?)null);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;
            var mockUserRepo = new Mock<IUserRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockProjectRepo, mockUserRepo, CreateMockLogger());

            var result = await service.ChangeUserPermissionsAsync(projectId, userId, ProjectUserPermission.OWNER);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("asignado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.ProjectUser);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        #endregion

        #region GetProjectUsers Tests

        [Fact]
        public async Task GetProjectUsers_ReturnsUsers_WhenUsersExist()
        {
            var projectId = Guid.NewGuid();
            var users = new List<ProjectUser>
            {
                new ProjectUser(projectId, Guid.NewGuid(), ProjectUserPermission.VIEWER, ProjectUserRole.AUTOR),
                new ProjectUser(projectId, Guid.NewGuid(), ProjectUserPermission.EDITOR, ProjectUserRole.DESARROLLADOR)
            };

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByProjectIdAsync(projectId, default)).ReturnsAsync(users);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;
            var mockUserRepo = new Mock<IUserRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockProjectRepo, mockUserRepo, CreateMockLogger());

            var result = await service.GetProjectUsersAsync(projectId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        #endregion
    }
}

