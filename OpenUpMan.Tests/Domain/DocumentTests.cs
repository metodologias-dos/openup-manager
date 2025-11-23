using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Domain
{
    public class DocumentTests
    {
        [Fact]
        public void Constructor_ShouldInitializeDocument_WithValidParameters()
        {
            var phaseItemId = Guid.NewGuid();
            var title = "Documento de Requisitos";
            var createdBy = Guid.NewGuid();
            var description = "Descripción del documento";

            var document = new Document(phaseItemId, title, createdBy, description);

            Assert.NotEqual(Guid.Empty, document.Id);
            Assert.Equal(phaseItemId, document.PhaseItemId);
            Assert.Equal(title, document.Title);
            Assert.Equal(createdBy, document.CreatedBy);
            Assert.Equal(description, document.Description);
            Assert.Equal(0, document.LastVersionNumber);
            Assert.True((DateTime.UtcNow - document.CreatedAt).TotalSeconds < 2);
        }

        [Fact]
        public void Constructor_ShouldInitialize_WithoutDescription()
        {
            var phaseItemId = Guid.NewGuid();
            var title = "Documento de Requisitos";
            var createdBy = Guid.NewGuid();

            var document = new Document(phaseItemId, title, createdBy);

            Assert.Null(document.Description);
            Assert.Equal(title, document.Title);
        }

        [Fact]
        public void IncrementVersion_ShouldIncrementLastVersionNumber()
        {
            var document = new Document(Guid.NewGuid(), "Test Doc", Guid.NewGuid());
            Assert.Equal(0, document.LastVersionNumber);

            document.IncrementVersion();
            Assert.Equal(1, document.LastVersionNumber);

            document.IncrementVersion();
            Assert.Equal(2, document.LastVersionNumber);

            document.IncrementVersion();
            Assert.Equal(3, document.LastVersionNumber);
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateTitleAndDescription()
        {
            var document = new Document(Guid.NewGuid(), "Old Title", Guid.NewGuid(), "Old Description");

            document.UpdateDetails("New Title", "New Description");

            Assert.Equal("New Title", document.Title);
            Assert.Equal("New Description", document.Description);
        }

        [Fact]
        public void UpdateDetails_ShouldAllowNullDescription()
        {
            var document = new Document(Guid.NewGuid(), "Title", Guid.NewGuid(), "Description");

            document.UpdateDetails("New Title", null);

            Assert.Equal("New Title", document.Title);
            Assert.Null(document.Description);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenPhaseItemIdIsEmpty()
        {
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new Document(Guid.Empty, "Title", createdBy));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_ShouldThrow_WhenTitleIsInvalid(string? title)
        {
            var phaseItemId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new Document(phaseItemId, title!, createdBy));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenCreatedByIsEmpty()
        {
            var phaseItemId = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new Document(phaseItemId, "Title", Guid.Empty));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateDetails_ShouldThrow_WhenTitleIsInvalid(string title)
        {
            var document = new Document(Guid.NewGuid(), "Original Title", Guid.NewGuid());

            Assert.Throws<ArgumentException>(() => document.UpdateDetails(title, "Desc"));
        }
    }
}

