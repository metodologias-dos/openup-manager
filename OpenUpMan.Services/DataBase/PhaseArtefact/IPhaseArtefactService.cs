using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IPhaseArtefactService
    {
        Task<ServiceResult<PhaseArtefact>> AssignArtefactToPhaseAsync(Guid phaseId, Guid artefactId, Guid? documentId = null, bool registrado = false, CancellationToken ct = default);
        Task<ServiceResult<IEnumerable<Artefact>>> GetArtefactsByPhaseIdAsync(Guid phaseId, CancellationToken ct = default);
        Task<ServiceResult<IEnumerable<PhaseArtefact>>> GetPhaseArtefactsByPhaseIdAsync(Guid phaseId, CancellationToken ct = default);
        Task<ServiceResult<PhaseArtefact>> GetPhaseArtefactAsync(Guid phaseId, Guid artefactId, CancellationToken ct = default);
        Task<ServiceResult<PhaseArtefact>> UpdatePhaseArtefactAsync(Guid phaseId, Guid artefactId, Guid? documentId, bool registrado, CancellationToken ct = default);
        Task<ServiceResult<PhaseArtefact>> MarkAsRegisteredAsync(Guid phaseId, Guid artefactId, CancellationToken ct = default);
        Task<ServiceResult<bool>> RemoveArtefactFromPhaseAsync(Guid phaseId, Guid artefactId, CancellationToken ct = default);
    }
}

