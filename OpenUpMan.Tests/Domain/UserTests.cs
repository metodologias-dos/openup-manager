using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Domain
{
    public class UserTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldInitializeUser_WithValidParameters()
        {
            // Arrange
            var username = "testuser";
            var passwordHash = "hashedPassword123";

            // Act
            var user = new User(username, passwordHash);

            // Assert
            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.Equal(username, user.Username);
            Assert.Equal(passwordHash, user.PasswordHash);
            Assert.True((DateTime.UtcNow - user.CreatedAt).TotalSeconds < 1);
            Assert.Null(user.PasswordChangedAt);
        }

        #endregion

        #region SetPasswordHash Tests

        [Fact]
        public void SetPasswordHash_ShouldUpdatePasswordHash_WhenValidHashProvided()
        {
            // Arrange
            var user = new User("testuser", "initialHash");
            var newPasswordHash = "newHashedPassword456";

            // Act
            user.SetPasswordHash(newPasswordHash);

            // Assert
            Assert.Equal(newPasswordHash, user.PasswordHash);
            Assert.NotNull(user.PasswordChangedAt);
            Assert.True((DateTime.UtcNow - user.PasswordChangedAt.Value).TotalSeconds < 1);
        }

        [Fact]
        public void SetPasswordHash_ShouldUpdatePasswordChangedAt_WhenCalledMultipleTimes()
        {
            // Arrange
            var user = new User("testuser", "initialHash");
            var firstHash = "firstNewHash";
            var secondHash = "secondNewHash";

            // Act
            user.SetPasswordHash(firstHash);
            var firstChangeTime = user.PasswordChangedAt;
            
            // Small delay to ensure different timestamp
            Thread.Sleep(10);
            
            user.SetPasswordHash(secondHash);
            var secondChangeTime = user.PasswordChangedAt;

            // Assert
            Assert.Equal(secondHash, user.PasswordHash);
            Assert.NotNull(firstChangeTime);
            Assert.NotNull(secondChangeTime);
            Assert.True(secondChangeTime > firstChangeTime);
        }

        [Fact]
        public void SetPasswordHash_ShouldThrowArgumentException_WhenHashIsNull()
        {
            // Arrange
            var user = new User("testuser", "initialHash");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.SetPasswordHash(null!));
            Assert.Equal("passwordHash", exception.ParamName);
            Assert.Contains("Password hash cannot be null or empty", exception.Message);
        }

        [Fact]
        public void SetPasswordHash_ShouldThrowArgumentException_WhenHashIsEmpty()
        {
            // Arrange
            var user = new User("testuser", "initialHash");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.SetPasswordHash(string.Empty));
            Assert.Equal("passwordHash", exception.ParamName);
            Assert.Contains("Password hash cannot be null or empty", exception.Message);
        }

        [Fact]
        public void SetPasswordHash_ShouldThrowArgumentException_WhenHashIsWhitespace()
        {
            // Arrange
            var user = new User("testuser", "initialHash");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.SetPasswordHash("   "));
            Assert.Equal("passwordHash", exception.ParamName);
            Assert.Contains("Password hash cannot be null or empty", exception.Message);
        }

        #endregion
    }
}

