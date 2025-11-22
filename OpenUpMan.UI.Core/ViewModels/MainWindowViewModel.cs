using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenUpMan.Services;

namespace OpenUpMan.UI.Core.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IUserService _userService;

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
}

