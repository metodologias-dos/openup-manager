using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenUpMan.Domain;
using OpenUpMan.Services;

namespace OpenUpMan.UI.ViewModels
{
  public partial class ProjectUsersManagementViewModel : ViewModelBase
  {
    private readonly IProjectUserService _projectUserService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly int _projectId;

    [ObservableProperty]
    private ObservableCollection<ProjectUserItemViewModel> _projectUsers = new();

    [ObservableProperty]
    private string _userSearchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<User> _userSearchResults = new();

    [ObservableProperty]
    private User? _selectedSearchUser;

    private List<ProjectUserItemViewModel> _deletedUsers = new();
    private IEnumerable<Role> _allRoles = Enumerable.Empty<Role>();

    public event Action? CloseRequested;

    public IRelayCommand<ProjectUserItemViewModel> RemoveUserCommand { get; }
    public IAsyncRelayCommand SaveUsersCommand { get; }

    public ProjectUsersManagementViewModel(
        int projectId,
        IProjectUserService projectUserService,
        IUserService userService,
        IRoleService roleService)
    {
      _projectId = projectId;
      _projectUserService = projectUserService;
      _userService = userService;
      _roleService = roleService;

      RemoveUserCommand = new RelayCommand<ProjectUserItemViewModel>(OnRemoveUser);
      SaveUsersCommand = new AsyncRelayCommand(OnSaveUsers);

      _ = LoadProjectUsers();
    }

    private void OnRemoveUser(ProjectUserItemViewModel? userItem)
    {
      if (userItem != null)
      {
        if (userItem.IsNew)
        {
          ProjectUsers.Remove(userItem);
        }
        else
        {
          userItem.IsDeleted = true;
          _deletedUsers.Add(userItem);
          ProjectUsers.Remove(userItem);
        }
      }
    }

    private async Task OnSaveUsers()
    {
      try
      {
        // Procesar usuarios activos (Nuevos o Modificados)
        foreach (var user in ProjectUsers)
        {
          if (user.IsNew)
          {
            await _projectUserService.AddUserToProjectAsync(_projectId, user.UserId, user.SelectedRole.Id);
            user.IsNew = false;
          }
          else if (user.IsModified)
          {
            await _projectUserService.ChangeUserRoleAsync(_projectId, user.UserId, user.SelectedRole.Id);
            user.IsModified = false;
          }
        }

        // Procesar usuarios eliminados
        foreach (var user in _deletedUsers)
        {
          await _projectUserService.RemoveUserFromProjectAsync(_projectId, user.UserId);
        }

        _deletedUsers.Clear();

        // Cerrar el diálogo o notificar éxito
        CloseRequested?.Invoke();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    public async Task LoadProjectUsers()
    {
      try
      {
        _deletedUsers.Clear();

        var rolesResult = await _roleService.GetAllRolesAsync();
        _allRoles = (rolesResult.Success && rolesResult.Data != null) ? rolesResult.Data : Enumerable.Empty<Role>();

        var details = await _projectUserService.GetProjectUsersDetailsAsync(_projectId);

        ProjectUsers.Clear();
        foreach (var detail in details)
        {
          ProjectUsers.Add(new ProjectUserItemViewModel(detail.User, detail.Role, _allRoles, detail.ProjectUser.Id));
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    partial void OnUserSearchTextChanged(string value)
    {
      SearchUsers(value);
    }

    private async void SearchUsers(string term)
    {
      if (string.IsNullOrWhiteSpace(term))
      {
        UserSearchResults.Clear();
        return;
      }

      var result = await _userService.SearchUsersAsync(term);
      if (result.Success && result.Data != null)
      {
        UserSearchResults.Clear();
        foreach (var user in result.Data)
        {
          if (!ProjectUsers.Any(pu => pu.UserId == user.Id && !pu.IsDeleted))
          {
            UserSearchResults.Add(user);
          }
        }
      }
    }

    [RelayCommand]
    private void AddUserFromSearch(User? user)
    {
      if (user == null) return;

      var defaultRole = _allRoles.FirstOrDefault();

      if (defaultRole != null)
      {
        ProjectUsers.Add(new ProjectUserItemViewModel(user, defaultRole, _allRoles, isNew: true));

        // Reset search
        SelectedSearchUser = null;
        UserSearchText = string.Empty;
      }
    }
  }
}
