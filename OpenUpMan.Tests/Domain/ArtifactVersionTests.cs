using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain;

public class ArtifactVersionTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateArtifactVersion()
    {
        // Arrange
        var artifactId = 1;
        var createdBy = 1;
        var notes = "Test Notes";
        var fileBlob = new byte[] { 1, 2, 3 };
        var fileMime = "application/octet-stream";
        var buildInfo = "Test Build Info";

        // Act
        var artifactVersion = new ArtifactVersion(artifactId, createdBy, notes, fileBlob, fileMime, buildInfo);

        // Assert
        Assert.Equal(artifactId, artifactVersion.ArtifactId);
        Assert.Equal(createdBy, artifactVersion.CreatedBy);
        Assert.Equal(notes, artifactVersion.Notes);
        Assert.Equal(fileBlob, artifactVersion.FileBlob);
        Assert.Equal(fileMime, artifactVersion.FileMime);
        Assert.Equal(buildInfo, artifactVersion.BuildInfo);
        Assert.True(artifactVersion.CreatedAt <= DateTime.UtcNow);
        Assert.Equal(0, artifactVersion.VersionNumber); // Initial value
    }

    [Fact]
    public void UpdateFile_WithValidParameters_ShouldUpdateFile()
    {
        // Arrange
        var artifactVersion = new ArtifactVersion(1, 1);
        var newFileBlob = new byte[] { 4, 5, 6 };
        var newFileMime = "application/pdf";

        // Act
        artifactVersion.UpdateFile(newFileBlob, newFileMime);

        // Assert
        Assert.Equal(newFileBlob, artifactVersion.FileBlob);
        Assert.Equal(newFileMime, artifactVersion.FileMime);
    }

    [Fact]
    public void UpdateNotes_WithValidNotes_ShouldUpdateNotes()
    {
        // Arrange
        var artifactVersion = new ArtifactVersion(1, 1);
        var newNotes = "New Notes";

        // Act
        artifactVersion.UpdateNotes(newNotes);

        // Assert
        Assert.Equal(newNotes, artifactVersion.Notes);
    }



    [Fact]
    public void UpdateBuildInfo_WithValidBuildInfo_ShouldUpdateBuildInfo()
    {
        // Arrange
        var artifactVersion = new ArtifactVersion(1, 1);
        var newBuildInfo = "New Build Info";

        // Act
        artifactVersion.UpdateBuildInfo(newBuildInfo);

        // Assert
        Assert.Equal(newBuildInfo, artifactVersion.BuildInfo);
    }
}
