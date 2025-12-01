using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain
{
    public class PhaseArtefactTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesPhaseArtefact()
        {
            // Arrange
            var phaseId = Guid.NewGuid();
            var artefactId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var registrado = true;

            // Act
            var phaseArtefact = new PhaseArtefact(phaseId, artefactId, documentId, registrado);

            // Assert
            Assert.Equal(phaseId, phaseArtefact.PhaseId);
            Assert.Equal(artefactId, phaseArtefact.ArtefactId);
            Assert.Equal(documentId, phaseArtefact.DocumentId);
            Assert.True(phaseArtefact.Registrado);
        }

        [Fact]
        public void Constructor_WithoutOptionalParameters_CreatesPhaseArtefact()
        {
            // Arrange
            var phaseId = Guid.NewGuid();
            var artefactId = Guid.NewGuid();

            // Act
            var phaseArtefact = new PhaseArtefact(phaseId, artefactId);

            // Assert
            Assert.Equal(phaseId, phaseArtefact.PhaseId);
            Assert.Equal(artefactId, phaseArtefact.ArtefactId);
            Assert.Null(phaseArtefact.DocumentId);
            Assert.False(phaseArtefact.Registrado);
        }

        [Fact]
        public void Constructor_EmptyPhaseId_ThrowsArgumentException()
        {
            // Arrange
            var artefactId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new PhaseArtefact(Guid.Empty, artefactId));
        }

        [Fact]
        public void Constructor_EmptyArtefactId_ThrowsArgumentException()
        {
            // Arrange
            var phaseId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new PhaseArtefact(phaseId, Guid.Empty));
        }

        [Fact]
        public void SetDocument_ValidDocumentId_UpdatesDocumentId()
        {
            // Arrange
            var phaseId = Guid.NewGuid();
            var artefactId = Guid.NewGuid();
            var phaseArtefact = new PhaseArtefact(phaseId, artefactId);
            var documentId = Guid.NewGuid();

            // Act
            phaseArtefact.SetDocument(documentId);

            // Assert
            Assert.Equal(documentId, phaseArtefact.DocumentId);
        }

        [Fact]
        public void SetDocument_NullDocumentId_ClearsDocumentId()
        {
            // Arrange
            var phaseId = Guid.NewGuid();
            var artefactId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var phaseArtefact = new PhaseArtefact(phaseId, artefactId, documentId);

            // Act
            phaseArtefact.SetDocument(null);

            // Assert
            Assert.Null(phaseArtefact.DocumentId);
        }

        [Fact]
        public void MarkAsRegistered_SetsRegistradoToTrue()
        {
            // Arrange
            var phaseId = Guid.NewGuid();
            var artefactId = Guid.NewGuid();
            var phaseArtefact = new PhaseArtefact(phaseId, artefactId, null, false);

            // Act
            phaseArtefact.MarkAsRegistered();

            // Assert
            Assert.True(phaseArtefact.Registrado);
        }

        [Fact]
        public void MarkAsUnregistered_SetsRegistradoToFalse()
        {
            // Arrange
            var phaseId = Guid.NewGuid();
            var artefactId = Guid.NewGuid();
            var phaseArtefact = new PhaseArtefact(phaseId, artefactId, null, true);

            // Act
            phaseArtefact.MarkAsUnregistered();

            // Assert
            Assert.False(phaseArtefact.Registrado);
        }

        [Fact]
        public void SetRegistrado_True_SetsRegistradoToTrue()
        {
            // Arrange
            var phaseId = Guid.NewGuid();
            var artefactId = Guid.NewGuid();
            var phaseArtefact = new PhaseArtefact(phaseId, artefactId);

            // Act
            phaseArtefact.SetRegistrado(true);

            // Assert
            Assert.True(phaseArtefact.Registrado);
        }

        [Fact]
        public void SetRegistrado_False_SetsRegistradoToFalse()
        {
            // Arrange
            var phaseId = Guid.NewGuid();
            var artefactId = Guid.NewGuid();
            var phaseArtefact = new PhaseArtefact(phaseId, artefactId, null, true);

            // Act
            phaseArtefact.SetRegistrado(false);

            // Assert
            Assert.False(phaseArtefact.Registrado);
        }

        [Fact]
        public void Constructor_BothIdsEmpty_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new PhaseArtefact(Guid.Empty, Guid.Empty));
        }
    }
}

