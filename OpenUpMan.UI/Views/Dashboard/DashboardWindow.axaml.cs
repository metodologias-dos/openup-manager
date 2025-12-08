using Avalonia.Controls;
using OpenUpMan.Services;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.UI.Views;

public partial class DashboardWindow : Window
{
  public DashboardWindow()
  {
    InitializeComponent();
    DataContext = new DashboardViewModel();
  }

  public DashboardWindow(IDashboardService dashboardService, int projectId)
  {
    InitializeComponent();
    DataContext = new DashboardViewModel(dashboardService, projectId);
  }
}
