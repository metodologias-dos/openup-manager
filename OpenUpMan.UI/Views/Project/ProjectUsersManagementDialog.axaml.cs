using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.UI.Views
{
  public partial class ProjectUsersManagementDialog : Window
  {
    public ProjectUsersManagementDialog()
    {
      InitializeComponent();
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }

    protected override void OnDataContextChanged(System.EventArgs e)
    {
      base.OnDataContextChanged(e);
      if (DataContext is ProjectUsersManagementViewModel vm)
      {
        vm.CloseRequested += () => Close();
      }
    }
  }
}
