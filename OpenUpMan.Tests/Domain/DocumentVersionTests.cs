using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Domain
{
    public class DocumentVersionTests
    {
        [Fact]
        public void Constructor_ShouldInitializeDocumentVersion_WithValidParameters()
        {
            var documentId = Guid.NewGuid();
            var versionNumber = 1;
            var createdBy = Guid.NewGuid();
            var extension = ".pdf";
            var binario = new byte[] { 1, 2, 3, 4, 5 };
            var observations = "Primera versión del documento";

            var version = new DocumentVersion(documentId, versionNumber, createdBy, extension, binario, observations);

            Assert.NotEqual(Guid.Empty, version.Id);
            Assert.Equal(documentId, version.DocumentId);
            Assert.Equal(versionNumber, version.VersionNumber);
            Assert.Equal(createdBy, version.CreatedBy);
            Assert.Equal(extension, version.Extension);
            Assert.Equal(binario, version.Binario);
            Assert.Equal(observations, version.Observations);
            Assert.True((DateTime.UtcNow - version.CreatedAt).TotalSeconds < 2);
        }

        [Fact]
        public void Constructor_ShouldInitialize_WithoutObservations()
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();
            var extension = ".pdf";
            var binario = new byte[] { 1, 2, 3 };

            var version = new DocumentVersion(documentId, 1, createdBy, extension, binario);

            Assert.Null(version.Observations);
            Assert.Equal(extension, version.Extension);
            Assert.Equal(binario, version.Binario);
        }

        [Fact]
        public void UpdateObservations_ShouldUpdateObservations()
        {
            var version = new DocumentVersion(Guid.NewGuid(), 1, Guid.NewGuid(), ".pdf", new byte[] { 1, 2, 3 }, "Old observations");

            version.UpdateObservations("New observations");

            Assert.Equal("New observations", version.Observations);
        }

        [Fact]
        public void UpdateObservations_ShouldAllowNullObservations()
        {
            var version = new DocumentVersion(Guid.NewGuid(), 1, Guid.NewGuid(), ".pdf", new byte[] { 1, 2, 3 }, "Observations");

            version.UpdateObservations(null);

            Assert.Null(version.Observations);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenDocumentIdIsEmpty()
        {
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new DocumentVersion(Guid.Empty, 1, createdBy, ".pdf", new byte[] { 1, 2, 3 }));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenVersionNumberIsZero()
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new DocumentVersion(documentId, 0, createdBy, ".pdf", new byte[] { 1, 2, 3 }));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenVersionNumberIsNegative()
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new DocumentVersion(documentId, -1, createdBy, ".pdf", new byte[] { 1, 2, 3 }));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenCreatedByIsEmpty()
        {
            var documentId = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new DocumentVersion(documentId, 1, Guid.Empty, ".pdf", new byte[] { 1, 2, 3 }));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_ShouldThrow_WhenExtensionIsInvalid(string? extension)
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new DocumentVersion(documentId, 1, createdBy, extension!, new byte[] { 1, 2, 3 }));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenBinarioIsNull()
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new DocumentVersion(documentId, 1, createdBy, ".pdf", null!));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenBinarioIsEmpty()
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new DocumentVersion(documentId, 1, createdBy, ".pdf", new byte[0]));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(100)]
        public void Constructor_ShouldAcceptValidVersionNumbers(int versionNumber)
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            var version = new DocumentVersion(documentId, versionNumber, createdBy, ".pdf", new byte[] { 1, 2, 3 });

            Assert.Equal(versionNumber, version.VersionNumber);
        }
    }
}

