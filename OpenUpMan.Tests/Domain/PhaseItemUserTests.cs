using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Domain
{
    public class PhaseItemUserTests
    {
        [Fact]
        public void Constructor_ShouldInitializePhaseItemUser_WithValidParameters()
        {
            var phaseItemId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var role = "PARTICIPANT";

            var phaseItemUser = new PhaseItemUser(phaseItemId, userId, role);

            Assert.Equal(phaseItemId, phaseItemUser.PhaseItemId);
            Assert.Equal(userId, phaseItemUser.UserId);
            Assert.Equal(role, phaseItemUser.Role);
        }

        [Fact]
        public void Constructor_ShouldUseDefaultRole_WhenNotSpecified()
        {
            var phaseItemId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var phaseItemUser = new PhaseItemUser(phaseItemId, userId);

            Assert.Equal("PARTICIPANT", phaseItemUser.Role);
        }

        [Theory]
        [InlineData("PARTICIPANT")]
        [InlineData("RESPONSIBLE")]
        [InlineData("OBSERVER")]
        public void SetRole_ShouldChangeRole_WithValidRole(string newRole)
        {
            var phaseItemUser = new PhaseItemUser(Guid.NewGuid(), Guid.NewGuid(), "PARTICIPANT");

            phaseItemUser.SetRole(newRole);

            Assert.Equal(newRole, phaseItemUser.Role);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenPhaseItemIdIsEmpty()
        {
            var userId = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => new PhaseItemUser(Guid.Empty, userId));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenUserIdIsEmpty()
        {
            var phaseItemId = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => new PhaseItemUser(phaseItemId, Guid.Empty));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void SetRole_ShouldThrow_WhenRoleIsInvalid(string? role)
        {
            var phaseItemUser = new PhaseItemUser(Guid.NewGuid(), Guid.NewGuid());

            Assert.Throws<ArgumentException>(() => phaseItemUser.SetRole(role!));
        }
    }
}

