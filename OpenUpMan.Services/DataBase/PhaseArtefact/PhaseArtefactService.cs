using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Data.Repositories;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class PhaseArtefactService : IPhaseArtefactService
    {
        private readonly IPhaseArtefactRepository _repo;
        private readonly ILogger<PhaseArtefactService> _logger;

        public PhaseArtefactService(IPhaseArtefactRepository repo, ILogger<PhaseArtefactService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResult<PhaseArtefact>> AssignArtefactToPhaseAsync(Guid phaseId, Guid artefactId, Guid? documentId = null, bool registrado = false, CancellationToken ct = default)
        {
            try
            {
                if (phaseId == Guid.Empty)
                {
                    return new ServiceResult<PhaseArtefact>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de fase inválido.",
                        Data: null
                    );
                }

                if (artefactId == Guid.Empty)
                {
                    return new ServiceResult<PhaseArtefact>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de artefacto inválido.",
                        Data: null
                    );
                }

                var exists = await _repo.ExistsAsync(phaseId, artefactId);
                if (exists)
                {
                    return new ServiceResult<PhaseArtefact>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El artefacto ya está asignado a esta fase.",
                        Data: null
                    );
                }

                var phaseArtefact = new PhaseArtefact(phaseId, artefactId, documentId, registrado);
                await _repo.AddAsync(phaseArtefact);

                _logger.LogInformation("Artefacto {ArtefactId} asignado a la fase {PhaseId}", artefactId, phaseId);

                return new ServiceResult<PhaseArtefact>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Artefacto asignado a la fase exitosamente.",
                    Data: phaseArtefact
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar artefacto {ArtefactId} a la fase {PhaseId}", artefactId, phaseId);
                return new ServiceResult<PhaseArtefact>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al asignar el artefacto a la fase.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<IEnumerable<Artefact>>> GetArtefactsByPhaseIdAsync(Guid phaseId, CancellationToken ct = default)
        {
            try
            {
                if (phaseId == Guid.Empty)
                {
                    return new ServiceResult<IEnumerable<Artefact>>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de fase inválido.",
                        Data: null
                    );
                }

                var artefacts = await _repo.GetArtefactsByPhaseIdAsync(phaseId);
                var artefactsList = artefacts.ToList();
                return new ServiceResult<IEnumerable<Artefact>>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: $"Se encontraron {artefactsList.Count} artefactos para la fase.",
                    Data: artefactsList
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener artefactos de la fase {PhaseId}", phaseId);
                return new ServiceResult<IEnumerable<Artefact>>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener los artefactos de la fase.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<IEnumerable<PhaseArtefact>>> GetPhaseArtefactsByPhaseIdAsync(Guid phaseId, CancellationToken ct = default)
        {
            try
            {
                if (phaseId == Guid.Empty)
                {
                    return new ServiceResult<IEnumerable<PhaseArtefact>>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de fase inválido.",
                        Data: null
                    );
                }

                var phaseArtefacts = await _repo.GetByPhaseIdAsync(phaseId);
                var phaseArtefactsList = phaseArtefacts.ToList();
                return new ServiceResult<IEnumerable<PhaseArtefact>>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: $"Se encontraron {phaseArtefactsList.Count} asignaciones de artefactos.",
                    Data: phaseArtefactsList
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asignaciones de artefactos de la fase {PhaseId}", phaseId);
                return new ServiceResult<IEnumerable<PhaseArtefact>>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener las asignaciones de artefactos.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<PhaseArtefact>> GetPhaseArtefactAsync(Guid phaseId, Guid artefactId, CancellationToken ct = default)
        {
            try
            {
                if (phaseId == Guid.Empty || artefactId == Guid.Empty)
                {
                    return new ServiceResult<PhaseArtefact>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "IDs inválidos.",
                        Data: null
                    );
                }

                var phaseArtefact = await _repo.GetByIdAsync(phaseId, artefactId);
                if (phaseArtefact == null)
                {
                    return new ServiceResult<PhaseArtefact>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Asignación de artefacto no encontrada.",
                        Data: null
                    );
                }

                return new ServiceResult<PhaseArtefact>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Asignación de artefacto encontrada.",
                    Data: phaseArtefact
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asignación de artefacto {ArtefactId} en fase {PhaseId}", artefactId, phaseId);
                return new ServiceResult<PhaseArtefact>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener la asignación de artefacto.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<PhaseArtefact>> UpdatePhaseArtefactAsync(Guid phaseId, Guid artefactId, Guid? documentId, bool registrado, CancellationToken ct = default)
        {
            try
            {
                if (phaseId == Guid.Empty || artefactId == Guid.Empty)
                {
                    return new ServiceResult<PhaseArtefact>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "IDs inválidos.",
                        Data: null
                    );
                }

                var phaseArtefact = await _repo.GetByIdAsync(phaseId, artefactId);
                if (phaseArtefact == null)
                {
                    return new ServiceResult<PhaseArtefact>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Asignación de artefacto no encontrada.",
                        Data: null
                    );
                }

                phaseArtefact.SetDocument(documentId);
                phaseArtefact.SetRegistrado(registrado);
                await _repo.UpdateAsync(phaseArtefact);

                _logger.LogInformation("Asignación de artefacto actualizada: Fase {PhaseId}, Artefacto {ArtefactId}", phaseId, artefactId);

                return new ServiceResult<PhaseArtefact>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Asignación de artefacto actualizada exitosamente.",
                    Data: phaseArtefact
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar asignación de artefacto {ArtefactId} en fase {PhaseId}", artefactId, phaseId);
                return new ServiceResult<PhaseArtefact>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar la asignación de artefacto.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<PhaseArtefact>> MarkAsRegisteredAsync(Guid phaseId, Guid artefactId, CancellationToken ct = default)
        {
            try
            {
                if (phaseId == Guid.Empty || artefactId == Guid.Empty)
                {
                    return new ServiceResult<PhaseArtefact>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "IDs inválidos.",
                        Data: null
                    );
                }

                var phaseArtefact = await _repo.GetByIdAsync(phaseId, artefactId);
                if (phaseArtefact == null)
                {
                    return new ServiceResult<PhaseArtefact>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Asignación de artefacto no encontrada.",
                        Data: null
                    );
                }

                phaseArtefact.MarkAsRegistered();
                await _repo.UpdateAsync(phaseArtefact);

                _logger.LogInformation("Artefacto {ArtefactId} marcado como registrado en fase {PhaseId}", artefactId, phaseId);

                return new ServiceResult<PhaseArtefact>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Artefacto marcado como registrado exitosamente.",
                    Data: phaseArtefact
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar artefacto {ArtefactId} como registrado en fase {PhaseId}", artefactId, phaseId);
                return new ServiceResult<PhaseArtefact>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al marcar el artefacto como registrado.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<bool>> RemoveArtefactFromPhaseAsync(Guid phaseId, Guid artefactId, CancellationToken ct = default)
        {
            try
            {
                if (phaseId == Guid.Empty || artefactId == Guid.Empty)
                {
                    return new ServiceResult<bool>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "IDs inválidos.",
                        Data: false
                    );
                }

                var exists = await _repo.ExistsAsync(phaseId, artefactId);
                if (!exists)
                {
                    return new ServiceResult<bool>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Asignación de artefacto no encontrada.",
                        Data: false
                    );
                }

                await _repo.DeleteAsync(phaseId, artefactId);

                _logger.LogInformation("Artefacto {ArtefactId} removido de la fase {PhaseId}", artefactId, phaseId);

                return new ServiceResult<bool>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Artefacto removido de la fase exitosamente.",
                    Data: true
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover artefacto {ArtefactId} de la fase {PhaseId}", artefactId, phaseId);
                return new ServiceResult<bool>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al remover el artefacto de la fase.",
                    Data: false
                );
            }
        }
    }
}

