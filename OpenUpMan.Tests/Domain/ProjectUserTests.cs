using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain
{
    public class ProjectUserTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesProjectUser()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            // Act
            var projectUser = new ProjectUser(projectId, userId, roleId);

            // Assert
            Assert.Equal(projectId, projectUser.ProjectId);
            Assert.Equal(userId, projectUser.UserId);
            Assert.Equal(roleId, projectUser.RoleId);
        }

        [Fact]
        public void Constructor_EmptyProjectId_ThrowsArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ProjectUser(Guid.Empty, userId, roleId));
        }

        [Fact]
        public void Constructor_EmptyUserId_ThrowsArgumentException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ProjectUser(projectId, Guid.Empty, roleId));
        }

        [Fact]
        public void Constructor_EmptyRoleId_ThrowsArgumentException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ProjectUser(projectId, userId, Guid.Empty));
        }

        [Fact]
        public void SetRole_ValidRoleId_UpdatesRole()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var projectUser = new ProjectUser(projectId, userId, roleId);
            var newRoleId = Guid.NewGuid();

            // Act
            projectUser.SetRole(newRoleId);

            // Assert
            Assert.Equal(newRoleId, projectUser.RoleId);
        }

        [Fact]
        public void SetRole_EmptyRoleId_ThrowsArgumentException()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var projectUser = new ProjectUser(projectId, userId, roleId);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => projectUser.SetRole(Guid.Empty));
        }
    }
}

