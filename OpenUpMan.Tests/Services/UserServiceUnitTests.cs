using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using OpenUpMan.Services;

namespace OpenUpMan.Tests.Services;

public class UserServiceUnitTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _userService;

    public UserServiceUnitTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _userService = new UserService(_userRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var username = "testuser";
        var password = "password";
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username, It.IsAny<CancellationToken>())).ReturnsAsync((User)null!);

        // Act
        var result = await _userService.CreateUserAsync(username, password);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Usuario creado exitosamente", result.Message);
        Assert.NotNull(result.User);
        Assert.Equal(username, result.User.Username);
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WithExistingUsername_ShouldReturnError()
    {
        // Arrange
        var username = "testuser";
        var password = "password";
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username, It.IsAny<CancellationToken>())).ReturnsAsync(new User(username, "hash"));

        // Act
        var result = await _userService.CreateUserAsync(username, password);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("No se pudo crear el usuario. Por favor, revise los datos.", result.Message);
        Assert.Null(result.User);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("username", "")]
    [InlineData(" ", "password")]
    [InlineData("username", " ")]
    public async Task CreateUserAsync_WithInvalidData_ShouldReturnError(string username, string password)
    {
        // Act
        var result = await _userService.CreateUserAsync(username, password);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Nombre de usuario o contraseña inválidos.", result.Message);
        Assert.Null(result.User);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var username = "testuser";
        var password = "password";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User(username, hashedPassword);
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync(username, password);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Autenticación exitosa", result.Message);
        Assert.Equal(user, result.User);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ShouldReturnError()
    {
        // Arrange
        var username = "testuser";
        var password = "password";
        var wrongPassword = "wrongpassword";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User(username, hashedPassword);
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync(username, wrongPassword);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Credenciales inválidas.", result.Message);
        Assert.Null(result.User);
    }

    [Fact]
    public async Task AuthenticateAsync_WithNonExistingUser_ShouldReturnError()
    {
        // Arrange
        var username = "testuser";
        var password = "password";
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username, It.IsAny<CancellationToken>())).ReturnsAsync((User)null!);

        // Act
        var result = await _userService.AuthenticateAsync(username, password);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Credenciales inválidas.", result.Message);
        Assert.Null(result.User);
    }
}
