using Xunit;
using OpenUpMan.UI.ViewModels;
using System.Linq;

namespace OpenUpMan.Tests.UI
{
    public class ProjectsPopupViewModelTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_InitializesProjectsCollection()
        {
            var vm = new ProjectsPopupViewModel();

            Assert.NotNull(vm.Projects);
            Assert.IsType<System.Collections.ObjectModel.ObservableCollection<ProjectListItemViewModel>>(vm.Projects);
        }

        [Fact]
        public void Constructor_LoadsSampleProjects()
        {
            var vm = new ProjectsPopupViewModel();

            Assert.Equal(5, vm.Projects.Count);
        }

        [Fact]
        public void Constructor_InitializesCommands()
        {
            var vm = new ProjectsPopupViewModel();

            Assert.NotNull(vm.NewProjectCommand);
            Assert.NotNull(vm.CloseCommand);
            Assert.NotNull(vm.OpenProjectCommand);
            Assert.NotNull(vm.DeleteProjectCommand);
        }

        [Fact]
        public void Constructor_LoadsProjectWithCorrectData()
        {
            var vm = new ProjectsPopupViewModel();

            var firstProject = vm.Projects.First();
            Assert.Equal("PRJ-001", firstProject.Id);
            Assert.Equal("Sistema de Gestión Empresarial", firstProject.Name);
            Assert.Equal("2025-11-23", firstProject.LastEdited);
        }

        #endregion

        #region CloseCommand Tests

        [Fact]
        public void CloseCommand_CanExecute()
        {
            var vm = new ProjectsPopupViewModel();

            Assert.True(vm.CloseCommand.CanExecute(null));
        }

        [Fact]
        public void CloseCommand_RaisesCloseRequestedEvent()
        {
            var vm = new ProjectsPopupViewModel();
            var eventRaised = false;

            vm.CloseRequested += () => eventRaised = true;
            vm.CloseCommand.Execute(null);

            Assert.True(eventRaised);
        }

        #endregion

        #region DeleteProjectCommand Tests

        [Fact]
        public void DeleteProjectCommand_CanExecute()
        {
            var vm = new ProjectsPopupViewModel();
            var project = vm.Projects.First();

            Assert.True(vm.DeleteProjectCommand.CanExecute(project));
        }

        [Fact]
        public void DeleteProjectCommand_RemovesProject()
        {
            var vm = new ProjectsPopupViewModel();
            var project = vm.Projects.First();
            var initialCount = vm.Projects.Count;

            vm.DeleteProjectCommand.Execute(project);

            Assert.Equal(initialCount - 1, vm.Projects.Count);
            Assert.DoesNotContain(project, vm.Projects);
        }

        [Fact]
        public void DeleteProjectCommand_WithNullProject_DoesNothing()
        {
            var vm = new ProjectsPopupViewModel();
            var initialCount = vm.Projects.Count;

            vm.DeleteProjectCommand.Execute(null);

            Assert.Equal(initialCount, vm.Projects.Count);
        }

        [Fact]
        public void DeleteProjectCommand_RemovesCorrectProject()
        {
            var vm = new ProjectsPopupViewModel();
            var projectToDelete = vm.Projects.FirstOrDefault(p => p.Id == "PRJ-003");

            vm.DeleteProjectCommand.Execute(projectToDelete);

            Assert.DoesNotContain(vm.Projects, p => p.Id == "PRJ-003");
            Assert.Contains(vm.Projects, p => p.Id == "PRJ-001");
            Assert.Contains(vm.Projects, p => p.Id == "PRJ-002");
        }

        #endregion

        #region OpenProjectCommand Tests

        [Fact]
        public void OpenProjectCommand_CanExecute()
        {
            var vm = new ProjectsPopupViewModel();
            var project = vm.Projects.First();

            Assert.True(vm.OpenProjectCommand.CanExecute(project));
        }

        [Fact]
        public void OpenProjectCommand_WithNullProject_DoesNotThrow()
        {
            var vm = new ProjectsPopupViewModel();

            var exception = Record.Exception(() => vm.OpenProjectCommand.Execute(null));

            Assert.Null(exception);
        }

        [Fact]
        public void OpenProjectCommand_WithValidProject_DoesNotThrow()
        {
            var vm = new ProjectsPopupViewModel();
            var project = vm.Projects.First();

            var exception = Record.Exception(() => vm.OpenProjectCommand.Execute(project));

            Assert.Null(exception);
        }

        #endregion

        #region NewProjectCommand Tests

        [Fact]
        public void NewProjectCommand_CanExecute()
        {
            var vm = new ProjectsPopupViewModel();

            Assert.True(vm.NewProjectCommand.CanExecute(null));
        }

        [Fact]
        public void NewProjectCommand_DoesNotThrow()
        {
            var vm = new ProjectsPopupViewModel();

            var exception = Record.Exception(() => vm.NewProjectCommand.Execute(null));

            Assert.Null(exception);
        }

        #endregion

        #region Projects Collection Tests

        [Fact]
        public void Projects_CanBeModified()
        {
            var vm = new ProjectsPopupViewModel();
            var newProject = new ProjectListItemViewModel
            {
                Id = "PRJ-006",
                Name = "Test Project",
                LastEdited = "2025-11-24"
            };

            vm.Projects.Add(newProject);

            Assert.Contains(newProject, vm.Projects);
            Assert.Equal(6, vm.Projects.Count);
        }

        [Fact]
        public void Projects_RaisesPropertyChanged_WhenSet()
        {
            var vm = new ProjectsPopupViewModel();
            var raised = false;

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.Projects))
                    raised = true;
            };

            vm.Projects = new System.Collections.ObjectModel.ObservableCollection<ProjectListItemViewModel>();

            Assert.True(raised);
        }

        #endregion

        #region Sample Data Tests

        [Fact]
        public void SampleData_ContainsExpectedProjects()
        {
            var vm = new ProjectsPopupViewModel();

            Assert.Contains(vm.Projects, p => p.Id == "PRJ-001" && p.Name == "Sistema de Gestión Empresarial");
            Assert.Contains(vm.Projects, p => p.Id == "PRJ-002" && p.Name == "Portal Web Corporativo");
            Assert.Contains(vm.Projects, p => p.Id == "PRJ-003" && p.Name == "App Mobile de Ventas");
            Assert.Contains(vm.Projects, p => p.Id == "PRJ-004" && p.Name == "Dashboard Analítico");
            Assert.Contains(vm.Projects, p => p.Id == "PRJ-005" && p.Name == "Sistema de Inventario");
        }

        [Fact]
        public void SampleData_HasValidDates()
        {
            var vm = new ProjectsPopupViewModel();

            Assert.All(vm.Projects, project =>
            {
                Assert.False(string.IsNullOrWhiteSpace(project.LastEdited));
            });
        }

        #endregion
    }
}

