using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain;

public class ProjectTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateProject()
    {
        // Arrange
        var name = "Test Project";
        var createdBy = 1;
        var code = "TP";
        var description = "Test Description";
        var startDate = DateTime.UtcNow;

        // Act
        var project = new Project(name, createdBy, code, description, startDate);

        // Assert
        Assert.Equal(name, project.Name);
        Assert.Equal(createdBy, project.CreatedBy);
        Assert.Equal(code, project.Code);
        Assert.Equal(description, project.Description);
        Assert.Equal(startDate, project.StartDate);
        Assert.Equal("CREATED", project.Status);
        Assert.True(project.CreatedAt <= DateTime.UtcNow);
        Assert.Null(project.UpdatedAt);
        Assert.Null(project.DeletedAt);
    }

    [Fact]
    public void Constructor_WithNullOrEmptyName_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Project(null!, 1));
        Assert.Throws<ArgumentException>(() => new Project("", 1));
        Assert.Throws<ArgumentException>(() => new Project("   ", 1));
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdateProject()
    {
        // Arrange
        var project = new Project("Old Name", 1);
        var newName = "New Name";
        var newDescription = "New Description";
        var newStartDate = DateTime.UtcNow.AddDays(1);
        var newCode = "NN";

        // Act
        project.UpdateDetails(newName, newDescription, newStartDate, newCode);

        // Assert
        Assert.Equal(newName, project.Name);
        Assert.Equal(newDescription, project.Description);
        Assert.Equal(newStartDate, project.StartDate);
        Assert.Equal(newCode, project.Code);
        Assert.NotNull(project.UpdatedAt);
    }

    [Fact]
    public void UpdateDetails_WithNullOrEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var project = new Project("Old Name", 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => project.UpdateDetails(null!, null, null));
        Assert.Throws<ArgumentException>(() => project.UpdateDetails("", null, null));
        Assert.Throws<ArgumentException>(() => project.UpdateDetails("   ", null, null));
    }

    [Fact]
    public void SetStatus_WithValidStatus_ShouldUpdateStatus()
    {
        // Arrange
        var project = new Project("Test Project", 1);
        var newStatus = "IN_PROGRESS";

        // Act
        project.SetStatus(newStatus);

        // Assert
        Assert.Equal(newStatus, project.Status);
        Assert.NotNull(project.UpdatedAt);
    }

    [Fact]
    public void SoftDelete_ShouldSetDeletedAt()
    {
        // Arrange
        var project = new Project("Test Project", 1);

        // Act
        project.SoftDelete();

        // Assert
        Assert.NotNull(project.DeletedAt);
        Assert.NotNull(project.UpdatedAt);
    }
}
