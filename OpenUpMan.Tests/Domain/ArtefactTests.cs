using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain
{
    public class ArtefactTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesArtefact()
        {
            // Arrange
            var name = "Vision Document";
            var description = "Documento de visión del proyecto";

            // Act
            var artefact = new Artefact(name, description);

            // Assert
            Assert.NotEqual(Guid.Empty, artefact.Id);
            Assert.Equal(name, artefact.Name);
            Assert.Equal(description, artefact.Description);
        }

        [Fact]
        public void Constructor_WithoutDescription_CreatesArtefact()
        {
            // Arrange
            var name = "Use Case Model";

            // Act
            var artefact = new Artefact(name);

            // Assert
            Assert.NotEqual(Guid.Empty, artefact.Id);
            Assert.Equal(name, artefact.Name);
            Assert.Null(artefact.Description);
        }

        [Fact]
        public void Constructor_NullName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Artefact(null!));
        }

        [Fact]
        public void Constructor_EmptyName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Artefact(""));
        }

        [Fact]
        public void Constructor_WhitespaceName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Artefact("   "));
        }

        [Fact]
        public void UpdateDetails_ValidParameters_UpdatesArtefact()
        {
            // Arrange
            var artefact = new Artefact("Architecture Notebook", "Cuaderno de arquitectura");
            var newName = "Architecture Document";
            var newDescription = "Documento de arquitectura actualizado";

            // Act
            artefact.UpdateDetails(newName, newDescription);

            // Assert
            Assert.Equal(newName, artefact.Name);
            Assert.Equal(newDescription, artefact.Description);
        }

        [Fact]
        public void UpdateDetails_NullName_ThrowsArgumentException()
        {
            // Arrange
            var artefact = new Artefact("Design Model");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => artefact.UpdateDetails(null!, "Description"));
        }

        [Fact]
        public void UpdateDetails_EmptyName_ThrowsArgumentException()
        {
            // Arrange
            var artefact = new Artefact("Test Cases");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => artefact.UpdateDetails("", "Description"));
        }

        [Fact]
        public void UpdateDetails_WithNullDescription_UpdatesArtefact()
        {
            // Arrange
            var artefact = new Artefact("Build", "Build artifact");
            var newName = "Deployment Build";

            // Act
            artefact.UpdateDetails(newName, null);

            // Assert
            Assert.Equal(newName, artefact.Name);
            Assert.Null(artefact.Description);
        }
    }
}

