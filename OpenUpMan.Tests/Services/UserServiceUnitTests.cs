using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using OpenUpMan.Services;
using Xunit;

namespace OpenUpMan.Tests.Services
{
    public class UserServiceUnitTests
    {
        private static ILogger<UserService> CreateMockLogger()
        {
            return new Mock<ILogger<UserService>>().Object;
        }

        #region CreateUser Tests

        [Fact]
        public async Task CreateUser_ReturnsSuccess_WhenNewUsername()
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUsernameAsync("mike", default)).ReturnsAsync((User?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<User>(), default)).Returns(Task.CompletedTask).Verifiable();
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask).Verifiable();

            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.CreateUserAsync("mike", "P@ssw0rd");

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("exitosa", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.User);
            Assert.Equal("mike", result.User.Username);
            mockRepo.Verify(r => r.AddAsync(It.Is<User>(u => u.Username == "mike"), default), Times.Once);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task CreateUser_ReturnsError_WhenUsernameExists()
        {
            var existing = new User("exists", "h");
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUsernameAsync("exists", default)).ReturnsAsync(existing);
            
            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.CreateUserAsync("exists", "abc");

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.NotEmpty(result.Message);
            Assert.Null(result.User);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<User>(), default), Times.Never);
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        [Theory]
        [InlineData("", "password")]
        [InlineData("  ", "password")]
        [InlineData("username", "")]
        [InlineData("username", "  ")]
        [InlineData("", "")]
        public async Task CreateUser_ReturnsError_WhenCredentialsInvalid(string username, string password)
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.CreateUserAsync(username, password);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("inválido", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.User);
        }

        [Fact]
        public async Task CreateUser_ReturnsError_WhenDatabaseThrowsException()
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUsernameAsync("test", default))
                .ThrowsAsync(new Exception("Database connection failed"));
            
            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.CreateUserAsync("test", "password");

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("Error", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.User);
        }

        #endregion

        #region Authenticate Tests

        [Fact]
        public async Task Authenticate_ReturnsSuccess_WhenCredentialsAreCorrect()
        {
            var password = "secret";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var existing = new User("authUser", hash);

            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUsernameAsync("authUser", default)).ReturnsAsync(existing);

            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.AuthenticateAsync("authUser", password);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("exitosa", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.User);
            Assert.Equal("authUser", result.User.Username);
            mockRepo.Verify(r => r.GetByUsernameAsync("authUser", default), Times.Once);
        }

        [Fact]
        public async Task Authenticate_ReturnsError_WhenUserDoesNotExist()
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUsernameAsync("nonexistent", default)).ReturnsAsync((User?)null);

            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.AuthenticateAsync("nonexistent", "password");

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("inválida", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.User);
            mockRepo.Verify(r => r.GetByUsernameAsync("nonexistent", default), Times.Once);
        }

        [Fact]
        public async Task Authenticate_ReturnsError_WhenPasswordIsIncorrect()
        {
            var correctPassword = "correct";
            var hash = BCrypt.Net.BCrypt.HashPassword(correctPassword);
            var existing = new User("authUser", hash);

            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUsernameAsync("authUser", default)).ReturnsAsync(existing);

            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.AuthenticateAsync("authUser", "wrongPassword");

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("inválida", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.User);
            mockRepo.Verify(r => r.GetByUsernameAsync("authUser", default), Times.Once);
        }

        [Theory]
        [InlineData("", "password")]
        [InlineData("  ", "password")]
        [InlineData("username", "")]
        [InlineData("username", "  ")]
        [InlineData("", "")]
        public async Task Authenticate_ReturnsError_WhenCredentialsAreEmpty(string username, string password)
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.AuthenticateAsync(username, password);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("inválida", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.User);
        }

        [Fact]
        public async Task Authenticate_ReturnsError_WhenDatabaseThrowsException()
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUsernameAsync("test", default))
                .ThrowsAsync(new Exception("Database connection failed"));
            
            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.AuthenticateAsync("test", "password");

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("Error", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(result.User);
        }

        #endregion

        #region Edge Case Tests

        [Theory]
        [InlineData("user@example.com", "password123")] // Email-like username
        [InlineData("user.name", "P@ssw0rd!")] // Username with dot
        [InlineData("user_name", "Pass123$")] // Username with underscore
        [InlineData("user-name", "MyP@ss123")] // Username with hyphen
        [InlineData("User123", "12345678")] // Mixed case username
        public async Task CreateUser_AcceptsValidSpecialCharacters(string username, string password)
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUsernameAsync(username, default)).ReturnsAsync((User?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<User>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.CreateUserAsync(username, password);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
        }

        [Theory]
        [InlineData("a", "password")] // Very short username
        [InlineData("verylongusernamethatshouldstillwork1234567890", "password")] // Long username
        [InlineData("user", "p")] // Very short password
        [InlineData("user", "verylongpasswordthatshouldstillwork1234567890!@#$%")] // Long password
        public async Task CreateUser_HandlesLengthBoundaries(string username, string password)
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUsernameAsync(username, default)).ReturnsAsync((User?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<User>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.CreateUserAsync(username, password);

            Assert.True(result.Success);
        }

        [Theory]
        [InlineData("admin'; DROP TABLE users;--", "password")] // SQL injection attempt
        [InlineData("<script>alert('xss')</script>", "password")] // XSS attempt
        [InlineData("user' OR '1'='1", "password")] // SQL injection attempt
        public async Task CreateUser_HandlesMaliciousInput(string username, string password)
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUsernameAsync(username, default)).ReturnsAsync((User?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<User>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new UserService(mockRepo.Object, CreateMockLogger());

            // Should handle gracefully - either accept or reject, but not crash
            var result = await service.CreateUserAsync(username, password);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Message);
        }

        [Fact]
        public async Task CreateUser_HandlesUnicodeCharacters()
        {
            var username = "用户名"; // Chinese characters
            var password = "密码123";
            
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUsernameAsync(username, default)).ReturnsAsync((User?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<User>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.CreateUserAsync(username, password);

            Assert.True(result.Success);
        }

        [Theory]
        [InlineData("\t\t", "password")] // Tabs only
        [InlineData("\n\n", "password")] // Newlines only
        [InlineData("   user   ", "password")] // Leading/trailing spaces
        public async Task CreateUser_HandlesWhitespaceVariations(string username, string password)
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.CreateUserAsync(username, password);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
        }

        [Fact]
        public async Task Authenticate_HandlesNullUsername()
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.AuthenticateAsync(null!, "password");

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
        }

        [Fact]
        public async Task Authenticate_HandlesNullPassword()
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.AuthenticateAsync("username", null!);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
        }

        [Fact]
        public async Task CreateUser_HandlesRepositoryTimeoutException()
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUsernameAsync("test", default))
                .ThrowsAsync(new TimeoutException("Database timeout"));
            
            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.CreateUserAsync("test", "password");

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
        }

        [Fact]
        public async Task CreateUser_HandlesRepositorySaveFailure()
        {
            var mockRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByUsernameAsync("test", default)).ReturnsAsync((User?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<User>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default))
                .ThrowsAsync(new Exception("Database save failed"));
            
            var service = new UserService(mockRepo.Object, CreateMockLogger());

            var result = await service.CreateUserAsync("test", "password");

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
        }

        #endregion
    }
}

