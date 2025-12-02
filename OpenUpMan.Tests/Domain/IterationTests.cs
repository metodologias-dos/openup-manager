using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain;

public class IterationTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateIteration()
    {
        // Arrange
        var phaseId = 1;
        var name = "Test Iteration";
        var goal = "Test Goal";

        // Act
        var iteration = new Iteration(phaseId, name, goal);

        // Assert
        Assert.Equal(phaseId, iteration.PhaseId);
        Assert.Equal(name, iteration.Name);
        Assert.Equal(goal, iteration.Goal);
        Assert.Equal(0, iteration.CompletionPercentage);
        Assert.Null(iteration.StartDate);
        Assert.Null(iteration.EndDate);
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdateIteration()
    {
        // Arrange
        var iteration = new Iteration(1, "Old Name", "Old Goal");
        var newName = "New Name";
        var newGoal = "New Goal";
        var newStartDate = DateTime.UtcNow;
        var newEndDate = DateTime.UtcNow.AddDays(7);

        // Act
        iteration.UpdateDetails(newName, newGoal, newStartDate, newEndDate);

        // Assert
        Assert.Equal(newName, iteration.Name);
        Assert.Equal(newGoal, iteration.Goal);
        Assert.Equal(newStartDate, iteration.StartDate);
        Assert.Equal(newEndDate, iteration.EndDate);
    }

    [Fact]
    public void SetCompletionPercentage_WithValidPercentage_ShouldUpdateCompletionPercentage()
    {
        // Arrange
        var iteration = new Iteration(1);
        var percentage = 50;

        // Act
        iteration.SetCompletionPercentage(percentage);

        // Assert
        Assert.Equal(percentage, iteration.CompletionPercentage);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void SetCompletionPercentage_WithInvalidPercentage_ShouldThrowArgumentException(int percentage)
    {
        // Arrange
        var iteration = new Iteration(1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => iteration.SetCompletionPercentage(percentage));
    }
}
