using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OpenUpMan.UI.Views;

public partial class ProjectView : UserControl
{
    public ProjectView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // Optional: allow code-behind interactions in future
}
