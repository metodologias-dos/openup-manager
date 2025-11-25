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
            var filePath = "/path/to/file.pdf";
            var observations = "Primera versión del documento";

            var version = new DocumentVersion(documentId, versionNumber, createdBy, filePath, observations);

            Assert.NotEqual(Guid.Empty, version.Id);
            Assert.Equal(documentId, version.DocumentId);
            Assert.Equal(versionNumber, version.VersionNumber);
            Assert.Equal(createdBy, version.CreatedBy);
            Assert.Equal(filePath, version.FilePath);
            Assert.Equal(observations, version.Observations);
            Assert.True((DateTime.UtcNow - version.CreatedAt).TotalSeconds < 2);
        }

        [Fact]
        public void Constructor_ShouldInitialize_WithoutObservations()
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();
            var filePath = "/path/to/file.pdf";

            var version = new DocumentVersion(documentId, 1, createdBy, filePath);

            Assert.Null(version.Observations);
            Assert.Equal(filePath, version.FilePath);
        }

        [Fact]
        public void UpdateObservations_ShouldUpdateObservations()
        {
            var version = new DocumentVersion(Guid.NewGuid(), 1, Guid.NewGuid(), "/path/file.pdf", "Old observations");

            version.UpdateObservations("New observations");

            Assert.Equal("New observations", version.Observations);
        }

        [Fact]
        public void UpdateObservations_ShouldAllowNullObservations()
        {
            var version = new DocumentVersion(Guid.NewGuid(), 1, Guid.NewGuid(), "/path/file.pdf", "Observations");

            version.UpdateObservations(null);

            Assert.Null(version.Observations);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenDocumentIdIsEmpty()
        {
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new DocumentVersion(Guid.Empty, 1, createdBy, "/path/file.pdf"));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenVersionNumberIsZero()
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new DocumentVersion(documentId, 0, createdBy, "/path/file.pdf"));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenVersionNumberIsNegative()
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new DocumentVersion(documentId, -1, createdBy, "/path/file.pdf"));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenCreatedByIsEmpty()
        {
            var documentId = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new DocumentVersion(documentId, 1, Guid.Empty, "/path/file.pdf"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_ShouldThrow_WhenFilePathIsInvalid(string? filePath)
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new DocumentVersion(documentId, 1, createdBy, filePath!));
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

            var version = new DocumentVersion(documentId, versionNumber, createdBy, "/path/file.pdf");

            Assert.Equal(versionNumber, version.VersionNumber);
        }
    }
}

