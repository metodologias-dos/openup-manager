using Moq;
using OpenUpMan.Domain;
using OpenUpMan.Services;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.Tests.UI;

public class UserAuthViewModelTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserAuthViewModel _viewModel;

    public UserAuthViewModelTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _viewModel = new UserAuthViewModel(_userServiceMock.Object);
    }

    [Fact]
    public async Task SubmitAsync_Login_Success()
    {
        // Arrange
        var user = new User("test", "hash");
        _userServiceMock.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult(true, ServiceResultType.Success, "Success", user));
        _viewModel.Username = "test";
        _viewModel.Password = "password";
        var authSucceeded = false;
        _viewModel.AuthenticationSucceeded += u => authSucceeded = true;

        // Act
        await _viewModel.SubmitCommand.ExecuteAsync(null);

        // Assert
        Assert.True(authSucceeded);
        Assert.True(_viewModel.IsSuccessFeedback);
        Assert.False(_viewModel.IsErrorFeedback);
    }

    [Fact]
    public async Task SubmitAsync_Login_Failure()
    {
        // Arrange
        _userServiceMock.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult(false, ServiceResultType.Error, "Failure", null));
        _viewModel.Username = "test";
        _viewModel.Password = "wrongpassword";
        var authSucceeded = false;
        _viewModel.AuthenticationSucceeded += u => authSucceeded = true;

        // Act
        await _viewModel.SubmitCommand.ExecuteAsync(null);

        // Assert
        Assert.False(authSucceeded);
        Assert.False(_viewModel.IsSuccessFeedback);
        Assert.True(_viewModel.IsErrorFeedback);
    }

    [Fact]
    public async Task SubmitAsync_CreateUser_Success()
    {
        // Arrange
        _viewModel.ToggleModeCommand.Execute(null); // Switch to create mode
        var user = new User("newuser", "hash");
        _userServiceMock.Setup(s => s.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult(true, ServiceResultType.Success, "Success", user));
        _viewModel.Username = "newuser";
        _viewModel.Password = "password";
        var authSucceeded = false;
        _viewModel.AuthenticationSucceeded += u => authSucceeded = true;

        // Act
        await _viewModel.SubmitCommand.ExecuteAsync(null);

        // Assert
        Assert.True(authSucceeded);
        Assert.True(_viewModel.IsSuccessFeedback);
    }

    [Fact]
    public void ToggleMode_SwitchesModesAndResetsState()
    {
        // Arrange
        var initialMode = _viewModel.IsCreateMode;
        _viewModel.Username = "test";
        _viewModel.Feedback = "Some feedback";

        // Act
        _viewModel.ToggleModeCommand.Execute(null);

        // Assert
        Assert.NotEqual(initialMode, _viewModel.IsCreateMode);
        Assert.Empty(_viewModel.Username);
        Assert.Empty(_viewModel.Feedback);
        if (_viewModel.IsCreateMode)
        {
            Assert.Equal("Crear cuenta", _viewModel.SubmitButtonText);
        }
        else
        {
            Assert.Equal("Iniciar sesión", _viewModel.SubmitButtonText);
        }
    }
}
