using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using Avalonia.Controls;
using OpenUpMan.UI.ViewModels;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace OpenUpMan.UI.Views;

public partial class UserAuthControl : UserControl
{
    private UserAuthViewModel? _subscribedVm;

    public UserAuthControl()
    {
        InitializeComponent();
        this.DataContextChanged += UserAuthControl_DataContextChanged;
    }

    private void UserAuthControl_DataContextChanged(object? sender, EventArgs e)
    {
        // Detach from previous DataContext
        if (_subscribedVm != null)
        {
            _subscribedVm.AuthenticationSucceeded -= OnAuthenticationSucceeded;
            _subscribedVm = null;
        }

        // Attach to new DataContext if it's the expected VM
        if (this.DataContext is UserAuthViewModel newVm)
        {
            _subscribedVm = newVm;
            _subscribedVm.AuthenticationSucceeded += OnAuthenticationSucceeded;
        }
    }

    private void OnAuthenticationSucceeded(OpenUpMan.Domain.User? user)
    {
        // Open the ProjectsPopup window on the UI thread
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            // Obtener los servicios desde el ServiceProvider
            var projectUserService = Program.ServiceProvider.GetService(typeof(OpenUpMan.Services.IProjectUserService)) as OpenUpMan.Services.IProjectUserService;
            var projectService = Program.ServiceProvider.GetService(typeof(OpenUpMan.Services.IProjectService)) as OpenUpMan.Services.IProjectService;

            if (projectUserService == null || projectService == null)
            {
                Console.WriteLine("Error: No se pudieron obtener los servicios de proyectos");
                return;
            }

            // Crear el ViewModel con servicios para cargar proyectos desde la BD
            var popupVm = new ProjectsPopupViewModel(user, projectUserService, projectService);

            // Try to find a window to be the owner
            var owner = TopLevel.GetTopLevel(this) as Window;

            // Try to get the classic desktop lifetime so we can swap MainWindow
            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

            ProjectsPopup? popup = null;
            try
            {
                // Attempt to construct the popup (this can throw if XAML fails to load)
                popup = new ProjectsPopup(popupVm)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };
            }
            catch (Exception ex)
            {
                // If popup can't be created, keep owner open and log to console for debugging
                Console.WriteLine("Failed to create ProjectsPopup: " + ex.Message);
                return;
            }

            if (lifetime != null && popup != null)
            {
                // Instead of swapping MainWindow (which can cause resource/context issues), hide the login window
                if (owner != null)
                {
                    try { owner.Hide(); } catch (Exception ex) { Console.WriteLine("Failed to hide owner: " + ex.Message); }
                }

                try
                {
                    popup.Show();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to show ProjectsPopup: " + ex.Message);
                    // If showing fails, show the owner back
                    try { owner?.Show(); } catch { }
                    try { popup.Close(); } catch { }
                    return;
                }

                // Do NOT close the owner here; keep it hidden so it can be reused as MainWindow later.
            }
            else if (popup != null && owner != null)
            {
                // Fallback: hide owner then show popup
                try { owner.Hide(); } catch (Exception ex) { Console.WriteLine("Failed to hide owner: " + ex.Message); }
                try { popup.Show(); } catch (Exception ex) { Console.WriteLine("Failed to show popup: " + ex.Message); }
            }
            else if (popup != null)
            {
                // No owner and no classic lifetime: just show popup
                try { popup.Show(); } catch (Exception ex) { Console.WriteLine("Failed to show popup: " + ex.Message); }
            }
        });
    }

    // Call this from the Projects logout path to restore the login window
    public static void ShowLoginWindow()
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (lifetime == null) return;

        var login = new MainWindow(); // or the window class you use for login
        lifetime.MainWindow = login;
        login.Show();
    }
}
