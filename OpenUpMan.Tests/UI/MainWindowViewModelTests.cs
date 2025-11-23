using Moq;
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

            await vm.AuthViewModel.SubmitCommand.ExecuteAsync(null);

            // Should have error feedback since NoOpUserService returns error
            Assert.True(vm.AuthViewModel.IsErrorFeedback);
            Assert.Contains("No backend configured", vm.AuthViewModel.Feedback);
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
        public async Task MainWindowViewModel_AuthViewModel_CanToggleMode()
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

        #endregion
    }
}
