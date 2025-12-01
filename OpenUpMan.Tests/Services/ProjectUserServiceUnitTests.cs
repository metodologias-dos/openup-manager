using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Data.Repositories;
using OpenUpMan.Domain;
using OpenUpMan.Services;

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
            var roleId = Guid.NewGuid();

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync((ProjectUser?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<ProjectUser>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockRoleRepo = new Mock<IRoleRepository>(MockBehavior.Strict);
            mockRoleRepo.Setup(r => r.ExistsAsync(roleId)).ReturnsAsync(true);

            var service = new ProjectUserService(mockRepo.Object, mockRoleRepo.Object, CreateMockLogger());

            var result = await service.AddUserToProjectAsync(projectId, userId, roleId);

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
            var roleId = Guid.NewGuid();
            var existingPu = new ProjectUser(projectId, userId, roleId);

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync(existingPu);

            var mockRoleRepo = new Mock<IRoleRepository>(MockBehavior.Strict);
            mockRoleRepo.Setup(r => r.ExistsAsync(roleId)).ReturnsAsync(true);

            var service = new ProjectUserService(mockRepo.Object, mockRoleRepo.Object, CreateMockLogger());

            var result = await service.AddUserToProjectAsync(projectId, userId, roleId);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("asignado", result.Message, StringComparison.OrdinalIgnoreCase);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<ProjectUser>(), default), Times.Never);
        }

        [Fact]
        public async Task AddUserToProject_ReturnsError_WhenRoleDoesNotExist()
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            
            var mockRoleRepo = new Mock<IRoleRepository>(MockBehavior.Strict);
            mockRoleRepo.Setup(r => r.ExistsAsync(roleId)).ReturnsAsync(false);

            var service = new ProjectUserService(mockRepo.Object, mockRoleRepo.Object, CreateMockLogger());

            var result = await service.AddUserToProjectAsync(projectId, userId, roleId);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("rol", result.Message, StringComparison.OrdinalIgnoreCase);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<ProjectUser>(), default), Times.Never);
        }

        #endregion

        #region RemoveUserFromProject Tests

        [Fact]
        public async Task RemoveUserFromProject_ReturnsSuccess_WhenUserIsInProject()
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var existingPu = new ProjectUser(projectId, userId, roleId);

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync(existingPu);
            mockRepo.Setup(r => r.RemoveAsync(existingPu, default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockRoleRepo = new Mock<IRoleRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockRoleRepo, CreateMockLogger());

            var result = await service.RemoveUserFromProjectAsync(projectId, userId);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("exitosa", result.Message, StringComparison.OrdinalIgnoreCase);
            mockRepo.Verify(r => r.RemoveAsync(existingPu, default), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromProject_ReturnsError_WhenUserNotInProject()
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync((ProjectUser?)null);

            var mockRoleRepo = new Mock<IRoleRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockRoleRepo, CreateMockLogger());

            var result = await service.RemoveUserFromProjectAsync(projectId, userId);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("asignado", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region ChangeUserRole Tests

        [Fact]
        public async Task ChangeUserRole_ReturnsSuccess_WhenUserIsInProject()
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oldRoleId = Guid.NewGuid();
            var newRoleId = Guid.NewGuid();
            var existingPu = new ProjectUser(projectId, userId, oldRoleId);

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync(existingPu);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockRoleRepo = new Mock<IRoleRepository>(MockBehavior.Strict);
            mockRoleRepo.Setup(r => r.ExistsAsync(newRoleId)).ReturnsAsync(true);

            var service = new ProjectUserService(mockRepo.Object, mockRoleRepo.Object, CreateMockLogger());

            var result = await service.ChangeUserRoleAsync(projectId, userId, newRoleId);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("actualizado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.ProjectUser);
            Assert.Equal(newRoleId, result.ProjectUser.RoleId);
        }

        [Fact]
        public async Task ChangeUserRole_ReturnsError_WhenUserNotInProject()
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var newRoleId = Guid.NewGuid();

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync((ProjectUser?)null);

            var mockRoleRepo = new Mock<IRoleRepository>(MockBehavior.Strict);
            mockRoleRepo.Setup(r => r.ExistsAsync(newRoleId)).ReturnsAsync(true);

            var service = new ProjectUserService(mockRepo.Object, mockRoleRepo.Object, CreateMockLogger());

            var result = await service.ChangeUserRoleAsync(projectId, userId, newRoleId);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("asignado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.ProjectUser);
        }

        [Fact]
        public async Task ChangeUserRole_ReturnsError_WhenRoleDoesNotExist()
        {
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oldRoleId = Guid.NewGuid();
            var newRoleId = Guid.NewGuid();
            var existingPu = new ProjectUser(projectId, userId, oldRoleId);

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(projectId, userId, default)).ReturnsAsync(existingPu);

            var mockRoleRepo = new Mock<IRoleRepository>(MockBehavior.Strict);
            mockRoleRepo.Setup(r => r.ExistsAsync(newRoleId)).ReturnsAsync(false);

            var service = new ProjectUserService(mockRepo.Object, mockRoleRepo.Object, CreateMockLogger());

            var result = await service.ChangeUserRoleAsync(projectId, userId, newRoleId);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("rol", result.Message, StringComparison.OrdinalIgnoreCase);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        #endregion

        #region GetProjectUsers Tests

        [Fact]
        public async Task GetProjectUsers_ReturnsUsers_WhenUsersExist()
        {
            var projectId = Guid.NewGuid();
            var roleId1 = Guid.NewGuid();
            var roleId2 = Guid.NewGuid();
            var users = new List<ProjectUser>
            {
                new ProjectUser(projectId, Guid.NewGuid(), roleId1),
                new ProjectUser(projectId, Guid.NewGuid(), roleId2)
            };

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByProjectIdAsync(projectId, default)).ReturnsAsync(users);

            var mockRoleRepo = new Mock<IRoleRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockRoleRepo, CreateMockLogger());

            var result = await service.GetProjectUsersAsync(projectId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetProjectUsers_ReturnsEmpty_WhenNoUsersExist()
        {
            var projectId = Guid.NewGuid();
            var users = new List<ProjectUser>();

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByProjectIdAsync(projectId, default)).ReturnsAsync(users);

            var mockRoleRepo = new Mock<IRoleRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockRoleRepo, CreateMockLogger());

            var result = await service.GetProjectUsersAsync(projectId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetUserProjects Tests

        [Fact]
        public async Task GetUserProjects_ReturnsProjects_WhenProjectsExist()
        {
            var userId = Guid.NewGuid();
            var projectId1 = Guid.NewGuid();
            var projectId2 = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var projects = new List<ProjectUser>
            {
                new ProjectUser(projectId1, userId, roleId),
                new ProjectUser(projectId2, userId, roleId)
            };

            var mockRepo = new Mock<IProjectUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUserIdAsync(userId, default)).ReturnsAsync(projects);

            var mockRoleRepo = new Mock<IRoleRepository>().Object;

            var service = new ProjectUserService(mockRepo.Object, mockRoleRepo, CreateMockLogger());

            var result = await service.GetUserProjectsAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        #endregion
    }
}

