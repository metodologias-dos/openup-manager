using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OpenUpMan.Data;
using OpenUpMan.UI.ViewModels;
using System;

namespace OpenUpMan.UI.Views;

public partial class ArtifactsWindow : Window
{
  public ArtifactsWindow()
  {
    InitializeComponent();
  }

  private void InitializeComponent()
  {
    AvaloniaXamlLoader.Load(this);
  }

  protected override void OnDataContextChanged(EventArgs e)
  {
    base.OnDataContextChanged(e);
    if (DataContext is ArtifactsViewModel vm)
    {
      vm.EditArtifactRequested += OpenEditWindow;
    }
  }

  private async void OpenEditWindow(int artifactId, int userId, string userName)
  {
    var artifactRepo = Program.ServiceProvider.GetService<IArtifactRepository>();
    if (artifactRepo == null) return;

    var editVm = new ArtifactEditViewModel(artifactRepo, artifactId, userId, userName);
    await editVm.LoadAsync();

    var editWindow = new ArtifactEditWindow
    {
      DataContext = editVm
    };

    await editWindow.ShowDialog(this);

    // Refresh list after edit
    if (DataContext is ArtifactsViewModel vm)
    {
      await vm.RefreshAsync();
    }
  }

  public void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
  {
    Close();
  }
}
