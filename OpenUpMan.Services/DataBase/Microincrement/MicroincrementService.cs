using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class MicroincrementService : IMicroincrementService
    {
        private readonly IMicroincrementRepository _repo;
        private readonly ILogger<MicroincrementService> _logger;

        public MicroincrementService(IMicroincrementRepository repo, ILogger<MicroincrementService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<MicroincrementServiceResult> CreateMicroincrementAsync(int iterationId, string title, string? description, int? authorId, string type, int? artifactId = null, string? evidenceUrl = null, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    return new MicroincrementServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El título es requerido."
                    );
                }

                var microincrement = new Microincrement(iterationId, title, authorId, type, description);
                if (artifactId.HasValue)
                {
                    microincrement.SetArtifact(artifactId.Value);
                }
                if (!string.IsNullOrWhiteSpace(evidenceUrl))
                {
                    microincrement.SetEvidenceUrl(evidenceUrl);
                }
                await _repo.AddAsync(microincrement, ct);

                _logger.LogInformation("Microincremento creado: {MicroincrementId} - {Title} para iteración {IterationId}", 
                    microincrement.Id, title, iterationId);

                return new MicroincrementServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Microincremento creado exitosamente.",
                    Microincrement: microincrement
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear microincremento");
                return new MicroincrementServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear el microincremento."
                );
            }
        }

        public async Task<MicroincrementServiceResult> GetMicroincrementByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var microincrement = await _repo.GetByIdAsync(id, ct);
                if (microincrement == null)
                {
                    return new MicroincrementServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Microincremento no encontrado."
                    );
                }

                return new MicroincrementServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Microincremento encontrado.",
                    Microincrement: microincrement
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener microincremento");
                return new MicroincrementServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener el microincremento."
                );
            }
        }

        public async Task<IEnumerable<Microincrement>> GetMicroincrementsByIterationIdAsync(int iterationId, CancellationToken ct = default)
        {
            return await _repo.GetByIterationIdAsync(iterationId, ct);
        }

        public async Task<IEnumerable<Microincrement>> GetMicroincrementsByAuthorAsync(int authorId, CancellationToken ct = default)
        {
            return await _repo.GetByAuthorIdAsync(authorId, ct);
        }

        public async Task<IEnumerable<Microincrement>> GetMicroincrementsByArtifactAsync(int artifactId, CancellationToken ct = default)
        {
            return await _repo.GetByArtifactIdAsync(artifactId, ct);
        }

        public async Task<MicroincrementServiceResult> UpdateMicroincrementAsync(int id, string title, string? description, string? evidenceUrl, CancellationToken ct = default)
        {
            try
            {
                var microincrement = await _repo.GetByIdAsync(id, ct);
                if (microincrement == null)
                {
                    return new MicroincrementServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Microincremento no encontrado."
                    );
                }

                microincrement.UpdateDetails(title, description, microincrement.Type);
                if (evidenceUrl != null)
                {
                    microincrement.SetEvidenceUrl(evidenceUrl);
                }
                await _repo.UpdateAsync(microincrement, ct);

                return new MicroincrementServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Microincremento actualizado exitosamente.",
                    Microincrement: microincrement
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar microincremento");
                return new MicroincrementServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar el microincremento."
                );
            }
        }

        public async Task<MicroincrementServiceResult> DeleteMicroincrementAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var microincrement = await _repo.GetByIdAsync(id, ct);
                if (microincrement == null)
                {
                    return new MicroincrementServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Microincremento no encontrado."
                    );
                }

                await _repo.DeleteAsync(id, ct);

                _logger.LogInformation("Microincremento {MicroincrementId} eliminado exitosamente", id);

                return new MicroincrementServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Microincremento eliminado exitosamente."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar microincremento {MicroincrementId}", id);
                return new MicroincrementServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: $"Error al eliminar el microincremento: {ex.Message}"
                );
            }
        }
    }
}

