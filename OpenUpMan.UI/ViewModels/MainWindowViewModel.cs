using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
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
        // Initialize the reusable auth VM
        AuthViewModel = new UserAuthViewModel(_userService);
    }

    // Expose the reusable auth view model for the control
    [ObservableProperty]
    private UserAuthViewModel _authViewModel;

    // Minimal no-op implementation to avoid pulling the data layer into the UI project.
    private sealed class NoOpUserService : IUserService
    {
        public Task<ServiceResult> CreateUserAsync(string username, string password, CancellationToken ct = default)
        {
            return Task.FromResult(new ServiceResult(
                Success: false,
                ResultType: ServiceResultType.Error,
                Message: "No backend configured.",
                User: null
            ));
        }

        public Task<ServiceResult> AuthenticateAsync(string username, string password, CancellationToken ct = default)
        {
            return Task.FromResult(new ServiceResult(
                Success: false,
                ResultType: ServiceResultType.Error,
                Message: "No backend configured.",
                User: null
            ));
        }
    }
}
