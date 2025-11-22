using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenUpMan.Services;

namespace OpenUpMan.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IUserService _userService;

    // Parameterless constructor used by the designer / simple app startup if DI isn't configured.
    public MainWindowViewModel() : this(new NoOpUserService()) { }

    public MainWindowViewModel(IUserService userService)
    {
        _userService = userService;
        CreateUserCommand = new AsyncRelayCommand(CreateUserAsync);
    }

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _feedback = string.Empty;

    public IAsyncRelayCommand CreateUserCommand { get; }

    private async Task CreateUserAsync()
    {
        var result = await _userService.CreateUserAsync(Username, Password);
        Feedback = result.Message;
    }

    public string Greeting { get; } = "Welcome to Avalonia!";

    // Minimal no-op implementation to avoid pulling the data layer into the UI project.
    private class NoOpUserService : IUserService
    {
        public Task<(bool Success, string Message, OpenUpMan.Domain.User? User)> CreateUserAsync(string username, string password, CancellationToken ct = default)
        {
            return Task.FromResult((false, "No backend configured.", (OpenUpMan.Domain.User?)null));
        }

        public Task<(bool Success, string Message, OpenUpMan.Domain.User? User)> AuthenticateAsync(string username, string password, CancellationToken ct = default)
        {
            return Task.FromResult((false, "No backend configured.", (OpenUpMan.Domain.User?)null));
        }
    }
}
