using Avalonia.Controls;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.UI.Views;

public partial class DashboardWindow : Window
{
  public DashboardWindow()
  {
    InitializeComponent();
    DataContext = new DashboardViewModel();
  }
}
