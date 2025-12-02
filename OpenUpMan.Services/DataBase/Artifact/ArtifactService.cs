using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class ArtifactService : IArtifactService
    {
        private readonly IArtifactRepository _repo;
        private readonly ILogger<ArtifactService> _logger;

        public ArtifactService(IArtifactRepository repo, ILogger<ArtifactService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ArtifactServiceResult> CreateArtifactAsync(int projectId, int phaseId, string name, string? artifactType = null, bool mandatory = false, string? description = null, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ArtifactServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El nombre del artefacto es requerido."
                    );
                }

                var artifact = new Artifact(projectId, phaseId, name, artifactType, mandatory, description);
                await _repo.AddAsync(artifact, ct);

                _logger.LogInformation("Artefacto creado exitosamente: {ArtifactName} (ID: {ArtifactId}) para proyecto {ProjectId}, fase {PhaseId}", 
                    name, artifact.Id, projectId, phaseId);

                return new ArtifactServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Artefacto creado exitosamente.",
                    Artifact: artifact
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el artefacto: {ArtifactName}", name);
                return new ArtifactServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear el artefacto. Por favor, inténtelo de nuevo."
                );
            }
        }

        public async Task<ArtifactServiceResult> GetArtifactByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var artifact = await _repo.GetByIdAsync(id, ct);
                if (artifact == null)
                {
                    return new ArtifactServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Artefacto no encontrado."
                    );
                }

                return new ArtifactServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Artefacto encontrado.",
                    Artifact: artifact
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el artefacto con ID: {ArtifactId}", id);
                return new ArtifactServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener el artefacto."
                );
            }
        }

        public async Task<IEnumerable<Artifact>> GetArtifactsByProjectIdAsync(int projectId, CancellationToken ct = default)
        {
            return await _repo.GetByProjectIdAsync(projectId, ct);
        }

        public async Task<IEnumerable<Artifact>> GetArtifactsByPhaseIdAsync(int phaseId, CancellationToken ct = default)
        {
            return await _repo.GetByPhaseIdAsync(phaseId, ct);
        }

        public async Task<IEnumerable<Artifact>> GetMandatoryArtifactsByProjectIdAsync(int projectId, CancellationToken ct = default)
        {
            return await _repo.GetMandatoryByProjectIdAsync(projectId, ct);
        }

        public async Task<ArtifactServiceResult> UpdateArtifactAsync(int id, string name, string? artifactType, bool mandatory, string? description, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ArtifactServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El nombre del artefacto es requerido."
                    );
                }

                var artifact = await _repo.GetByIdAsync(id, ct);
                if (artifact == null)
                {
                    return new ArtifactServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Artefacto no encontrado."
                    );
                }

                artifact.UpdateDetails(name, artifactType, mandatory, description);
                await _repo.UpdateAsync(artifact, ct);

                _logger.LogInformation("Artefacto actualizado exitosamente: {ArtifactId}", id);

                return new ArtifactServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Artefacto actualizado exitosamente.",
                    Artifact: artifact
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el artefacto: {ArtifactId}", id);
                return new ArtifactServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar el artefacto."
                );
            }
        }

        public async Task<ArtifactServiceResult> SetStateAsync(int id, string state, CancellationToken ct = default)
        {
            try
            {
                var artifact = await _repo.GetByIdAsync(id, ct);
                if (artifact == null)
                {
                    return new ArtifactServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Artefacto no encontrado."
                    );
                }

                artifact.SetState(state);
                await _repo.UpdateAsync(artifact, ct);

                return new ArtifactServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Estado del artefacto actualizado.",
                    Artifact: artifact
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de artefacto");
                return new ArtifactServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al cambiar el estado del artefacto."
                );
            }
        }

        public async Task<ArtifactServiceResult> DeleteArtifactAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var exists = await _repo.ExistsAsync(id, ct);
                if (!exists)
                {
                    return new ArtifactServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Artefacto no encontrado."
                    );
                }

                await _repo.DeleteAsync(id, ct);

                _logger.LogInformation("Artefacto eliminado exitosamente: {ArtifactId}", id);

                return new ArtifactServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Artefacto eliminado exitosamente."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el artefacto: {ArtifactId}", id);
                return new ArtifactServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al eliminar el artefacto."
                );
            }
        }
    }
}

