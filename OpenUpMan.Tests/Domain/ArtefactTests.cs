using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain;

public class ArtefactTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateArtefact()
    {
        // Arrange
        var projectId = 1;
        var phaseId = 1;
        var name = "Test Artefact";
        var artefactType = "Test Type";
        var mandatory = true;
        var description = "Test Description";

        // Act
        var artefact = new Artifact(projectId, phaseId, name, artefactType, mandatory, description);

        // Assert
        Assert.Equal(projectId, artefact.ProjectId);
        Assert.Equal(phaseId, artefact.PhaseId);
        Assert.Equal(name, artefact.Name);
        Assert.Equal(artefactType, artefact.ArtifactType);
        Assert.Equal(mandatory, artefact.Mandatory);
        Assert.Equal(description, artefact.Description);
        Assert.Equal("PENDING", artefact.CurrentState);
    }

    [Fact]
    public void Constructor_WithNullOrEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var projectId = 1;
        var phaseId = 1;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Artifact(projectId, phaseId, null!));
        Assert.Throws<ArgumentException>(() => new Artifact(projectId, phaseId, ""));
        Assert.Throws<ArgumentException>(() => new Artifact(projectId, phaseId, "   "));
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdateArtefact()
    {
        // Arrange
        var artefact = new Artifact(1, 1, "Old Name");
        var newName = "New Name";
        var newArtefactType = "New Type";
        var newMandatory = true;
        var newDescription = "New Description";

        // Act
        artefact.UpdateDetails(newName, newArtefactType, newMandatory, newDescription);

        // Assert
        Assert.Equal(newName, artefact.Name);
        Assert.Equal(newArtefactType, artefact.ArtifactType);
        Assert.Equal(newMandatory, artefact.Mandatory);
        Assert.Equal(newDescription, artefact.Description);
    }

    [Fact]
    public void UpdateDetails_WithNullOrEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var artefact = new Artifact(1, 1, "Old Name");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => artefact.UpdateDetails(null!, null, false, null));
        Assert.Throws<ArgumentException>(() => artefact.UpdateDetails("", null, false, null));
        Assert.Throws<ArgumentException>(() => artefact.UpdateDetails("   ", null, false, null));
    }

    [Fact]
    public void SetState_WithValidState_ShouldUpdateCurrentState()
    {
        // Arrange
        var artefact = new Artifact(1, 1, "Test Artefact");
        var newState = "IN_PROGRESS";

        // Act
        artefact.SetState(newState);

        // Assert
        Assert.Equal(newState, artefact.CurrentState);
    }
}
