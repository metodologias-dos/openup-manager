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
            var description = "Descripción del documento";

            var document = new Document(phaseItemId, title, description);

            Assert.NotEqual(Guid.Empty, document.Id);
            Assert.Equal(phaseItemId, document.PhaseItemId);
            Assert.Equal(title, document.Title);
            Assert.Equal(description, document.Description);
        }

        [Fact]
        public void Constructor_ShouldInitialize_WithoutDescription()
        {
            var phaseItemId = Guid.NewGuid();
            var title = "Documento de Requisitos";

            var document = new Document(phaseItemId, title);

            Assert.Null(document.Description);
            Assert.Equal(title, document.Title);
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateTitleAndDescription()
        {
            var document = new Document(Guid.NewGuid(), "Old Title", "Old Description");

            document.UpdateDetails("New Title", "New Description");

            Assert.Equal("New Title", document.Title);
            Assert.Equal("New Description", document.Description);
        }

        [Fact]
        public void UpdateDetails_ShouldAllowNullDescription()
        {
            var document = new Document(Guid.NewGuid(), "Title", "Description");

            document.UpdateDetails("New Title", null);

            Assert.Equal("New Title", document.Title);
            Assert.Null(document.Description);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenPhaseItemIdIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => 
                new Document(Guid.Empty, "Title"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_ShouldThrow_WhenTitleIsInvalid(string? title)
        {
            var phaseItemId = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new Document(phaseItemId, title!));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateDetails_ShouldThrow_WhenTitleIsInvalid(string title)
        {
            var document = new Document(Guid.NewGuid(), "Original Title");

            Assert.Throws<ArgumentException>(() => document.UpdateDetails(title, "Desc"));
        }
    }
}

