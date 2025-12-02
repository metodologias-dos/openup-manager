using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain;

public class RoleTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateRole()
    {
        // Arrange
        var name = "Test Role";
        var description = "Test Description";

        // Act
        var role = new Role(name, description);

        // Assert
        Assert.Equal(name, role.Name);
        Assert.Equal(description, role.Description);
    }

    [Fact]
    public void Constructor_WithNullOrEmptyName_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Role(null!));
        Assert.Throws<ArgumentException>(() => new Role(""));
        Assert.Throws<ArgumentException>(() => new Role("   "));
    }

    [Fact]
    public void Constructor_WithId_WithValidParameters_ShouldCreateRole()
    {
        // Arrange
        var id = 1;
        var name = "Test Role";
        var description = "Test Description";

        // Act
        var role = new Role(id, name, description);

        // Assert
        Assert.Equal(id, role.Id);
        Assert.Equal(name, role.Name);
        Assert.Equal(description, role.Description);
    }

    [Fact]
    public void Constructor_WithId_WithInvalidId_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Role(0, "Test Role"));
        Assert.Throws<ArgumentException>(() => new Role(-1, "Test Role"));
    }

    [Fact]
    public void Constructor_WithId_WithNullOrEmptyName_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Role(1, null!));
        Assert.Throws<ArgumentException>(() => new Role(1, ""));
        Assert.Throws<ArgumentException>(() => new Role(1, "   "));
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdateRole()
    {
        // Arrange
        var role = new Role("Old Name");
        var newName = "New Name";
        var newDescription = "New Description";

        // Act
        role.UpdateDetails(newName, newDescription);

        // Assert
        Assert.Equal(newName, role.Name);
        Assert.Equal(newDescription, role.Description);
    }

    [Fact]
    public void UpdateDetails_WithNullOrEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var role = new Role("Old Name");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => role.UpdateDetails(null!, null));
        Assert.Throws<ArgumentException>(() => role.UpdateDetails("", null));
        Assert.Throws<ArgumentException>(() => role.UpdateDetails("   ", null));
    }
}
