using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using OpenUpMan.Services;
using Xunit;

namespace OpenUpMan.Tests.Services
{
    public class DocumentServiceUnitTests
    {
        private static ILogger<DocumentService> CreateMockLogger()
        {
            return new Mock<ILogger<DocumentService>>().Object;
        }

        #region CreateDocument Tests

        [Fact]
        public async Task CreateDocument_ReturnsSuccess_WhenValidData()
        {
            var phaseItemId = Guid.NewGuid();

            var mockRepo = new Mock<IDocumentRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Document>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockItemRepo = new Mock<IPhaseItemRepository>().Object;

            var service = new DocumentService(mockRepo.Object, mockItemRepo, CreateMockLogger());

            var result = await service.CreateDocumentAsync(phaseItemId, "Documento Test", "Descripción");

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("exitosa", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.Document);
            Assert.Equal("Documento Test", result.Document.Title);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public async Task CreateDocument_ReturnsError_WhenTitleIsEmpty(string? title)
        {
            var phaseItemId = Guid.NewGuid();

            var mockRepo = new Mock<IDocumentRepository>(MockBehavior.Strict);
            var mockItemRepo = new Mock<IPhaseItemRepository>().Object;

            var service = new DocumentService(mockRepo.Object, mockItemRepo, CreateMockLogger());

            var result = await service.CreateDocumentAsync(phaseItemId, title!);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("requerido", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region GetDocumentById Tests

        [Fact]
        public async Task GetDocumentById_ReturnsSuccess_WhenDocumentExists()
        {
            var documentId = Guid.NewGuid();
            var phaseItemId = Guid.NewGuid();
            var document = new Document(phaseItemId, "Test Doc");

            var mockRepo = new Mock<IDocumentRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(documentId, default)).ReturnsAsync(document);

            var mockItemRepo = new Mock<IPhaseItemRepository>().Object;

            var service = new DocumentService(mockRepo.Object, mockItemRepo, CreateMockLogger());

            var result = await service.GetDocumentByIdAsync(documentId);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.NotNull(result.Document);
        }

        [Fact]
        public async Task GetDocumentById_ReturnsError_WhenDocumentNotFound()
        {
            var documentId = Guid.NewGuid();

            var mockRepo = new Mock<IDocumentRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(documentId, default)).ReturnsAsync((Document?)null);

            var mockItemRepo = new Mock<IPhaseItemRepository>().Object;

            var service = new DocumentService(mockRepo.Object, mockItemRepo, CreateMockLogger());

            var result = await service.GetDocumentByIdAsync(documentId);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("encontrado", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region UpdateDocument Tests

        [Fact]
        public async Task UpdateDocument_ReturnsSuccess_WhenDocumentExists()
        {
            var documentId = Guid.NewGuid();
            var phaseItemId = Guid.NewGuid();
            var document = new Document(phaseItemId, "Old Title");

            var mockRepo = new Mock<IDocumentRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(documentId, default)).ReturnsAsync(document);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Document>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockItemRepo = new Mock<IPhaseItemRepository>().Object;

            var service = new DocumentService(mockRepo.Object, mockItemRepo, CreateMockLogger());

            var result = await service.UpdateDocumentAsync(documentId, "New Title", "New Description");

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.NotNull(result.Document);
            Assert.Equal("New Title", result.Document.Title);
        }

        #endregion

        #region GetDocumentsByPhaseItem Tests

        [Fact]
        public async Task GetDocumentsByPhaseItem_ReturnsDocuments()
        {
            var phaseItemId = Guid.NewGuid();
            var documents = new List<Document>
            {
                new Document(phaseItemId, "Doc 1"),
                new Document(phaseItemId, "Doc 2")
            };

            var mockRepo = new Mock<IDocumentRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByPhaseItemIdAsync(phaseItemId, default)).ReturnsAsync(documents);

            var mockItemRepo = new Mock<IPhaseItemRepository>().Object;

            var service = new DocumentService(mockRepo.Object, mockItemRepo, CreateMockLogger());

            var result = await service.GetDocumentsByPhaseItemAsync(phaseItemId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        #endregion
    }
}

