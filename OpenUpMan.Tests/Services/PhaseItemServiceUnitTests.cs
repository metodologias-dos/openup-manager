using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using OpenUpMan.Services;
using Xunit;

namespace OpenUpMan.Tests.Services
{
    public class PhaseItemServiceUnitTests
    {
        private static ILogger<PhaseItemService> CreateMockLogger()
        {
            return new Mock<ILogger<PhaseItemService>>().Object;
        }

        #region CreateIteration Tests

        [Fact]
        public async Task CreateIteration_ReturnsSuccess_WhenValidData()
        {
            var phaseId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            var mockRepo = new Mock<IPhaseItemRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<PhaseItem>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockPhaseRepo = new Mock<IProjectPhaseRepository>().Object;

            var service = new PhaseItemService(mockRepo.Object, mockPhaseRepo, CreateMockLogger());

            var result = await service.CreateIterationAsync(
                phaseId, 1, "Iteración 1", createdBy, "Descripción", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("exitosa", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.PhaseItem);
            Assert.Equal(PhaseItemType.ITERATION, result.PhaseItem.Type);
            Assert.Equal(1, result.PhaseItem.Number);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<PhaseItem>(), default), Times.Once);
        }

        #endregion

        #region CreateMicroincrement Tests

        [Fact]
        public async Task CreateMicroincrement_ReturnsSuccess_WhenValidData()
        {
            var phaseId = Guid.NewGuid();
            var parentIterationId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();

            var mockRepo = new Mock<IPhaseItemRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<PhaseItem>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockPhaseRepo = new Mock<IProjectPhaseRepository>().Object;

            var service = new PhaseItemService(mockRepo.Object, mockPhaseRepo, CreateMockLogger());

            var result = await service.CreateMicroincrementAsync(
                phaseId, parentIterationId, 1, "Micro 1", createdBy, "Descripción");

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("exitosa", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.PhaseItem);
            Assert.Equal(PhaseItemType.MICROINCREMENT, result.PhaseItem.Type);
            Assert.Equal(parentIterationId, result.PhaseItem.ParentIterationId);
        }

        #endregion

        #region UpdatePhaseItem Tests

        [Fact]
        public async Task UpdatePhaseItem_ReturnsSuccess_WhenItemExists()
        {
            var itemId = Guid.NewGuid();
            var phaseId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();
            var existingItem = new PhaseItem(phaseId, PhaseItemType.ITERATION, 1, "Old Name", createdBy);

            var mockRepo = new Mock<IPhaseItemRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(itemId, default)).ReturnsAsync(existingItem);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<PhaseItem>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockPhaseRepo = new Mock<IProjectPhaseRepository>().Object;

            var service = new PhaseItemService(mockRepo.Object, mockPhaseRepo, CreateMockLogger());

            var result = await service.UpdatePhaseItemAsync(itemId, "New Name", "New Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("actualizado", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.PhaseItem);
            Assert.Equal("New Name", result.PhaseItem.Name);
        }

        [Fact]
        public async Task UpdatePhaseItem_ReturnsError_WhenItemNotFound()
        {
            var itemId = Guid.NewGuid();

            var mockRepo = new Mock<IPhaseItemRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(itemId, default)).ReturnsAsync((PhaseItem?)null);

            var mockPhaseRepo = new Mock<IProjectPhaseRepository>().Object;

            var service = new PhaseItemService(mockRepo.Object, mockPhaseRepo, CreateMockLogger());

            var result = await service.UpdatePhaseItemAsync(itemId, "New Name", "Desc", null, null);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("encontrado", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region ChangePhaseItemState Tests

        [Theory]
        [InlineData(PhaseItemState.PLANNED)]
        [InlineData(PhaseItemState.ACTIVE)]
        [InlineData(PhaseItemState.DONE)]
        [InlineData(PhaseItemState.CANCELLED)]
        public async Task ChangePhaseItemState_ReturnsSuccess_WhenItemExists(PhaseItemState newState)
        {
            var itemId = Guid.NewGuid();
            var phaseId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();
            var existingItem = new PhaseItem(phaseId, PhaseItemType.ITERATION, 1, "Test", createdBy);

            var mockRepo = new Mock<IPhaseItemRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(itemId, default)).ReturnsAsync(existingItem);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<PhaseItem>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockPhaseRepo = new Mock<IProjectPhaseRepository>().Object;

            var service = new PhaseItemService(mockRepo.Object, mockPhaseRepo, CreateMockLogger());

            var result = await service.ChangePhaseItemStateAsync(itemId, newState);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.NotNull(result.PhaseItem);
            Assert.Equal(newState, result.PhaseItem.State);
        }

        #endregion

        #region GetIterationsByPhase Tests

        [Fact]
        public async Task GetIterationsByPhase_ReturnsOnlyIterations()
        {
            var phaseId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();
            var iterations = new List<PhaseItem>
            {
                new PhaseItem(phaseId, PhaseItemType.ITERATION, 1, "Iter 1", createdBy),
                new PhaseItem(phaseId, PhaseItemType.ITERATION, 2, "Iter 2", createdBy)
            };

            var mockRepo = new Mock<IPhaseItemRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetIterationsByPhaseIdAsync(phaseId, default)).ReturnsAsync(iterations);

            var mockPhaseRepo = new Mock<IProjectPhaseRepository>().Object;

            var service = new PhaseItemService(mockRepo.Object, mockPhaseRepo, CreateMockLogger());

            var result = await service.GetIterationsByPhaseAsync(phaseId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(PhaseItemType.ITERATION, item.Type));
        }

        #endregion

        #region GetMicroincrementsByIteration Tests

        [Fact]
        public async Task GetMicroincrementsByIteration_ReturnsMicroincrements()
        {
            var phaseId = Guid.NewGuid();
            var iterationId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();
            var microincrements = new List<PhaseItem>
            {
                new PhaseItem(phaseId, PhaseItemType.MICROINCREMENT, 1, "Micro 1", createdBy, iterationId),
                new PhaseItem(phaseId, PhaseItemType.MICROINCREMENT, 2, "Micro 2", createdBy, iterationId)
            };

            var mockRepo = new Mock<IPhaseItemRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetMicroincrementsByIterationIdAsync(iterationId, default))
                .ReturnsAsync(microincrements);

            var mockPhaseRepo = new Mock<IProjectPhaseRepository>().Object;

            var service = new PhaseItemService(mockRepo.Object, mockPhaseRepo, CreateMockLogger());

            var result = await service.GetMicroincrementsByIterationAsync(iterationId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(PhaseItemType.MICROINCREMENT, item.Type));
        }

        #endregion
    }
}

