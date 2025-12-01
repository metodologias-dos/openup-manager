using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain
{
    public class RoleTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesRole()
        {
            // Arrange
            var name = "DESARROLLADOR";
            var description = "Rol de desarrollador";

            // Act
            var role = new Role(name, description);

            // Assert
            Assert.NotEqual(Guid.Empty, role.Id);
            Assert.Equal(name, role.Name);
            Assert.Equal(description, role.Description);
        }

        [Fact]
        public void Constructor_WithoutDescription_CreatesRole()
        {
            // Arrange
            var name = "TESTER";

            // Act
            var role = new Role(name);

            // Assert
            Assert.NotEqual(Guid.Empty, role.Id);
            Assert.Equal(name, role.Name);
            Assert.Null(role.Description);
        }

        [Fact]
        public void Constructor_NullName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Role(null!));
        }

        [Fact]
        public void Constructor_EmptyName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Role(""));
        }

        [Fact]
        public void Constructor_WhitespaceName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Role("   "));
        }

        [Fact]
        public void UpdateDetails_ValidParameters_UpdatesRole()
        {
            // Arrange
            var role = new Role("ADMIN", "Administrador");
            var newName = "SUPER_ADMIN";
            var newDescription = "Super Administrador";

            // Act
            role.UpdateDetails(newName, newDescription);

            // Assert
            Assert.Equal(newName, role.Name);
            Assert.Equal(newDescription, role.Description);
        }

        [Fact]
        public void UpdateDetails_NullName_ThrowsArgumentException()
        {
            // Arrange
            var role = new Role("ADMIN");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => role.UpdateDetails(null!, "Description"));
        }

        [Fact]
        public void UpdateDetails_EmptyName_ThrowsArgumentException()
        {
            // Arrange
            var role = new Role("ADMIN");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => role.UpdateDetails("", "Description"));
        }
    }
}

