using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class ProjectPhaseService : IProjectPhaseService
    {
        private readonly IProjectPhaseRepository _repo;
        private readonly IProjectRepository _projectRepo;
        private readonly ILogger<ProjectPhaseService> _logger;

        public ProjectPhaseService(
            IProjectPhaseRepository repo,
            IProjectRepository projectRepo,
            ILogger<ProjectPhaseService> logger)
        {
            _repo = repo;
            _projectRepo = projectRepo;
            _logger = logger;
        }

        public async Task<ProjectPhaseServiceResult> CreatePhaseAsync(Guid projectId, PhaseCode code, string name, int order, CancellationToken ct = default)
        {
            try
            {
                var existing = await _repo.GetByProjectIdAndCodeAsync(projectId, code, ct);
                if (existing != null)
                {
                    return new ProjectPhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Ya existe una fase con ese código en el proyecto."
                    );
                }

                var phase = new ProjectPhase(projectId, code, name, order);
                await _repo.AddAsync(phase, ct);
                await _repo.SaveChangesAsync(ct);

                _logger.LogInformation("Fase creada: {PhaseId} - {Code}", phase.Id, code);

                return new ProjectPhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Fase creada exitosamente.",
                    ProjectPhase: phase
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear fase");
                return new ProjectPhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear la fase."
                );
            }
        }

        public async Task<ProjectPhaseServiceResult> GetPhaseByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var phase = await _repo.GetByIdAsync(id, ct);
                if (phase == null)
                {
                    return new ProjectPhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Fase no encontrada."
                    );
                }

                return new ProjectPhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Fase encontrada.",
                    ProjectPhase: phase
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener fase");
                return new ProjectPhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener la fase."
                );
            }
        }

        public async Task<IEnumerable<ProjectPhase>> GetPhasesByProjectAsync(Guid projectId, CancellationToken ct = default)
        {
            return await _repo.GetByProjectIdAsync(projectId, ct);
        }

        public async Task<ProjectPhaseServiceResult> UpdatePhaseAsync(Guid id, string name, int order, CancellationToken ct = default)
        {
            try
            {
                var phase = await _repo.GetByIdAsync(id, ct);
                if (phase == null)
                {
                    return new ProjectPhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Fase no encontrada."
                    );
                }

                phase.UpdateDetails(name, order);
                await _repo.UpdateAsync(phase, ct);
                await _repo.SaveChangesAsync(ct);

                return new ProjectPhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Fase actualizada exitosamente.",
                    ProjectPhase: phase
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar fase");
                return new ProjectPhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar la fase."
                );
            }
        }

        public async Task<ProjectPhaseServiceResult> ChangePhaseStateAsync(Guid id, PhaseState newState, CancellationToken ct = default)
        {
            try
            {
                var phase = await _repo.GetByIdAsync(id, ct);
                if (phase == null)
                {
                    return new ProjectPhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Fase no encontrada."
                    );
                }

                phase.SetState(newState);
                await _repo.UpdateAsync(phase, ct);
                await _repo.SaveChangesAsync(ct);

                return new ProjectPhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Estado de la fase actualizado.",
                    ProjectPhase: phase
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de fase");
                return new ProjectPhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al cambiar el estado de la fase."
                );
            }
        }

        public async Task<ProjectPhaseServiceResult> InitializeProjectPhasesAsync(Guid projectId, CancellationToken ct = default)
        {
            try
            {
                var phases = new[]
                {
                    new ProjectPhase(projectId, PhaseCode.INCEPTION, "Inicio", 1),
                    new ProjectPhase(projectId, PhaseCode.ELABORATION, "Elaboración", 2),
                    new ProjectPhase(projectId, PhaseCode.CONSTRUCTION, "Construcción", 3),
                    new ProjectPhase(projectId, PhaseCode.TRANSITION, "Transición", 4)
                };

                foreach (var phase in phases)
                {
                    await _repo.AddAsync(phase, ct);
                }
                await _repo.SaveChangesAsync(ct);

                _logger.LogInformation("Fases inicializadas para proyecto {ProjectId}", projectId);

                return new ProjectPhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Fases inicializadas exitosamente."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inicializar fases");
                return new ProjectPhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al inicializar las fases."
                );
            }
        }
    }
}

