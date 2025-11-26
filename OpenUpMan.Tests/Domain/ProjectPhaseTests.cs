using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Domain
{
    public class ProjectPhaseTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProjectPhase_WithValidParameters()
        {
            var projectId = Guid.NewGuid();
            var code = PhaseCode.INCEPTION;
            var name = "Inicio";
            var order = 1;

            var phase = new ProjectPhase(projectId, code, name, order);

            Assert.NotEqual(Guid.Empty, phase.Id);
            Assert.Equal(projectId, phase.ProjectId);
            Assert.Equal(code, phase.Code);
            Assert.Equal(name, phase.Name);
            Assert.Equal(order, phase.Order);
            Assert.Equal(PhaseState.NOT_STARTED, phase.State);
        }

        [Theory]
        [InlineData(PhaseCode.INCEPTION)]
        [InlineData(PhaseCode.ELABORATION)]
        [InlineData(PhaseCode.CONSTRUCTION)]
        [InlineData(PhaseCode.TRANSITION)]
        public void Constructor_ShouldAcceptAllPhaseCodes(PhaseCode code)
        {
            var projectId = Guid.NewGuid();

            var phase = new ProjectPhase(projectId, code, "Test", 1);

            Assert.Equal(code, phase.Code);
        }

        [Theory]
        [InlineData(PhaseState.NOT_STARTED)]
        [InlineData(PhaseState.IN_PROGRESS)]
        [InlineData(PhaseState.COMPLETED)]
        public void SetState_ShouldChangeState_WithValidState(PhaseState newState)
        {
            var phase = new ProjectPhase(Guid.NewGuid(), PhaseCode.INCEPTION, "Test", 1);

            phase.SetState(newState);

            Assert.Equal(newState, phase.State);
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateNameAndOrder()
        {
            var phase = new ProjectPhase(Guid.NewGuid(), PhaseCode.INCEPTION, "Old Name", 1);

            phase.UpdateDetails("New Name", 2);

            Assert.Equal("New Name", phase.Name);
            Assert.Equal(2, phase.Order);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenProjectIdIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => 
                new ProjectPhase(Guid.Empty, PhaseCode.INCEPTION, "Test", 1));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_ShouldThrow_WhenNameIsInvalid(string? name)
        {
            var projectId = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new ProjectPhase(projectId, PhaseCode.INCEPTION, name!, 1));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenOrderIsNegative()
        {
            var projectId = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => 
                new ProjectPhase(projectId, PhaseCode.INCEPTION, "Test", -1));
        }
    }
}

