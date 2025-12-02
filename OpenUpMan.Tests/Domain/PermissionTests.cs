using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain;

public class PermissionTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreatePermission()
    {
        // Arrange
        var name = "Test Permission";
        var description = "Test Description";

        // Act
        var permission = new Permission(name, description);

        // Assert
        Assert.Equal(name, permission.Name);
        Assert.Equal(description, permission.Description);
    }

    [Fact]
    public void Constructor_WithNullOrEmptyName_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Permission(null!));
        Assert.Throws<ArgumentException>(() => new Permission(""));
        Assert.Throws<ArgumentException>(() => new Permission("   "));
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdatePermission()
    {
        // Arrange
        var permission = new Permission("Old Name");
        var newName = "New Name";
        var newDescription = "New Description";

        // Act
        permission.UpdateDetails(newName, newDescription);

        // Assert
        Assert.Equal(newName, permission.Name);
        Assert.Equal(newDescription, permission.Description);
    }

    [Fact]
    public void UpdateDetails_WithNullOrEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var permission = new Permission("Old Name");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => permission.UpdateDetails(null!, null));
        Assert.Throws<ArgumentException>(() => permission.UpdateDetails("", null));
        Assert.Throws<ArgumentException>(() => permission.UpdateDetails("   ", null));
    }
}
