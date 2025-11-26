using Avalonia.Controls;
using OpenUpMan.UI.ViewModels;
using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace OpenUpMan.UI.Views;

public partial class ProjectsPopup : Window
{
    public ProjectsPopup()
    {
        InitializeComponent();
    }

    private bool _isClosingProgrammatically = false;
    
    public ProjectsPopup(ProjectsPopupViewModel vm) : this()
    {
        DataContext = vm;
        vm.CloseRequested += OnCloseRequested;
        vm.LogoutRequested += OnLogoutRequested;
        vm.NewProjectDialogRequested += OnNewProjectDialogRequested;
        
        // Handle window closing (X button) to trigger logout
        this.Closing += OnWindowClosing;
    }
    
    private void OnWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        // If we're already closing programmatically, allow it
        if (_isClosingProgrammatically)
        {
            return;
        }
        
        // Cancel the default close behavior
        e.Cancel = true;
        
        // Trigger logout on the UI thread asynchronously to avoid recursion
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (DataContext is ProjectsPopupViewModel vm)
            {
                OnLogoutRequested();
            }
        });
    }

    private void OnCloseRequested()
    {
        this.Close();
    }

    private async void OnNewProjectDialogRequested()
    {
        if (DataContext is not ProjectsPopupViewModel vm || vm.CurrentUser == null)
            return;

        // Obtener los servicios desde el ServiceProvider
        var projectService = Program.ServiceProvider.GetService(typeof(OpenUpMan.Services.IProjectService)) as OpenUpMan.Services.IProjectService;
        var projectUserService = Program.ServiceProvider.GetService(typeof(OpenUpMan.Services.IProjectUserService)) as OpenUpMan.Services.IProjectUserService;

        if (projectService == null || projectUserService == null)
        {
            Console.WriteLine("Error: No se pudieron obtener los servicios necesarios");
            return;
        }

        var dialogVm = new ProjectDialogViewModel(projectService, projectUserService, vm.CurrentUser.Id);
        var dialog = new ProjectDialog(dialogVm);

        // Suscribirse al evento de proyecto creado
        dialogVm.ProjectCreated += (result) =>
        {
            vm.AddProject(result);
        };

        // Mostrar el diálogo como modal
        await dialog.ShowDialog(this);
    }

    private void OnLogoutRequested()
    {
        // Set flag to prevent infinite recursion
        _isClosingProgrammatically = true;
        
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