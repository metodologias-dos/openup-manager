using Avalonia.Controls;
using OpenUpMan.UI.ViewModels;
using System;

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

    private void OnAuthenticationSucceeded(Domain.User? user)
    {
        // Open the ProjectsPopup window on the UI thread
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            var popupVm = new ProjectsPopupViewModel();
            var popup = new ProjectsPopup(popupVm)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            // Try to find a window to be the owner
            var owner = TopLevel.GetTopLevel(this) as Window;
            if (owner != null)
            {
                popup.Show(owner);
            }
            else
            {
                popup.Show();
            }
        });
    }
}
