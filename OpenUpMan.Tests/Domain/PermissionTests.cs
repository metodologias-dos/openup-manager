using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain
{
    public class PermissionTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesPermission()
        {
            // Arrange
            var name = "BorrarProyecto";
            var description = "Permite borrar proyectos";

            // Act
            var permission = new Permission(name, description);

            // Assert
            Assert.NotEqual(Guid.Empty, permission.Id);
            Assert.Equal(name, permission.Name);
            Assert.Equal(description, permission.Description);
        }

        [Fact]
        public void Constructor_WithoutDescription_CreatesPermission()
        {
            // Arrange
            var name = "RenombrarProyecto";

            // Act
            var permission = new Permission(name);

            // Assert
            Assert.NotEqual(Guid.Empty, permission.Id);
            Assert.Equal(name, permission.Name);
            Assert.Null(permission.Description);
        }

        [Fact]
        public void Constructor_NullName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Permission(null!));
        }

        [Fact]
        public void Constructor_EmptyName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Permission(""));
        }

        [Fact]
        public void Constructor_WhitespaceName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Permission("   "));
        }

        [Fact]
        public void UpdateDetails_ValidParameters_UpdatesPermission()
        {
            // Arrange
            var permission = new Permission("AgregarUsuarios", "Agregar usuarios al proyecto");
            var newName = "AgregarYEliminarUsuarios";
            var newDescription = "Agregar y eliminar usuarios del proyecto";

            // Act
            permission.UpdateDetails(newName, newDescription);

            // Assert
            Assert.Equal(newName, permission.Name);
            Assert.Equal(newDescription, permission.Description);
        }

        [Fact]
        public void UpdateDetails_NullName_ThrowsArgumentException()
        {
            // Arrange
            var permission = new Permission("AgregarUsuarios");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => permission.UpdateDetails(null!, "Description"));
        }

        [Fact]
        public void UpdateDetails_EmptyName_ThrowsArgumentException()
        {
            // Arrange
            var permission = new Permission("AgregarUsuarios");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => permission.UpdateDetails("", "Description"));
        }
    }
}

