using Xunit;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.Tests.UI
{
    public class ProjectListItemViewModelTests
    {
        #region Property Tests

        [Fact]
        public void Id_CanBeSetAndGet()
        {
            var vm = new ProjectListItemViewModel();

            vm.Id = "PRJ-123";

            Assert.Equal("PRJ-123", vm.Id);
        }

        [Fact]
        public void Name_CanBeSetAndGet()
        {
            var vm = new ProjectListItemViewModel();

            vm.Name = "Test Project";

            Assert.Equal("Test Project", vm.Name);
        }

        [Fact]
        public void LastEdited_CanBeSetAndGet()
        {
            var vm = new ProjectListItemViewModel();

            vm.LastEdited = "2025-11-24";

            Assert.Equal("2025-11-24", vm.LastEdited);
        }

        [Fact]
        public void Constructor_InitializesWithEmptyStrings()
        {
            var vm = new ProjectListItemViewModel();

            Assert.Equal(string.Empty, vm.Id);
            Assert.Equal(string.Empty, vm.Name);
            Assert.Equal(string.Empty, vm.LastEdited);
        }

        #endregion

        #region PropertyChanged Tests

        [Fact]
        public void Id_RaisesPropertyChanged()
        {
            var vm = new ProjectListItemViewModel();
            var raised = false;

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.Id))
                    raised = true;
            };

            vm.Id = "PRJ-001";

            Assert.True(raised);
        }

        [Fact]
        public void Name_RaisesPropertyChanged()
        {
            var vm = new ProjectListItemViewModel();
            var raised = false;

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.Name))
                    raised = true;
            };

            vm.Name = "New Name";

            Assert.True(raised);
        }

        [Fact]
        public void LastEdited_RaisesPropertyChanged()
        {
            var vm = new ProjectListItemViewModel();
            var raised = false;

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.LastEdited))
                    raised = true;
            };

            vm.LastEdited = "2025-11-24";

            Assert.True(raised);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void ProjectListItem_CanBeUsedInCollection()
        {
            var collection = new System.Collections.ObjectModel.ObservableCollection<ProjectListItemViewModel>();
            var item = new ProjectListItemViewModel
            {
                Id = "PRJ-001",
                Name = "Test",
                LastEdited = "2025-11-24"
            };

            collection.Add(item);

            Assert.Single(collection);
            Assert.Equal(item, collection[0]);
        }

        #endregion
    }
}

