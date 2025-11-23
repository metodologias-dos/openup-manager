using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using OpenUpMan.Services;
using Xunit;

namespace OpenUpMan.Tests.Services
{
    public class ProjectPhaseServiceUnitTests
    {
        private static ILogger<ProjectPhaseService> CreateMockLogger()
        {
            return new Mock<ILogger<ProjectPhaseService>>().Object;
        }

        #region CreatePhase Tests

        [Fact]
        public async Task CreatePhase_ReturnsSuccess_WhenPhaseCodeIsNew()
        {
            var projectId = Guid.NewGuid();

            var mockRepo = new Mock<IProjectPhaseRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByProjectIdAndCodeAsync(projectId, PhaseCode.INCEPTION, default))
                .ReturnsAsync((ProjectPhase?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<ProjectPhase>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;

            var service = new ProjectPhaseService(mockRepo.Object, mockProjectRepo, CreateMockLogger());

            var result = await service.CreatePhaseAsync(projectId, PhaseCode.INCEPTION, "Inicio", 1);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("exitosa", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.ProjectPhase);
            Assert.Equal(PhaseCode.INCEPTION, result.ProjectPhase.Code);
        }

        [Fact]
        public async Task CreatePhase_ReturnsError_WhenPhaseCodeExists()
        {
            var projectId = Guid.NewGuid();
            var existingPhase = new ProjectPhase(projectId, PhaseCode.INCEPTION, "Existing", 1);

            var mockRepo = new Mock<IProjectPhaseRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByProjectIdAndCodeAsync(projectId, PhaseCode.INCEPTION, default))
                .ReturnsAsync(existingPhase);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;

            var service = new ProjectPhaseService(mockRepo.Object, mockProjectRepo, CreateMockLogger());

            var result = await service.CreatePhaseAsync(projectId, PhaseCode.INCEPTION, "New", 1);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("existe", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region InitializeProjectPhases Tests

        [Fact]
        public async Task InitializeProjectPhases_CreatesAllFourPhases()
        {
            var projectId = Guid.NewGuid();

            var mockRepo = new Mock<IProjectPhaseRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<ProjectPhase>(), default))
                .Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;

            var service = new ProjectPhaseService(mockRepo.Object, mockProjectRepo, CreateMockLogger());

            var result = await service.InitializeProjectPhasesAsync(projectId);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("inicializada", result.Message, StringComparison.OrdinalIgnoreCase);
            
            // Verificar que se llamó 4 veces para crear las 4 fases
            mockRepo.Verify(r => r.AddAsync(It.IsAny<ProjectPhase>(), default), Times.Exactly(4));
            mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        #endregion

        #region ChangePhaseState Tests

        [Theory]
        [InlineData(PhaseState.NOT_STARTED)]
        [InlineData(PhaseState.IN_PROGRESS)]
        [InlineData(PhaseState.COMPLETED)]
        public async Task ChangePhaseState_ReturnsSuccess_WhenPhaseExists(PhaseState newState)
        {
            var phaseId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var existingPhase = new ProjectPhase(projectId, PhaseCode.INCEPTION, "Test", 1);

            var mockRepo = new Mock<IProjectPhaseRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(phaseId, default)).ReturnsAsync(existingPhase);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<ProjectPhase>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;

            var service = new ProjectPhaseService(mockRepo.Object, mockProjectRepo, CreateMockLogger());

            var result = await service.ChangePhaseStateAsync(phaseId, newState);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("actualizado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.ProjectPhase);
            Assert.Equal(newState, result.ProjectPhase.State);
        }

        [Fact]
        public async Task ChangePhaseState_ReturnsError_WhenPhaseNotFound()
        {
            var phaseId = Guid.NewGuid();

            var mockRepo = new Mock<IProjectPhaseRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(phaseId, default)).ReturnsAsync((ProjectPhase?)null);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;

            var service = new ProjectPhaseService(mockRepo.Object, mockProjectRepo, CreateMockLogger());

            var result = await service.ChangePhaseStateAsync(phaseId, PhaseState.IN_PROGRESS);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("encontrada", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region GetPhasesByProject Tests

        [Fact]
        public async Task GetPhasesByProject_ReturnsOrderedPhases()
        {
            var projectId = Guid.NewGuid();
            var phases = new List<ProjectPhase>
            {
                new ProjectPhase(projectId, PhaseCode.INCEPTION, "Inicio", 1),
                new ProjectPhase(projectId, PhaseCode.ELABORATION, "Elaboración", 2)
            };

            var mockRepo = new Mock<IProjectPhaseRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByProjectIdAsync(projectId, default)).ReturnsAsync(phases);

            var mockProjectRepo = new Mock<IProjectRepository>().Object;

            var service = new ProjectPhaseService(mockRepo.Object, mockProjectRepo, CreateMockLogger());

            var result = await service.GetPhasesByProjectAsync(projectId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        #endregion
    }
}

