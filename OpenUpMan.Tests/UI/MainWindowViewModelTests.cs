using Moq;
using OpenUpMan.Services;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.Tests.UI;

public class MainWindowViewModelTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly MainWindowViewModel _viewModel;

    public MainWindowViewModelTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _viewModel = new MainWindowViewModel(_userServiceMock.Object);
    }

    [Fact]
    public void Constructor_InitializesAuthViewModel()
    {
        // Assert
        Assert.NotNull(_viewModel.AuthViewModel);
    }
}
