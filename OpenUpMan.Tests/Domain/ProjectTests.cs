using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Domain
{
    public class ProjectTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProject_WithValidParameters()
        {
            var identifier = "PROY-001";
            var name = "Proyecto prueba";
            var ownerId = Guid.NewGuid();
            var startDate = DateTime.UtcNow;

            var project = new Project(identifier, name, startDate, ownerId, "Desc");

            Assert.NotEqual(Guid.Empty, project.Id);
            Assert.Equal(identifier, project.Identifier);
            Assert.Equal(name, project.Name);
            Assert.Equal(ownerId, project.OwnerId);
            Assert.Equal(ProjectState.CREATED, project.State);
            Assert.True((DateTime.UtcNow - project.CreatedAt).TotalSeconds < 2);
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateFields_AndSetUpdatedAt()
        {
            var project = new Project("PROY-002", "Name", DateTime.UtcNow, Guid.NewGuid(), "Old");
            var oldUpdated = project.UpdatedAt;

            Thread.Sleep(10);

            var newStart = DateTime.UtcNow.AddDays(1);
            project.UpdateDetails("NewName", "NewDesc", newStart);

            Assert.Equal("NewName", project.Name);
            Assert.Equal("NewDesc", project.Description);
            Assert.Equal(newStart, project.StartDate);
            Assert.NotNull(project.UpdatedAt);
            Assert.NotEqual(oldUpdated, project.UpdatedAt);
        }

        [Fact]
        public void SetState_ShouldChangeState_AndSetUpdatedAt()
        {
            var project = new Project("PROY-003", "Name", DateTime.UtcNow, Guid.NewGuid());
            var oldUpdated = project.UpdatedAt;

            Thread.Sleep(10);

            project.SetState(ProjectState.ACTIVE);

            Assert.Equal(ProjectState.ACTIVE, project.State);
            Assert.NotNull(project.UpdatedAt);
            Assert.NotEqual(oldUpdated, project.UpdatedAt);
        }

        [Fact]
        public void AssignOwner_ShouldSetOwnerId_AndSetUpdatedAt()
        {
            var project = new Project("PROY-004", "Name", DateTime.UtcNow, Guid.NewGuid());
            var newOwner = Guid.NewGuid();
            var oldUpdated = project.UpdatedAt;

            Thread.Sleep(10);

            project.AssignOwner(newOwner);

            Assert.Equal(newOwner, project.OwnerId);
            Assert.NotNull(project.UpdatedAt);
            Assert.NotEqual(oldUpdated, project.UpdatedAt);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenIdentifierInvalid()
        {
            var owner = Guid.NewGuid();
            Assert.Throws<ArgumentException>(() => new Project("", "Name", DateTime.UtcNow, owner));
            Assert.Throws<ArgumentException>(() => new Project("   ", "Name", DateTime.UtcNow, owner));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenNameInvalid()
        {
            var owner = Guid.NewGuid();
            Assert.Throws<ArgumentException>(() => new Project("PROY-005", "", DateTime.UtcNow, owner));
            Assert.Throws<ArgumentException>(() => new Project("PROY-005", "   ", DateTime.UtcNow, owner));
        }
    }
}

