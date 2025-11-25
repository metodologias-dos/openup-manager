using Avalonia.Controls;
using Avalonia.Interactivity;
using OpenUpMan.UI.ViewModels;
using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System.Linq;

namespace OpenUpMan.UI.Views;

public partial class ProjectsPopup : Window
{
    public ProjectsPopup()
    {
        InitializeComponent();
    }

    public ProjectsPopup(ProjectsPopupViewModel vm) : this()
    {
        DataContext = vm;
        vm.CloseRequested += OnCloseRequested;
        vm.LogoutRequested += OnLogoutRequested;
    }

    private void OnCloseRequested()
    {
        this.Close();
    }

    private void OnLogoutRequested()
    {
        // Close this projects window
        this.Close();

        // Reopen the login/main window
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            // Try to find an existing MainWindow
            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime != null)
            {
                // Try to find an existing MainWindow (possibly hidden) and show it
                var main = lifetime.MainWindow as MainWindow;
                if (main != null)
                {
                    try
                    {
                        // Clear login inputs if the MainWindow has the expected DataContext
                        if (main.DataContext is OpenUpMan.UI.ViewModels.MainWindowViewModel mainVm && mainVm.AuthViewModel != null)
                        {
                            mainVm.AuthViewModel.ClearInputs();
                        }

                        main.Show();
                        main.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to show existing MainWindow on logout: " + ex.Message);
                    }

                    // Close this projects window
                    try { this.Close(); } catch (Exception ex) { Console.WriteLine("Failed to close ProjectsPopup on logout: " + ex.Message); }
                    return;
                }

                // If there's no existing main window, create one (DI-backed)
                var loginVm = (Program.ServiceProvider.GetService(typeof(OpenUpMan.UI.ViewModels.MainWindowViewModel)) as OpenUpMan.UI.ViewModels.MainWindowViewModel) ?? new OpenUpMan.UI.ViewModels.MainWindowViewModel();
                var login = new MainWindow
                {
                    DataContext = loginVm
                };
                // Ensure login viewmodel is cleared
                loginVm.AuthViewModel.ClearInputs();
                login.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                lifetime.MainWindow = login;
                try { login.Show(); } catch (Exception ex) { Console.WriteLine("Failed to show login on logout: " + ex.Message); }

                // Now close this projects window
                try { this.Close(); } catch (Exception ex) { Console.WriteLine("Failed to close ProjectsPopup on logout: " + ex.Message); }
                return;
            }
        });
    }
}