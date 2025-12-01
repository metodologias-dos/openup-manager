using Microsoft.Extensions.Logging;
using Moq;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using OpenUpMan.Services;
using Xunit;

namespace OpenUpMan.Tests.Services
{
    public class DocumentVersionServiceUnitTests
    {
        private static ILogger<DocumentVersionService> CreateMockLogger()
        {
            return new Mock<ILogger<DocumentVersionService>>().Object;
        }

        #region CreateVersion Tests

        [Fact]
        public async Task CreateVersion_ReturnsSuccess_WhenDocumentExists()
        {
            var documentId = Guid.NewGuid();
            var phaseItemId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();
            var document = new Document(phaseItemId, "Test Doc");
            var binario = new byte[] { 1, 2, 3, 4, 5 };

            var mockRepo = new Mock<IDocumentVersionRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetLatestVersionAsync(documentId, default)).ReturnsAsync((DocumentVersion?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<DocumentVersion>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockDocRepo = new Mock<IDocumentRepository>(MockBehavior.Strict);
            mockDocRepo.Setup(r => r.GetByIdAsync(documentId, default)).ReturnsAsync(document);

            var service = new DocumentVersionService(mockRepo.Object, mockDocRepo.Object, CreateMockLogger());

            var result = await service.CreateVersionAsync(documentId, createdBy, ".pdf", binario, "Primera versión");

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.Contains("exitosa", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(result.DocumentVersion);
            Assert.Equal(1, result.DocumentVersion.VersionNumber);
            Assert.Equal(".pdf", result.DocumentVersion.Extension);
        }

        [Fact]
        public async Task CreateVersion_ReturnsError_WhenDocumentNotFound()
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();
            var binario = new byte[] { 1, 2, 3 };

            var mockRepo = new Mock<IDocumentVersionRepository>(MockBehavior.Strict);
            
            var mockDocRepo = new Mock<IDocumentRepository>(MockBehavior.Strict);
            mockDocRepo.Setup(r => r.GetByIdAsync(documentId, default)).ReturnsAsync((Document?)null);

            var service = new DocumentVersionService(mockRepo.Object, mockDocRepo.Object, CreateMockLogger());

            var result = await service.CreateVersionAsync(documentId, createdBy, ".pdf", binario);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("encontrado", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateVersion_IncrementsVersionNumber()
        {
            var documentId = Guid.NewGuid();
            var phaseItemId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();
            var document = new Document(phaseItemId, "Test Doc");
            var binario = new byte[] { 1, 2, 3 };
            
            // Simular que ya existe una versión
            var existingVersion = new DocumentVersion(documentId, 1, createdBy, ".pdf", new byte[] { 1, 2 });

            var mockRepo = new Mock<IDocumentVersionRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetLatestVersionAsync(documentId, default)).ReturnsAsync(existingVersion);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<DocumentVersion>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockDocRepo = new Mock<IDocumentRepository>(MockBehavior.Strict);
            mockDocRepo.Setup(r => r.GetByIdAsync(documentId, default)).ReturnsAsync(document);

            var service = new DocumentVersionService(mockRepo.Object, mockDocRepo.Object, CreateMockLogger());

            var result = await service.CreateVersionAsync(documentId, createdBy, ".pdf", binario);

            Assert.True(result.Success);
            Assert.NotNull(result.DocumentVersion);
            Assert.Equal(2, result.DocumentVersion.VersionNumber);
        }

        #endregion

        #region GetLatestVersion Tests

        [Fact]
        public async Task GetLatestVersion_ReturnsSuccess_WhenVersionExists()
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();
            var latestVersion = new DocumentVersion(documentId, 3, createdBy, ".pdf", new byte[] { 1, 2, 3 });

            var mockRepo = new Mock<IDocumentVersionRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetLatestVersionAsync(documentId, default)).ReturnsAsync(latestVersion);

            var mockDocRepo = new Mock<IDocumentRepository>().Object;

            var service = new DocumentVersionService(mockRepo.Object, mockDocRepo, CreateMockLogger());

            var result = await service.GetLatestVersionAsync(documentId);

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.NotNull(result.DocumentVersion);
            Assert.Equal(3, result.DocumentVersion.VersionNumber);
        }

        [Fact]
        public async Task GetLatestVersion_ReturnsError_WhenNoVersionsExist()
        {
            var documentId = Guid.NewGuid();

            var mockRepo = new Mock<IDocumentVersionRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetLatestVersionAsync(documentId, default)).ReturnsAsync((DocumentVersion?)null);

            var mockDocRepo = new Mock<IDocumentRepository>().Object;

            var service = new DocumentVersionService(mockRepo.Object, mockDocRepo, CreateMockLogger());

            var result = await service.GetLatestVersionAsync(documentId);

            Assert.False(result.Success);
            Assert.Equal(ServiceResultType.Error, result.ResultType);
            Assert.Contains("versiones", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region UpdateObservations Tests

        [Fact]
        public async Task UpdateObservations_ReturnsSuccess_WhenVersionExists()
        {
            var versionId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();
            var version = new DocumentVersion(documentId, 1, createdBy, ".pdf", new byte[] { 1, 2, 3 });

            var mockRepo = new Mock<IDocumentVersionRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByIdAsync(versionId, default)).ReturnsAsync(version);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<DocumentVersion>(), default)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            var mockDocRepo = new Mock<IDocumentRepository>().Object;

            var service = new DocumentVersionService(mockRepo.Object, mockDocRepo, CreateMockLogger());

            var result = await service.UpdateObservationsAsync(versionId, "Nuevas observaciones");

            Assert.True(result.Success);
            Assert.Equal(ServiceResultType.Success, result.ResultType);
            Assert.NotNull(result.DocumentVersion);
            Assert.Equal("Nuevas observaciones", result.DocumentVersion.Observations);
        }

        #endregion

        #region GetVersionsByDocument Tests

        [Fact]
        public async Task GetVersionsByDocument_ReturnsAllVersions()
        {
            var documentId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();
            var versions = new List<DocumentVersion>
            {
                new DocumentVersion(documentId, 1, createdBy, ".pdf", new byte[] { 1, 2, 3 }),
                new DocumentVersion(documentId, 2, createdBy, ".pdf", new byte[] { 4, 5, 6 }),
                new DocumentVersion(documentId, 3, createdBy, ".pdf", new byte[] { 7, 8, 9 })
            };

            var mockRepo = new Mock<IDocumentVersionRepository>(MockBehavior.Strict);
            mockRepo.Setup(r => r.GetByDocumentIdAsync(documentId, default)).ReturnsAsync(versions);

            var mockDocRepo = new Mock<IDocumentRepository>().Object;

            var service = new DocumentVersionService(mockRepo.Object, mockDocRepo, CreateMockLogger());

            var result = await service.GetVersionsByDocumentAsync(documentId);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        #endregion
    }
}

