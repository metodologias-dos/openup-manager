using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Domain;

public class UserTests
{
    [Fact]
    public void SetPasswordHash_WithValidHash_ShouldUpdatePasswordHash()
    {
        // Arrange
        var user = new User("testuser", "oldpassword");
        var newPasswordHash = "newpassword";

        // Act
        user.SetPasswordHash(newPasswordHash);

        // Assert
        Assert.Equal(newPasswordHash, user.PasswordHash);
    }

    [Fact]
    public void SetPasswordHash_WithNullOrEmptyHash_ShouldThrowArgumentException()
    {
        // Arrange
        var user = new User("testuser", "oldpassword");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.SetPasswordHash(null!));
        Assert.Throws<ArgumentException>(() => user.SetPasswordHash(""));
        Assert.Throws<ArgumentException>(() => user.SetPasswordHash("   "));
    }
}
