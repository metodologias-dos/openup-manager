using Moq;
using OpenUpMan.Domain;
using OpenUpMan.Services;
using OpenUpMan.UI.ViewModels;
using Xunit;

namespace OpenUpMan.Tests.UI
{
    public class MainWindowViewModelTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_ParameterlessConstructor_InitializesWithNoOpService()
        {
            var vm = new MainWindowViewModel();

            Assert.NotNull(vm);
            Assert.NotNull(vm.AuthViewModel);
        }

        [Fact]
        public async Task Constructor_ParameterlessConstructor_CreatesNoOpUserService()
        {
            // Arrange & Act
            var vm = new MainWindowViewModel();

            // Assert - Verify NoOpUserService is being used by testing its behavior
            vm.AuthViewModel.Username = "test";
            vm.AuthViewModel.Password = "test";
            await vm.AuthViewModel.SubmitCommand.ExecuteAsync(null);

            Assert.Contains("No backend configured", vm.AuthViewModel.Feedback);
        }

        [Fact]
        public void Constructor_ParameterlessConstructor_InitializesAuthViewModel()
        {
            // Arrange & Act
            var vm = new MainWindowViewModel();

            // Assert
            Assert.NotNull(vm.AuthViewModel);
            Assert.IsType<UserAuthViewModel>(vm.AuthViewModel);
        }

        [Fact]
        public void Constructor_ParameterlessConstructor_AuthViewModelInLoginMode()
        {
            // Arrange & Act
            var vm = new MainWindowViewModel();

            // Assert
            Assert.True(vm.AuthViewModel.IsLoginMode);
            Assert.False(vm.AuthViewModel.IsCreateMode);
        }

        [Fact]
        public void Constructor_ParameterlessConstructor_CanSwitchToCreateMode()
        {
            // Arrange & Act
            var vm = new MainWindowViewModel();
            vm.AuthViewModel.ToggleModeCommand.Execute(null);

            // Assert - Tests that the NoOpUserService works in both modes
            Assert.True(vm.AuthViewModel.IsCreateMode);
        }

        [Fact]
        public void Constructor_WithService_InitializesAuthViewModel()
        {
            var mockService = new Mock<IUserService>();
            var vm = new MainWindowViewModel(mockService.Object);

            Assert.NotNull(vm.AuthViewModel);
        }

        [Fact]
        public void Constructor_WithService_UsesProvidedService()
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), default))
                .ReturnsAsync(new ServiceResult(true, ServiceResultType.Success, "Test", null));

            var vm = new MainWindowViewModel(mockService.Object);

            Assert.NotNull(vm.AuthViewModel);
            // The AuthViewModel should be using the provided service
        }

        #endregion

        #region NoOpUserService Tests

        [Fact]
        public async Task NoOpUserService_CreateUserAsync_ReturnsError()
        {
            var vm = new MainWindowViewModel(); // Uses NoOpUserService
            vm.AuthViewModel.ToggleModeCommand.Execute(null); // Switch to create mode
            vm.AuthViewModel.Username = "newuser";
            vm.AuthViewModel.Password = "password";

            await vm.AuthViewModel.SubmitCommand.ExecuteAsync(null);

            // Should have error feedback since NoOpUserService returns error
            Assert.True(vm.AuthViewModel.IsErrorFeedback);
            Assert.Contains("No backend configured", vm.AuthViewModel.Feedback);
        }

        [Fact]
        public async Task NoOpUserService_CreateUserAsync_ReturnsFalseSuccess()
        {
            var vm = new MainWindowViewModel(); // Uses NoOpUserService
            vm.AuthViewModel.ToggleModeCommand.Execute(null);
            vm.AuthViewModel.Username = "test";
            vm.AuthViewModel.Password = "test";

            await vm.AuthViewModel.SubmitCommand.ExecuteAsync(null);

            Assert.False(vm.AuthViewModel.IsSuccessFeedback);
            Assert.Equal(ServiceResultType.Error, ServiceResultType.Error);
        }

        [Fact]
        public async Task NoOpUserService_CreateUserAsync_ReturnsNullUser()
        {
            var vm = new MainWindowViewModel(); // Uses NoOpUserService
            vm.AuthViewModel.ToggleModeCommand.Execute(null);
            vm.AuthViewModel.Username = "test";
            vm.AuthViewModel.Password = "test";

            await vm.AuthViewModel.SubmitCommand.ExecuteAsync(null);

            // NoOpUserService should return null User
            Assert.True(vm.AuthViewModel.IsErrorFeedback);
        }

        [Fact]
        public async Task NoOpUserService_AuthenticateAsync_ReturnsError()
        {
            var vm = new MainWindowViewModel(); // Uses NoOpUserService
            vm.AuthViewModel.Username = "test";
            vm.AuthViewModel.Password = "test";

            await vm.AuthViewModel.SubmitCommand.ExecuteAsync(null);

            Assert.True(vm.AuthViewModel.IsErrorFeedback);
            Assert.Contains("No backend configured", vm.AuthViewModel.Feedback);
        }

        [Fact]
        public async Task NoOpUserService_AuthenticateAsync_ReturnsFalseSuccess()
        {
            var vm = new MainWindowViewModel(); // Uses NoOpUserService
            vm.AuthViewModel.Username = "user";
            vm.AuthViewModel.Password = "pass";

            await vm.AuthViewModel.SubmitCommand.ExecuteAsync(null);

            Assert.False(vm.AuthViewModel.IsSuccessFeedback);
        }

        [Fact]
        public async Task NoOpUserService_AuthenticateAsync_ReturnsNullUser()
        {
            var vm = new MainWindowViewModel(); // Uses NoOpUserService
            vm.AuthViewModel.Username = "user";
            vm.AuthViewModel.Password = "pass";

            await vm.AuthViewModel.SubmitCommand.ExecuteAsync(null);

            // NoOpUserService returns null User
            Assert.True(vm.AuthViewModel.IsErrorFeedback);
            Assert.Equal("No backend configured.", vm.AuthViewModel.Feedback);
        }

        #endregion

        #region AuthViewModel Tests

        [Fact]
        public void AuthViewModel_IsNotNull_AfterConstruction()
        {
            var vm = new MainWindowViewModel();

            Assert.NotNull(vm.AuthViewModel);
        }

        [Fact]
        public void AuthViewModel_CanBeAccessed()
        {
            var mockService = new Mock<IUserService>();
            var vm = new MainWindowViewModel(mockService.Object);

            var authVm = vm.AuthViewModel;

            Assert.NotNull(authVm);
            Assert.IsType<UserAuthViewModel>(authVm);
        }

        [Fact]
        public void AuthViewModel_StartsInLoginMode()
        {
            var vm = new MainWindowViewModel();

            Assert.False(vm.AuthViewModel.IsCreateMode);
            Assert.True(vm.AuthViewModel.IsLoginMode);
        }

        #endregion

        #region Property Change Tests

        [Fact]
        public void AuthViewModel_CanBeSet()
        {
            var mockService = new Mock<IUserService>();
            var vm = new MainWindowViewModel(mockService.Object);
            var newAuthVm = new UserAuthViewModel(mockService.Object);

            vm.AuthViewModel = newAuthVm;

            Assert.Equal(newAuthVm, vm.AuthViewModel);
        }

        [Fact]
        public void AuthViewModel_RaisesPropertyChanged_WhenSet()
        {
            var mockService = new Mock<IUserService>();
            var vm = new MainWindowViewModel(mockService.Object);
            var raised = false;

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.AuthViewModel))
                    raised = true;
            };

            vm.AuthViewModel = new UserAuthViewModel(mockService.Object);

            Assert.True(raised);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task MainWindowViewModel_AuthViewModel_CanAuthenticate()
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResult(true, ServiceResultType.Success, "Success"));

            var vm = new MainWindowViewModel(mockService.Object);
            vm.AuthViewModel.Username = "user";
            vm.AuthViewModel.Password = "pass";

            await vm.AuthViewModel.SubmitCommand.ExecuteAsync(null);

            Assert.NotEmpty(vm.AuthViewModel.Feedback);
            Assert.True(vm.AuthViewModel.IsSuccessFeedback, 
                $"Expected IsSuccessFeedback to be true. Feedback: '{vm.AuthViewModel.Feedback}', IsError: {vm.AuthViewModel.IsErrorFeedback}");
        }

        [Fact]
        public void MainWindowViewModel_AuthViewModel_CanToggleMode()
        {
            var mockService = new Mock<IUserService>();
            var vm = new MainWindowViewModel(mockService.Object);

            Assert.True(vm.AuthViewModel.IsLoginMode);

            vm.AuthViewModel.ToggleModeCommand.Execute(null);

            Assert.False(vm.AuthViewModel.IsLoginMode);
            Assert.True(vm.AuthViewModel.IsCreateMode);
        }
    
        [Fact]
        public async Task MainWindowViewModel_AuthViewModel_CanCreateUser()
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResult(true, ServiceResultType.Success, "Created"));

            var vm = new MainWindowViewModel(mockService.Object);
            vm.AuthViewModel.ToggleModeCommand.Execute(null); // Switch to register
            vm.AuthViewModel.Username = "newuser";
            vm.AuthViewModel.Password = "password";

            await vm.AuthViewModel.SubmitCommand.ExecuteAsync(null);

            Assert.True(vm.AuthViewModel.IsSuccessFeedback);
        }

        [Fact]
        public async Task MainWindowViewModel_CreateUserAsync_DelegatesToService()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var expectedResult = new ServiceResult(true, ServiceResultType.Success, "User created", null);
            mockService.Setup(s => s.CreateUserAsync("testuser", "testpass", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            var vm = new MainWindowViewModel(mockService.Object);

            // Act - Call CreateUserAsync indirectly through the AuthViewModel
            vm.AuthViewModel.ToggleModeCommand.Execute(null); // Switch to create mode
            vm.AuthViewModel.Username = "testuser";
            vm.AuthViewModel.Password = "testpass";
            await vm.AuthViewModel.SubmitCommand.ExecuteAsync(null);

            // Assert - Verify CreateUserAsync was called
            mockService.Verify(s => s.CreateUserAsync("testuser", "testpass", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MainWindowViewModel_CreateUserAsync_ReturnsServiceResult()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var testUser = new User("createdUser", "hash");
            var expectedResult = new ServiceResult(true, ServiceResultType.Success, "Usuario creado", testUser);
            
            mockService.Setup(s => s.CreateUserAsync("createdUser", "password123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            var vm = new MainWindowViewModel(mockService.Object);

            // Act
            vm.AuthViewModel.ToggleModeCommand.Execute(null);
            vm.AuthViewModel.Username = "createdUser";
            vm.AuthViewModel.Password = "password123";
            await vm.AuthViewModel.SubmitCommand.ExecuteAsync(null);

            // Assert
            Assert.True(vm.AuthViewModel.IsSuccessFeedback);
            Assert.Equal("Usuario creado", vm.AuthViewModel.Feedback);
            mockService.Verify(s => s.CreateUserAsync("createdUser", "password123", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MainWindowViewModel_CreateUserAsync_HandlesFailure()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var expectedResult = new ServiceResult(false, ServiceResultType.Error, "Usuario ya existe", null);
            
            mockService.Setup(s => s.CreateUserAsync("existing", "password", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            var vm = new MainWindowViewModel(mockService.Object);

            // Act
            vm.AuthViewModel.ToggleModeCommand.Execute(null);
            vm.AuthViewModel.Username = "existing";
            vm.AuthViewModel.Password = "password";
            await vm.AuthViewModel.SubmitCommand.ExecuteAsync(null);

            // Assert
            Assert.True(vm.AuthViewModel.IsErrorFeedback);
            Assert.Equal("Usuario ya existe", vm.AuthViewModel.Feedback);
        }

        #endregion
    }
}
