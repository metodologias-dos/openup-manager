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
                var main = lifetime.MainWindow as MainWindow;
                if (main != null)
                {
                    try
                    {
                        main.Show();
                        main.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        return;
                    }
                    catch { }
                }
            }

            var login = new MainWindow();
            login.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            login.Show();
        });
    }
}
