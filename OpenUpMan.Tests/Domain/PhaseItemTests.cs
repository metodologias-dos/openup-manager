using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Domain
{
    public class PhaseItemTests
    {
        [Fact]
        public void Constructor_ShouldInitializeIteration_WithValidParameters()
        {
            var projectPhaseId = Guid.NewGuid();
            var type = PhaseItemType.ITERATION;
            var number = 1;
            var name = "Iteración 1";
            var createdBy = Guid.NewGuid();

            var item = new PhaseItem(projectPhaseId, type, number, name, createdBy);

            Assert.NotEqual(Guid.Empty, item.Id);
            Assert.Equal(projectPhaseId, item.ProjectPhaseId);
            Assert.Equal(type, item.Type);
            Assert.Equal(number, item.Number);
            Assert.Equal(name, item.Name);
            Assert.Equal(createdBy, item.CreatedBy);
            Assert.Null(item.ParentIterationId);
            Assert.Equal(PhaseItemState.PLANNED, item.State);
            Assert.True((DateTime.UtcNow - item.CreatedAt).TotalSeconds < 2);
        }

        [Fact]
        public void Constructor_ShouldInitializeMicroincrement_WithParentIteration()
        {
            var projectPhaseId = Guid.NewGuid();
            var parentIterationId = Guid.NewGuid();
            var type = PhaseItemType.MICROINCREMENT;
            var createdBy = Guid.NewGuid();

            var item = new PhaseItem(projectPhaseId, type, 1, "Micro 1", createdBy, parentIterationId);

            Assert.Equal(type, item.Type);
            Assert.Equal(parentIterationId, item.ParentIterationId);
        }

        [Theory]
        [InlineData(PhaseItemState.PLANNED)]
        [InlineData(PhaseItemState.ACTIVE)]
        [InlineData(PhaseItemState.DONE)]
        [InlineData(PhaseItemState.CANCELLED)]
        public void SetState_ShouldChangeState_WithValidState(PhaseItemState newState)
        {
            var item = new PhaseItem(Guid.NewGuid(), PhaseItemType.ITERATION, 1, "Test", Guid.NewGuid());

            item.SetState(newState);

            Assert.Equal(newState, item.State);
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateAllFields()
        {
            var item = new PhaseItem(Guid.NewGuid(), PhaseItemType.ITERATION, 1, "Old Name", Guid.NewGuid());
            var newStartDate = DateTime.UtcNow.AddDays(1);
            var newEndDate = DateTime.UtcNow.AddDays(15);

            item.UpdateDetails("New Name", "New Description", newStartDate, newEndDate);

            Assert.Equal("New Name", item.Name);
            Assert.Equal("New Description", item.Description);
            Assert.Equal(newStartDate, item.StartDate);
            Assert.Equal(newEndDate, item.EndDate);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenProjectPhaseIdIsEmpty()
        {
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new PhaseItem(Guid.Empty, PhaseItemType.ITERATION, 1, "Test", createdBy));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_ShouldThrow_WhenNameIsInvalid(string? name)
        {
            var projectPhaseId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new PhaseItem(projectPhaseId, PhaseItemType.ITERATION, 1, name!, createdBy));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenCreatedByIsEmpty()
        {
            var projectPhaseId = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new PhaseItem(projectPhaseId, PhaseItemType.ITERATION, 1, "Test", Guid.Empty));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenNumberIsNegative()
        {
            var projectPhaseId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new PhaseItem(projectPhaseId, PhaseItemType.ITERATION, -1, "Test", createdBy));
        }

        [Theory]
        [InlineData(PhaseItemType.ITERATION)]
        [InlineData(PhaseItemType.MICROINCREMENT)]
        public void Constructor_ShouldAcceptBothTypes(PhaseItemType type)
        {
            var projectPhaseId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            var item = new PhaseItem(projectPhaseId, type, 1, "Test", createdBy);

            Assert.Equal(type, item.Type);
        }
    }
}

