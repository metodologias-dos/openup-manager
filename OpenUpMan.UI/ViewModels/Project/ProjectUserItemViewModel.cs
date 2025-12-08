using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenUpMan.Domain;
using OpenUpMan.Services;

namespace OpenUpMan.UI.ViewModels
{
  public partial class ProjectUserItemViewModel : ViewModelBase
  {
    public int UserId { get; }
    public string Username { get; }

    // If it's an existing ProjectUser, we have an ID. If new, it might be 0.
    public int ProjectUserId { get; }

    [ObservableProperty]
    private Role _selectedRole;

    [ObservableProperty]
    private bool _isDeleted;

    [ObservableProperty]
    private bool _isNew;

    [ObservableProperty]
    private bool _isModified;

    public List<Role> AvailableRoles { get; }

    public ProjectUserItemViewModel(User user, Role currentRole, IEnumerable<Role> availableRoles, int projectUserId = 0, bool isNew = false)
    {
      UserId = user.Id;
      Username = user.Username;
      ProjectUserId = projectUserId;
      AvailableRoles = availableRoles.ToList();
      _selectedRole = AvailableRoles.FirstOrDefault(r => r.Id == currentRole.Id) ?? currentRole;
      _isNew = isNew;

      // Listen to changes
      PropertyChanged += (s, e) =>
      {
        if (e.PropertyName == nameof(SelectedRole) && !IsNew)
        {
          IsModified = true;
        }
      };
    }
  }
}
