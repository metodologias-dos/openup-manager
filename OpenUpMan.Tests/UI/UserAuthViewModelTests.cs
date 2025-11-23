using Moq;
using OpenUpMan.Domain;
using OpenUpMan.Services;
using OpenUpMan.UI.ViewModels;
using Xunit;

namespace OpenUpMan.Tests.UI
{
    public class UserAuthViewModelTests
    {
        #region Initialization Tests

        [Fact]
        public void Constructor_InitializesInLoginMode()
        {
            var mockService = new Mock<IUserService>();
            var vm = new UserAuthViewModel(mockService.Object);

            Assert.False(vm.IsCreateMode);
            Assert.True(vm.IsLoginMode);
            Assert.Equal("Inicio de sesión", vm.HeaderText);
            Assert.Equal("Iniciar sesión", vm.SubmitButtonText);
            Assert.Equal("No tengo cuenta", vm.ToggleButtonText);
        }

        [Fact]
        public void Constructor_InitializesEmptyInputs()
        {
            var mockService = new Mock<IUserService>();
            var vm = new UserAuthViewModel(mockService.Object);

            Assert.Equal(string.Empty, vm.Username);
            Assert.Equal(string.Empty, vm.Password);
            Assert.Equal(string.Empty, vm.Feedback);
        }

        [Fact]
        public void Constructor_InitializesFeedbackFlags()
        {
            var mockService = new Mock<IUserService>();
            var vm = new UserAuthViewModel(mockService.Object);

            Assert.False(vm.IsSuccessFeedback);
            Assert.False(vm.IsErrorFeedback);
            Assert.False(vm.IsNeutralFeedback);
        }

        #endregion

        #region Toggle Mode Tests

        [Fact]
        public void ToggleMode_SwitchesToRegisterMode()
        {
            var mockService = new Mock<IUserService>();
            var vm = new UserAuthViewModel(mockService.Object);

            vm.ToggleModeCommand.Execute(null);

            Assert.True(vm.IsCreateMode);
            Assert.False(vm.IsLoginMode);
            Assert.Equal("Crear Cuenta", vm.HeaderText);
            Assert.Equal("Crear cuenta", vm.SubmitButtonText);
            Assert.Equal("Ya tengo cuenta", vm.ToggleButtonText);
        }

        [Fact]
        public void ToggleMode_SwitchesBackToLoginMode()
        {
            var mockService = new Mock<IUserService>();
            var vm = new UserAuthViewModel(mockService.Object);

            vm.ToggleModeCommand.Execute(null);
            vm.ToggleModeCommand.Execute(null);

            Assert.False(vm.IsCreateMode);
            Assert.True(vm.IsLoginMode);
            Assert.Equal("Inicio de sesión", vm.HeaderText);
        }

        [Fact]
        public void ToggleMode_ClearsUsernameInput()
        {
            var mockService = new Mock<IUserService>();
            var vm = new UserAuthViewModel(mockService.Object)
            {
                Username = "testuser"
            };

            vm.ToggleModeCommand.Execute(null);

            Assert.Equal(string.Empty, vm.Username);
        }

        [Fact]
        public void ToggleMode_ClearsPasswordInput()
        {
            var mockService = new Mock<IUserService>();
            var vm = new UserAuthViewModel(mockService.Object)
            {
                Password = "password123"
            };

            vm.ToggleModeCommand.Execute(null);

            Assert.Equal(string.Empty, vm.Password);
        }

        [Fact]
        public void ToggleMode_ClearsFeedbackMessage()
        {
            var mockService = new Mock<IUserService>();
            var vm = new UserAuthViewModel(mockService.Object);
            
            // Manually set feedback
            typeof(UserAuthViewModel)
                .GetProperty(nameof(vm.Feedback))!
                .SetValue(vm, "Some error message");

            vm.ToggleModeCommand.Execute(null);

            Assert.Equal(string.Empty, vm.Feedback);
        }

        [Fact]
        public void ToggleMode_ResetsFeedbackFlags()
        {
            var mockService = new Mock<IUserService>();
            var vm = new UserAuthViewModel(mockService.Object);
            
            // Set feedback flags
            typeof(UserAuthViewModel)
                .GetProperty(nameof(vm.IsSuccessFeedback))!
                .SetValue(vm, true);
            typeof(UserAuthViewModel)
                .GetProperty(nameof(vm.IsErrorFeedback))!
                .SetValue(vm, true);

            vm.ToggleModeCommand.Execute(null);

            Assert.False(vm.IsSuccessFeedback);
            Assert.False(vm.IsErrorFeedback);
            Assert.False(vm.IsNeutralFeedback);
        }

        #endregion

        #region Submit Command - Login Tests

        [Fact]
        public async Task SubmitCommand_CallsAuthenticateAsync_InLoginMode()
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.AuthenticateAsync("user", "pass", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResult(true, ServiceResultType.Success, "Success"));

            var vm = new UserAuthViewModel(mockService.Object)
            {
                Username = "user",
                Password = "pass"
            };

            await vm.SubmitCommand.ExecuteAsync(null);

            mockService.Verify(s => s.AuthenticateAsync("user", "pass", It.IsAny<CancellationToken>()), Times.Once);
            mockService.Verify(s => s.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SubmitCommand_SetsSuccessFeedback_OnSuccessfulLogin()
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResult(true, ServiceResultType.Success, "Autenticación exitosa"));

            var vm = new UserAuthViewModel(mockService.Object)
            {
                Username = "user",
                Password = "pass"
            };

            await vm.SubmitCommand.ExecuteAsync(null);

            Assert.True(vm.IsSuccessFeedback);
            Assert.False(vm.IsErrorFeedback);
            Assert.Equal("Autenticación exitosa", vm.Feedback);
        }

        [Fact]
        public async Task SubmitCommand_SetsErrorFeedback_OnFailedLogin()
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResult(false, ServiceResultType.Error, "Credenciales inválidas"));

            var vm = new UserAuthViewModel(mockService.Object)
            {
                Username = "user",
                Password = "wrongpass"
            };

            await vm.SubmitCommand.ExecuteAsync(null);

            Assert.False(vm.IsSuccessFeedback);
            Assert.True(vm.IsErrorFeedback);
            Assert.Equal("Credenciales inválidas", vm.Feedback);
        }

        #endregion

        #region Submit Command - Register Tests

        [Fact]
        public async Task SubmitCommand_CallsCreateUserAsync_InRegisterMode()
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.CreateUserAsync("newuser", "password", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResult(true, ServiceResultType.Success, "Usuario creado"));

            var vm = new UserAuthViewModel(mockService.Object);
            vm.ToggleModeCommand.Execute(null); // Switch to register mode
            vm.Username = "newuser";
            vm.Password = "password";

            await vm.SubmitCommand.ExecuteAsync(null);

            mockService.Verify(s => s.CreateUserAsync("newuser", "password", It.IsAny<CancellationToken>()), Times.Once);
            mockService.Verify(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SubmitCommand_SetsSuccessFeedback_OnSuccessfulRegistration()
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResult(true, ServiceResultType.Success, "Usuario creado exitosamente"));

            var vm = new UserAuthViewModel(mockService.Object);
            vm.ToggleModeCommand.Execute(null);
            vm.Username = "newuser";
            vm.Password = "password";

            await vm.SubmitCommand.ExecuteAsync(null);

            Assert.True(vm.IsSuccessFeedback);
            Assert.False(vm.IsErrorFeedback);
            Assert.Equal("Usuario creado exitosamente", vm.Feedback);
        }

        [Fact]
        public async Task SubmitCommand_SetsErrorFeedback_OnFailedRegistration()
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResult(false, ServiceResultType.Error, "Usuario ya existe"));

            var vm = new UserAuthViewModel(mockService.Object);
            vm.ToggleModeCommand.Execute(null);
            vm.Username = "existinguser";
            vm.Password = "password";

            await vm.SubmitCommand.ExecuteAsync(null);

            Assert.False(vm.IsSuccessFeedback);
            Assert.True(vm.IsErrorFeedback);
            Assert.Equal("Usuario ya existe", vm.Feedback);
        }

        #endregion

        #region Input Validation Edge Cases

        [Theory]
        [InlineData("", "password")]
        [InlineData("user", "")]
        [InlineData("", "")]
        public async Task SubmitCommand_HandlesEmptyInputs(string username, string password)
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResult(false, ServiceResultType.Error, "Credenciales inválidas"));

            var vm = new UserAuthViewModel(mockService.Object)
            {
                Username = username,
                Password = password
            };

            await vm.SubmitCommand.ExecuteAsync(null);

            Assert.True(vm.IsErrorFeedback);
        }

        [Fact]
        public async Task SubmitCommand_HandlesLongUsername()
        {
            var longUsername = new string('a', 1000);
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.AuthenticateAsync(longUsername, "password", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResult(false, ServiceResultType.Error, "Error"));

            var vm = new UserAuthViewModel(mockService.Object)
            {
                Username = longUsername,
                Password = "password"
            };

            await vm.SubmitCommand.ExecuteAsync(null);

            // Should not crash
            Assert.NotNull(vm.Feedback);
        }

        [Fact]
        public async Task SubmitCommand_HandlesSpecialCharactersInUsername()
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.AuthenticateAsync("user@example.com", "pass", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResult(true, ServiceResultType.Success, "Success"));

            var vm = new UserAuthViewModel(mockService.Object)
            {
                Username = "user@example.com",
                Password = "pass"
            };

            await vm.SubmitCommand.ExecuteAsync(null);

            mockService.Verify(s => s.AuthenticateAsync("user@example.com", "pass", It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Property Change Notifications

        [Fact]
        public void Username_RaisesPropertyChanged()
        {
            var mockService = new Mock<IUserService>();
            var vm = new UserAuthViewModel(mockService.Object);
            var raised = false;

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.Username))
                    raised = true;
            };

            vm.Username = "testuser";

            Assert.True(raised);
        }

        [Fact]
        public void Password_RaisesPropertyChanged()
        {
            var mockService = new Mock<IUserService>();
            var vm = new UserAuthViewModel(mockService.Object);
            var raised = false;

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.Password))
                    raised = true;
            };

            vm.Password = "testpass";

            Assert.True(raised);
        }

        [Fact]
        public void IsLoginMode_RaisesPropertyChanged_OnToggle()
        {
            var mockService = new Mock<IUserService>();
            var vm = new UserAuthViewModel(mockService.Object);
            var raised = false;

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.IsLoginMode))
                    raised = true;
            };

            vm.ToggleModeCommand.Execute(null);

            Assert.True(raised);
        }

        #endregion

        #region Concurrent Execution Tests

        [Fact]
        public async Task SubmitCommand_HandlesMultipleSimultaneousCalls()
        {
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(async () =>
                {
                    await Task.Delay(100);
                    return new ServiceResult(true, ServiceResultType.Success, "Success");
                });

            var vm = new UserAuthViewModel(mockService.Object)
            {
                Username = "user",
                Password = "pass"
            };

            var task1 = vm.SubmitCommand.ExecuteAsync(null);
            var task2 = vm.SubmitCommand.ExecuteAsync(null);
            var task3 = vm.SubmitCommand.ExecuteAsync(null);

            await Task.WhenAll(task1, task2, task3);

            // All calls should complete without crashing
            Assert.NotNull(vm.Feedback);
        }

        #endregion
    }
}

