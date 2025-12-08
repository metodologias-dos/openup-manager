﻿using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using System.Linq;

namespace OpenUpMan.Services
{
    public class PhaseService : IPhaseService
    {
        private readonly IPhaseRepository _repo;
        private readonly ILogger<PhaseService> _logger;
        private readonly IArtifactRepository _artifactRepo;

        public PhaseService(IPhaseRepository repo, ILogger<PhaseService> logger, IArtifactRepository artifactRepo)
        {
            _repo = repo;
            _logger = logger;
            _artifactRepo = artifactRepo;
        }

        public async Task<PhaseServiceResult> CreatePhaseAsync(int projectId, string name, int orderIndex, string status = "PENDING", 
            string? objective = null, string? scope = null, string? observations = null, CancellationToken ct = default)
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
                
                // Establecer los campos opcionales
                if (!string.IsNullOrWhiteSpace(objective))
                    phase.SetObjective(objective);
                
                if (!string.IsNullOrWhiteSpace(scope))
                    phase.SetScope(scope);
                
                if (!string.IsNullOrWhiteSpace(observations))
                    phase.SetObservations(observations);
                
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

        public async Task<PhaseServiceResult> UpdatePhaseAsync(int id, string name, DateTime? startDate, DateTime? endDate, string status, 
            int? orderIndex = null, string? objective = null, string? scope = null, string? observations = null, CancellationToken ct = default)
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

                // Usar orderIndex proporcionado o mantener el existente
                var finalOrderIndex = orderIndex ?? phase.OrderIndex;
                phase.UpdateDetails(name, startDate, endDate, finalOrderIndex, objective, scope, observations);
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

        public async Task<PhaseServiceResult> UpdatePhaseObjectiveAsync(int id, string? objective, CancellationToken ct = default)
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

                phase.SetObjective(objective);
                await _repo.UpdateAsync(phase, ct);

                _logger.LogInformation("Objetivo de fase {PhaseId} actualizado", id);

                return new PhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Objetivo de la fase actualizado exitosamente.",
                    Phase: phase
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar objetivo de fase {PhaseId}", id);
                return new PhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar el objetivo de la fase."
                );
            }
        }

        public async Task<PhaseServiceResult> UpdatePhaseScopeAsync(int id, string? scope, CancellationToken ct = default)
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

                phase.SetScope(scope);
                await _repo.UpdateAsync(phase, ct);

                _logger.LogInformation("Alcance de fase {PhaseId} actualizado", id);

                return new PhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Alcance de la fase actualizado exitosamente.",
                    Phase: phase
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar alcance de fase {PhaseId}", id);
                return new PhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar el alcance de la fase."
                );
            }
        }

        public async Task<PhaseServiceResult> UpdatePhaseObservationsAsync(int id, string? observations, CancellationToken ct = default)
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

                phase.SetObservations(observations);
                await _repo.UpdateAsync(phase, ct);

                _logger.LogInformation("Observaciones de fase {PhaseId} actualizadas", id);

                return new PhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Observaciones de la fase actualizadas exitosamente.",
                    Phase: phase
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar observaciones de fase {PhaseId}", id);
                return new PhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar las observaciones de la fase."
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

        public async Task<PhaseServiceResult> StartPhaseAsync(int phaseId, int projectId, CancellationToken ct = default)
        {
            try
            {
                var phase = await _repo.GetByIdAsync(phaseId, ct);
                if (phase == null)
                {
                    return new PhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Fase no encontrada."
                    );
                }

                // Si la fase ya está iniciada o terminada, no permitir iniciarla de nuevo
                if (phase.Status == "IN_PROGRESS")
                {
                    return new PhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Warning,
                        Message: "La fase ya está en progreso."
                    );
                }

                if (phase.Status == "DONE")
                {
                    return new PhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Warning,
                        Message: "La fase ya está terminada."
                    );
                }

                // Obtener todas las fases del proyecto ordenadas
                var allPhases = (await _repo.GetByProjectIdAsync(projectId, ct))
                    .OrderBy(p => p.OrderIndex ?? 0)
                    .ToList();

                // La fase de Incepción NO requiere validación de fase anterior
                // Validar por nombre o por orderIndex
                var isInceptionPhase = phase.Name.Contains("Inception", StringComparison.OrdinalIgnoreCase) || 
                                       phase.Name.Contains("Inicio", StringComparison.OrdinalIgnoreCase) ||
                                       (phase.OrderIndex ?? 0) == 0;
                
                // Si NO es la fase de Incepción, validar que la fase anterior esté terminada
                if (!isInceptionPhase)
                {
                    var currentOrderIndex = phase.OrderIndex ?? 0;
                    var previousPhase = allPhases.FirstOrDefault(p => (p.OrderIndex ?? 0) == currentOrderIndex - 1);
                    
                    if (previousPhase != null && previousPhase.Status != "DONE")
                    {
                        return new PhaseServiceResult(
                            Success: false,
                            ResultType: ServiceResultType.Warning,
                            Message: "No es posible iniciar esta fase sin terminar la fase anterior. Por favor, termine la fase anterior para poder iniciar esta fase."
                        );
                    }
                }

                // Actualizar la fase: establecer fecha de inicio y cambiar estado
                phase.SetDates(DateTime.Now, phase.EndDate);
                phase.SetStatus("IN_PROGRESS");
                await _repo.UpdateAsync(phase, ct);

                _logger.LogInformation("Fase {PhaseId} iniciada exitosamente", phaseId);

                return new PhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Fase iniciada exitosamente.",
                    Phase: phase
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar fase {PhaseId}", phaseId);
                return new PhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al iniciar la fase."
                );
            }
        }

        public async Task<PhaseServiceResult> EndPhaseAsync(int phaseId, CancellationToken ct = default)
        {
            try
            {
                var phase = await _repo.GetByIdAsync(phaseId, ct);
                if (phase == null)
                {
                    return new PhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Fase no encontrada."
                    );
                }

                // Validar que la fase esté en progreso
                if (phase.Status != "IN_PROGRESS")
                {
                    return new PhaseServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Warning,
                        Message: "Solo se pueden finalizar fases que estén en progreso."
                    );
                }

                // Validar que todos los artefactos obligatorios tengan al menos una versión con file_blob
                var mandatoryArtifacts = await _artifactRepo.GetByPhaseIdAsync(phaseId, ct);
                var mandatoryOnly = mandatoryArtifacts.Where(a => a.Mandatory).ToList();

                foreach (var artifact in mandatoryOnly)
                {
                    var versions = await _artifactRepo.GetVersionHistoryAsync(artifact.Id, ct);
                    var hasFileUploaded = versions.Any(v => v.FileBlob != null && v.FileBlob.Length > 0);
                    
                    if (!hasFileUploaded)
                    {
                        return new PhaseServiceResult(
                            Success: false,
                            ResultType: ServiceResultType.Warning,
                            Message: $"No se puede finalizar la fase. El artefacto obligatorio '{artifact.Name}' no tiene ninguna versión cargada."
                        );
                    }
                }
                
                // Actualizar la fase: establecer fecha de fin y cambiar estado
                phase.SetDates(phase.StartDate, DateTime.Now);
                phase.SetStatus("DONE");
                await _repo.UpdateAsync(phase, ct);

                _logger.LogInformation("Fase {PhaseId} finalizada exitosamente", phaseId);

                return new PhaseServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Fase finalizada exitosamente.",
                    Phase: phase
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al finalizar fase {PhaseId}", phaseId);
                return new PhaseServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al finalizar la fase."
                );
            }
        }
    }
}
