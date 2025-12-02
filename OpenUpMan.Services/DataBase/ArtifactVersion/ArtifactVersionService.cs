using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class ArtifactVersionService : IArtifactVersionService
    {
        private readonly IArtifactRepository _artifactRepo;
        private readonly ILogger<ArtifactVersionService> _logger;

        public ArtifactVersionService(IArtifactRepository artifactRepo, ILogger<ArtifactVersionService> logger)
        {
            _artifactRepo = artifactRepo;
            _logger = logger;
        }

        public async Task<ArtifactVersionServiceResult> CreateVersionAsync(int artifactId, int? createdBy, string? notes = null, byte[]? fileBlob = null, string? fileMime = null, string? buildInfo = null, CancellationToken ct = default)
        {
            try
            {
                // Verify artifact exists
                var artifact = await _artifactRepo.GetByIdAsync(artifactId, ct);
                if (artifact == null)
                {
                    return new ArtifactVersionServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Artefacto no encontrado."
                    );
                }

                var version = new ArtifactVersion(artifactId, createdBy, notes, fileBlob, fileMime, buildInfo);
                var versionNumber = await _artifactRepo.AddVersionAsync(version, ct);

                _logger.LogInformation("Versión {VersionNumber} creada para artefacto {ArtifactId}", versionNumber, artifactId);

                return new ArtifactVersionServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Versión creada exitosamente.",
                    ArtifactVersion: version,
                    VersionNumber: versionNumber
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear versión de artefacto");
                return new ArtifactVersionServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear la versión del artefacto."
                );
            }
        }

        public async Task<ArtifactVersionServiceResult> GetLatestVersionAsync(int artifactId, CancellationToken ct = default)
        {
            try
            {
                var version = await _artifactRepo.GetLatestVersionAsync(artifactId, ct);
                if (version == null)
                {
                    return new ArtifactVersionServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "No se encontró ninguna versión del artefacto."
                    );
                }

                return new ArtifactVersionServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Versión encontrada.",
                    ArtifactVersion: version,
                    VersionNumber: version.VersionNumber
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener última versión");
                return new ArtifactVersionServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener la última versión."
                );
            }
        }

        public async Task<ArtifactVersionServiceResult> GetVersionAsync(int artifactId, int versionNumber, CancellationToken ct = default)
        {
            try
            {
                var version = await _artifactRepo.GetVersionAsync(artifactId, versionNumber, ct);
                if (version == null)
                {
                    return new ArtifactVersionServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Versión no encontrada."
                    );
                }

                return new ArtifactVersionServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Versión encontrada.",
                    ArtifactVersion: version,
                    VersionNumber: version.VersionNumber
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener versión");
                return new ArtifactVersionServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener la versión."
                );
            }
        }

        public async Task<IEnumerable<ArtifactVersion>> GetVersionHistoryAsync(int artifactId, CancellationToken ct = default)
        {
            return await _artifactRepo.GetVersionHistoryAsync(artifactId, ct);
        }
    }
}

