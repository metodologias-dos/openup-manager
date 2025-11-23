using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenUpMan.Services;

namespace OpenUpMan.UI.ViewModels;

public partial class UserAuthViewModel : ViewModelBase
{
    private readonly IUserService _userService;

    public UserAuthViewModel(IUserService userService)
    {
        _userService = userService;
        SubmitCommand = new AsyncRelayCommand(SubmitAsync);
        ToggleModeCommand = new RelayCommand(ToggleMode);

        // Default to login mode
        IsCreateMode = false;

        // Initialize texts
        HeaderText = "Inicio de sesión";
        SubmitButtonText = "Iniciar sesión";
        ToggleButtonText = "No tengo cuenta";
    }

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _feedback = string.Empty;

    [ObservableProperty]
    private bool _isSuccessFeedback;

    [ObservableProperty]
    private bool _isErrorFeedback;

    [ObservableProperty]
    private bool _isNeutralFeedback;

    [ObservableProperty]
    private bool _isCreateMode;

    public bool IsLoginMode => !IsCreateMode;

    [ObservableProperty]
    private string _submitButtonText = string.Empty;

    [ObservableProperty]
    private string _toggleButtonText = string.Empty;

    [ObservableProperty]
    private string _headerText = string.Empty;

    public IAsyncRelayCommand SubmitCommand { get; }
    public IRelayCommand ToggleModeCommand { get; }

    partial void OnIsCreateModeChanged(bool value)
    {
        if (value)
        {
            SubmitButtonText = "Crear cuenta";
            ToggleButtonText = "Ya tengo cuenta";
            HeaderText = "Crear Cuenta";
        }
        else
        {
            SubmitButtonText = "Iniciar sesión";
            ToggleButtonText = "No tengo cuenta";
            HeaderText = "Inicio de sesión";
        }
        
        // Clear inputs
        Username = string.Empty;
        Password = string.Empty;
        
        // Reset feedback
        Feedback = string.Empty;
        IsSuccessFeedback = false;
        IsErrorFeedback = false;
        IsNeutralFeedback = false;
        
        OnPropertyChanged(nameof(IsLoginMode));
    }

    private void ToggleMode()
    {
        IsCreateMode = !IsCreateMode;
    }

    private async Task SubmitAsync(CancellationToken ct = default)
    {
        ServiceResult result;
        
        if (IsCreateMode)
        {
            result = await _userService.CreateUserAsync(Username, Password, ct);
        }
        else
        {
            result = await _userService.AuthenticateAsync(Username, Password, ct);
        }
        
        Feedback = result.Message;
        
        // Set the feedback visibility flags based on result type
        IsSuccessFeedback = result.ResultType == ServiceResultType.Success;
        IsErrorFeedback = result.ResultType == ServiceResultType.Error;
        IsNeutralFeedback = false; // We always have success or error
    }
}
