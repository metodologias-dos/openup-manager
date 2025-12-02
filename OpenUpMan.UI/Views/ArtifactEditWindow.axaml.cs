using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using OpenUpMan.UI.ViewModels;
using System;

namespace OpenUpMan.UI.Views;

public partial class ArtifactEditWindow : Window
{
  public ArtifactEditWindow()
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
    if (DataContext is ArtifactEditViewModel vm)
    {
      vm.CloseRequested += Close;
      vm.BrowseFileRequested += BrowseFile;
    }
  }

  private async void BrowseFile()
  {
    if (DataContext is not ArtifactEditViewModel vm) return;

    var topLevel = TopLevel.GetTopLevel(this);
    if (topLevel == null) return;

    var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
    {
      Title = "Seleccionar archivo",
      AllowMultiple = false
    });

    if (files.Count > 0)
    {
      var file = files[0];
      // Get path if possible (Desktop)
      if (file.TryGetLocalPath() is string path)
      {
        vm.SetFilePath(path);
      }
      else
      {
        // Handle non-local paths if needed, for now just name
        vm.SetFilePath(file.Name);
      }
    }
  }
}
