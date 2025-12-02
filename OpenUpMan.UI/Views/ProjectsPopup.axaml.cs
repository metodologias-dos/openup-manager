using Avalonia.Controls;
using OpenUpMan.UI.ViewModels;
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;

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
        vm.OpenProjectRequested += OnOpenProjectRequested;

        // Handle window closing (X button) to trigger logout
        this.Closing += OnWindowClosing;

        // Attach header click handlers after the window is loaded
        this.Opened += OnWindowOpened;
    }

    private void OnWindowOpened(object? sender, EventArgs e)
    {
        // Find and attach event handlers to column headers
        AttachHeaderClickHandlers();
    }

    private void AttachHeaderClickHandlers()
    {
        if (DataContext is not ProjectsPopupViewModel vm)
            return;

        // Find all DataGridColumnHeaders in the visual tree
        var headers = this.GetVisualDescendants()
            .OfType<DataGridColumnHeader>()
            .ToList();

        for (int i = 0; i < headers.Count; i++)
        {
            var header = headers[i];

            // Get the TextBlock content from the header
            var textBlock = header.GetVisualDescendants()
                .OfType<TextBlock>()
                .FirstOrDefault();

            if (textBlock == null)
            {
                // Try to get text from Content property directly
                var content = header.Content?.ToString() ?? "";

                if (content.Contains("ACCIONES"))
                    continue; // Skip actions column

                // Fallback: use column index
                if (i == 0) // First column = NOMBRE
                {
                    header.Tapped += (s, e) => vm.SortByNameCommand.Execute(null);
                }
                else if (i == 1) // Second column = ÚLTIMA EDICIÓN
                {
                    header.Tapped += (s, e) => vm.SortByDateCommand.Execute(null);
                }
                continue;
            }

            // Get all text from the TextBlock including Runs
            var allText = string.Empty;
            if (textBlock.Inlines != null)
            {
                foreach (var inline in textBlock.Inlines)
                {
                    if (inline is Avalonia.Controls.Documents.Run run)
                    {
                        allText += run.Text;
                    }
                }
            }

            // If no inlines, try Text property
            if (string.IsNullOrEmpty(allText))
            {
                allText = textBlock.Text ?? "";
            }

            // Check the text content to identify which column this is
            if (allText.Contains("NOMBRE"))
            {
                header.Tapped += (s, e) => vm.SortByNameCommand.Execute(null);
            }
            else if (allText.Contains("ÚLTIMA EDICIÓN"))
            {
                header.Tapped += (s, e) => vm.SortByDateCommand.Execute(null);
            }
            // ACCIONES column gets no handler
        }
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

    private async void OnOpenProjectRequested(string? code)
    {
        if (string.IsNullOrEmpty(code)) return;

        // Try to get project name from service (optional)
        var projectService = Program.ServiceProvider.GetService(typeof(OpenUpMan.Services.IProjectService)) as OpenUpMan.Services.IProjectService;
        string title = code;
        int projectId = 0;

        if (projectService != null)
        {
            try
            {
                var result = await projectService.GetProjectByCodeAsync(code);
                if (result.Success && result.Project != null)
                {
                    title = result.Project.Name ?? code;
                    projectId = result.Project.Id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load project '{code}': {ex.Message}");
            }
        }

        // Create ViewModel and View (visual only)
        var projectVm = new OpenUpMan.UI.ViewModels.ProjectViewModel();
        projectVm.ProjectName = title;
        projectVm.ProjectId = projectId;

        if (DataContext is ProjectsPopupViewModel popupVm && popupVm.CurrentUser != null)
        {
            projectVm.CurrentUserId = popupVm.CurrentUser.Id;
            projectVm.CurrentUserName = popupVm.CurrentUser.Username;
        }

        var view = new OpenUpMan.UI.Views.ProjectView
        {
            DataContext = projectVm
        };

        // Create a window to host the view
        var wnd = new Window
        {
            Title = title,
            Width = 1000,
            Height = 700,
            Content = view,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        // Subscribe to BackRequested event to close project window and show ProjectsPopup again
        projectVm.BackRequested += () =>
        {
            wnd.Close();
            this.Show();
        };

        // Handle window closing to show ProjectsPopup again
        wnd.Closing += (s, e) =>
        {
            this.Show();
        };

        try
        {
            // Show as independent window
            wnd.Show();
            // Hide ProjectsPopup after project window is shown
            this.Hide();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to open project window: " + ex.Message);
            this.Show();
        }
    }
}