using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class PhaseService : IPhaseService
    {
        private readonly IPhaseRepository _repo;
        private readonly ILogger<PhaseService> _logger;

        public PhaseService(IPhaseRepository repo, ILogger<PhaseService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<PhaseServiceResult> CreatePhaseAsync(int projectId, string name, int orderIndex, string status = "PENDING", CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new PhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El nombre de la fase es requerido."
                    );
                }

                var phase = new Phase(projectId, name, orderIndex);
                if (status != "PENDING")
                {
                    phase.SetStatus(status);
                }
                await _repo.AddAsync(phase, ct);

                _logger.LogInformation("Fase creada: {PhaseId} - {Name} para proyecto {ProjectId}", phase.Id, name, projectId);

                return new PhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Fase creada exitosamente.",
                    Phase: phase
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear fase");
                return new PhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear la fase."
                );
            }
        }

        public async Task<PhaseServiceResult> GetPhaseByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var phase = await _repo.GetByIdAsync(id, ct);
                if (phase == null)
                {
                    return new PhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Fase no encontrada."
                    );
                }

                return new PhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Fase encontrada.",
                    Phase: phase
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener fase");
                return new PhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener la fase."
                );
            }
        }

        public async Task<IEnumerable<Phase>> GetPhasesByProjectIdAsync(int projectId, CancellationToken ct = default)
        {
            return await _repo.GetByProjectIdAsync(projectId, ct);
        }

        public async Task<PhaseServiceResult> UpdatePhaseAsync(int id, string name, DateTime? startDate, DateTime? endDate, string status, CancellationToken ct = default)
        {
            try
            {
                var phase = await _repo.GetByIdAsync(id, ct);
                if (phase == null)
                {
                    return new PhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Fase no encontrada."
                    );
                }

                phase.UpdateDetails(name, startDate, endDate, phase.OrderIndex);
                phase.SetStatus(status);
                await _repo.UpdateAsync(phase, ct);

                return new PhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Fase actualizada exitosamente.",
                    Phase: phase
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar fase");
                return new PhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar la fase."
                );
            }
        }

        public async Task<PhaseServiceResult> SetPhaseStatusAsync(int id, string status, CancellationToken ct = default)
        {
            try
            {
                var phase = await _repo.GetByIdAsync(id, ct);
                if (phase == null)
                {
                    return new PhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Fase no encontrada."
                    );
                }

                phase.SetStatus(status);
                await _repo.UpdateAsync(phase, ct);

                return new PhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Estado de la fase actualizado.",
                    Phase: phase
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de fase");
                return new PhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al cambiar el estado de la fase."
                );
            }
        }

        public async Task<PhaseServiceResult> DeletePhaseAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var phase = await _repo.GetByIdAsync(id, ct);
                if (phase == null)
                {
                    return new PhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Fase no encontrada."
                    );
                }

                await _repo.DeleteAsync(id, ct);

                _logger.LogInformation("Fase {PhaseId} eliminada exitosamente", id);

                return new PhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Fase eliminada exitosamente."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar fase {PhaseId}", id);
                return new PhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: $"Error al eliminar la fase: {ex.Message}"
                );
            }
        }
    }
}

