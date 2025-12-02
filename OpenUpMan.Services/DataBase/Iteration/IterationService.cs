using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class IterationService : IIterationService
    {
        private readonly IIterationRepository _repo;
        private readonly ILogger<IterationService> _logger;

        public IterationService(IIterationRepository repo, ILogger<IterationService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<IterationServiceResult> CreateIterationAsync(int phaseId, string? name, string? goal, DateTime? startDate, DateTime? endDate, CancellationToken ct = default)
        {
            try
            {
                var iteration = new Iteration(phaseId, name, goal);
                if (startDate.HasValue || endDate.HasValue)
                {
                    iteration.UpdateDetails(name, goal, startDate, endDate);
                }
                await _repo.AddAsync(iteration, ct);

                _logger.LogInformation("Iteración creada: {IterationId} - {Name} para fase {PhaseId}", iteration.Id, name ?? "Sin nombre", phaseId);

                return new IterationServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Iteración creada exitosamente.",
                    Iteration: iteration
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear iteración");
                return new IterationServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear la iteración."
                );
            }
        }

        public async Task<IterationServiceResult> GetIterationByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var iteration = await _repo.GetByIdAsync(id, ct);
                if (iteration == null)
                {
                    return new IterationServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Iteración no encontrada."
                    );
                }

                return new IterationServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Iteración encontrada.",
                    Iteration: iteration
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener iteración");
                return new IterationServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener la iteración."
                );
            }
        }

        public async Task<IEnumerable<Iteration>> GetIterationsByPhaseIdAsync(int phaseId, CancellationToken ct = default)
        {
            return await _repo.GetByPhaseIdAsync(phaseId, ct);
        }

        public async Task<IterationServiceResult> UpdateIterationAsync(int id, string? name, string? goal, DateTime? startDate, DateTime? endDate, int completionPercentage, CancellationToken ct = default)
        {
            try
            {
                var iteration = await _repo.GetByIdAsync(id, ct);
                if (iteration == null)
                {
                    return new IterationServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Iteración no encontrada."
                    );
                }

                iteration.UpdateDetails(name, goal, startDate, endDate);
                iteration.SetCompletionPercentage(completionPercentage);
                await _repo.UpdateAsync(iteration, ct);

                return new IterationServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Iteración actualizada exitosamente.",
                    Iteration: iteration
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar iteración");
                return new IterationServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar la iteración."
                );
            }
        }

        public async Task<IterationServiceResult> SetCompletionAsync(int id, int percentage, CancellationToken ct = default)
        {
            try
            {
                var iteration = await _repo.GetByIdAsync(id, ct);
                if (iteration == null)
                {
                    return new IterationServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Iteración no encontrada."
                    );
                }

                iteration.SetCompletionPercentage(percentage);
                await _repo.UpdateAsync(iteration, ct);

                return new IterationServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Porcentaje de completitud actualizado.",
                    Iteration: iteration
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar completitud de iteración");
                return new IterationServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar el porcentaje de completitud."
                );
            }
        }

        public async Task<IterationServiceResult> DeleteIterationAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var iteration = await _repo.GetByIdAsync(id, ct);
                if (iteration == null)
                {
                    return new IterationServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Iteración no encontrada."
                    );
                }

                await _repo.DeleteAsync(id, ct);

                _logger.LogInformation("Iteración {IterationId} eliminada exitosamente", id);

                return new IterationServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Iteración eliminada exitosamente."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar iteración {IterationId}", id);
                return new IterationServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: $"Error al eliminar la iteración: {ex.Message}"
                );
            }
        }
    }
}

