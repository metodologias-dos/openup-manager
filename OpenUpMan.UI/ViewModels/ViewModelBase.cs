using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenUpMan.UI.ViewModels;

/// <summary>
/// Base class for all ViewModels. Marked as abstract to prevent direct instantiation
/// and enforce that all ViewModels are properly derived types.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
}
