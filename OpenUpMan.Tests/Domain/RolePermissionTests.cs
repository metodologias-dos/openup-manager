using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain
{
    public class RolePermissionTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesRolePermission()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();

            // Act
            var rolePermission = new RolePermission(roleId, permissionId);

            // Assert
            Assert.Equal(roleId, rolePermission.RoleId);
            Assert.Equal(permissionId, rolePermission.PermissionId);
        }

        [Fact]
        public void Constructor_EmptyRoleId_ThrowsArgumentException()
        {
            // Arrange
            var permissionId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new RolePermission(Guid.Empty, permissionId));
        }

        [Fact]
        public void Constructor_EmptyPermissionId_ThrowsArgumentException()
        {
            // Arrange
            var roleId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new RolePermission(roleId, Guid.Empty));
        }

        [Fact]
        public void Constructor_BothEmpty_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new RolePermission(Guid.Empty, Guid.Empty));
        }
    }
}

