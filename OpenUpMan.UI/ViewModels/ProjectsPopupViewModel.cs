﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using OpenUpMan.Domain;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace OpenUpMan.UI.ViewModels;

public partial class ProjectsPopupViewModel : ViewModelBase
{
    // Event to request the window to close
    public event Action? CloseRequested;

    // Event to request a logout (close projects and return to login)
    public event Action? LogoutRequested;

    // Event to request opening the new project dialog
    public event Action? NewProjectDialogRequested;

    public ProjectsPopupViewModel(IEnumerable<ProjectListItemViewModel>? initialProjects = null)
    {
        Projects = new ObservableCollection<ProjectListItemViewModel>();

        // If initial projects are provided (e.g. by tests or a service), populate the collection
        if (initialProjects != null)
        {
            foreach (var p in initialProjects)
                Projects.Add(p);
        }

        NewProjectCommand = new RelayCommand(OnNewProject);
        CloseCommand = new RelayCommand(OnClose);
        OpenProjectCommand = new RelayCommand<ProjectListItemViewModel?>(OnOpenProject);
        DeleteProjectCommand = new RelayCommand<ProjectListItemViewModel?>(OnDeleteProject);

        // logout command
        LogoutCommand = new RelayCommand(OnLogout);
    }

    [ObservableProperty]
    private ObservableCollection<ProjectListItemViewModel> _projects = null!;

    public IRelayCommand NewProjectCommand { get; }
    public IRelayCommand CloseCommand { get; }
    public IRelayCommand<ProjectListItemViewModel?> OpenProjectCommand { get; }
    public IRelayCommand<ProjectListItemViewModel?> DeleteProjectCommand { get; }

    // Command to trigger logout
    public IRelayCommand LogoutCommand { get; }

    private void OnNewProject()
    {
        NewProjectDialogRequested?.Invoke();
    }

    private void OnClose()
    {
        CloseRequested?.Invoke();
    }

    private void OnOpenProject(ProjectListItemViewModel? project)
    {
        if (project == null) return;
        // TODO: open the project in a new Window
    }

    private void OnDeleteProject(ProjectListItemViewModel? project)
    {
        if (project == null) return;
        Projects.Remove(project);
    }

    private void OnLogout()
    {
        LogoutRequested?.Invoke();
    }

    public void AddProject(ProjectDialogResult result)
    {
        var newProject = new ProjectListItemViewModel
        {
            Id = result.Identifier,
            Name = result.Name,
            LastEdited = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
        };
        Projects.Add(newProject);
    }
}

public partial class ProjectListItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _id = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _lastEdited = string.Empty;
}
