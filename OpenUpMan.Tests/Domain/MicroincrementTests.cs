using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain;

public class MicroincrementTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateMicroincrement()
    {
        // Arrange
        var iterationId = 1;
        var title = "Test Microincrement";
        var authorId = 1;
        var type = "technical";
        var description = "Test Description";

        // Act
        var microincrement = new Microincrement(iterationId, title, authorId, type, description);

        // Assert
        Assert.Equal(iterationId, microincrement.IterationId);
        Assert.Equal(title, microincrement.Title);
        Assert.Equal(authorId, microincrement.AuthorId);
        Assert.Equal(type, microincrement.Type);
        Assert.Equal(description, microincrement.Description);
        Assert.True(microincrement.Date <= DateTime.UtcNow);
        Assert.Null(microincrement.ArtifactId);
        Assert.Null(microincrement.EvidenceUrl);
    }

    [Fact]
    public void Constructor_WithNullOrEmptyTitle_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Microincrement(1, null!, 1));
        Assert.Throws<ArgumentException>(() => new Microincrement(1, "", 1));
        Assert.Throws<ArgumentException>(() => new Microincrement(1, "   ", 1));
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdateMicroincrement()
    {
        // Arrange
        var microincrement = new Microincrement(1, "Old Title", 1);
        var newTitle = "New Title";
        var newDescription = "New Description";
        var newType = "bugfix";

        // Act
        microincrement.UpdateDetails(newTitle, newDescription, newType);

        // Assert
        Assert.Equal(newTitle, microincrement.Title);
        Assert.Equal(newDescription, microincrement.Description);
        Assert.Equal(newType, microincrement.Type);
    }

    [Fact]
    public void UpdateDetails_WithNullOrEmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var microincrement = new Microincrement(1, "Old Title", 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => microincrement.UpdateDetails(null!, null, "functional"));
        Assert.Throws<ArgumentException>(() => microincrement.UpdateDetails("", null, "functional"));
        Assert.Throws<ArgumentException>(() => microincrement.UpdateDetails("   ", null, "functional"));
    }

    [Fact]
    public void SetArtifact_WithValidArtifactId_ShouldUpdateArtifactId()
    {
        // Arrange
        var microincrement = new Microincrement(1, "Test Microincrement", 1);
        var artifactId = 123;

        // Act
        microincrement.SetArtifact(artifactId);

        // Assert
        Assert.Equal(artifactId, microincrement.ArtifactId);
    }

    [Fact]
    public void SetEvidenceUrl_WithValidUrl_ShouldUpdateEvidenceUrl()
    {
        // Arrange
        var microincrement = new Microincrement(1, "Test Microincrement", 1);
        var url = "http://example.com/evidence";

        // Act
        microincrement.SetEvidenceUrl(url);

        // Assert
        Assert.Equal(url, microincrement.EvidenceUrl);
    }
}
