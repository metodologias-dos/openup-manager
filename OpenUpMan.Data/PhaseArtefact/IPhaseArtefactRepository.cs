using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IPhaseArtefactRepository
    {
        Task<IEnumerable<Artefact>>GetArtefactsByPhaseIdAsync(Guid phaseId);
        Task<IEnumerable<PhaseArtefact>> GetByPhaseIdAsync(Guid phaseId);
        Task<PhaseArtefact?> GetByIdAsync(Guid phaseId, Guid artefactId);
        Task AddAsync(PhaseArtefact phaseArtefact);
        Task UpdateAsync(PhaseArtefact phaseArtefact);
        Task DeleteAsync(Guid phaseId, Guid artefactId);
        Task<bool> ExistsAsync(Guid phaseId, Guid artefactId);
    }
}

