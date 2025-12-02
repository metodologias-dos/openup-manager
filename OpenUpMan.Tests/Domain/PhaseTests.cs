using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain;

public class PhaseTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreatePhase()
    {
        // Arrange
        var projectId = 1;
        var name = "Test Phase";
        var orderIndex = 1;

        // Act
        var phase = new Phase(projectId, name, orderIndex);

        // Assert
        Assert.Equal(projectId, phase.ProjectId);
        Assert.Equal(name, phase.Name);
        Assert.Equal(orderIndex, phase.OrderIndex);
        Assert.Equal("PENDING", phase.Status);
        Assert.Null(phase.StartDate);
        Assert.Null(phase.EndDate);
    }

    [Fact]
    public void Constructor_WithNullOrEmptyName_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Phase(1, null!));
        Assert.Throws<ArgumentException>(() => new Phase(1, ""));
        Assert.Throws<ArgumentException>(() => new Phase(1, "   "));
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdatePhase()
    {
        // Arrange
        var phase = new Phase(1, "Old Name");
        var newName = "New Name";
        var newStartDate = DateTime.UtcNow;
        var newEndDate = DateTime.UtcNow.AddDays(1);
        var newOrderIndex = 2;

        // Act
        phase.UpdateDetails(newName, newStartDate, newEndDate, newOrderIndex);

        // Assert
        Assert.Equal(newName, phase.Name);
        Assert.Equal(newStartDate, phase.StartDate);
        Assert.Equal(newEndDate, phase.EndDate);
        Assert.Equal(newOrderIndex, phase.OrderIndex);
    }

    [Fact]
    public void UpdateDetails_WithNullOrEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var phase = new Phase(1, "Old Name");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => phase.UpdateDetails(null!, null, null, null));
        Assert.Throws<ArgumentException>(() => phase.UpdateDetails("", null, null, null));
        Assert.Throws<ArgumentException>(() => phase.UpdateDetails("   ", null, null, null));
    }

    [Fact]
    public void SetStatus_WithValidStatus_ShouldUpdateStatus()
    {
        // Arrange
        var phase = new Phase(1, "Test Phase");
        var newStatus = "IN_PROGRESS";

        // Act
        phase.SetStatus(newStatus);

        // Assert
        Assert.Equal(newStatus, phase.Status);
    }

    [Fact]
    public void SetDates_WithValidDates_ShouldUpdateDates()
    {
        // Arrange
        var phase = new Phase(1, "Test Phase");
        var newStartDate = DateTime.UtcNow;
        var newEndDate = DateTime.UtcNow.AddDays(1);

        // Act
        phase.SetDates(newStartDate, newEndDate);

        // Assert
        Assert.Equal(newStartDate, phase.StartDate);
        Assert.Equal(newEndDate, phase.EndDate);
    }
}
