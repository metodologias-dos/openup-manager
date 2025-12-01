using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IArtefactService
    {
        Task<ServiceResult<Artefact>> CreateArtefactAsync(string name, string? description = null, CancellationToken ct = default);
        Task<ServiceResult<Artefact>> GetArtefactByIdAsync(Guid id, CancellationToken ct = default);
        Task<ServiceResult<Artefact>> GetArtefactByNameAsync(string name, CancellationToken ct = default);
        Task<ServiceResult<IEnumerable<Artefact>>> GetAllArtefactsAsync(CancellationToken ct = default);
        Task<ServiceResult<Artefact>> UpdateArtefactAsync(Guid id, string name, string? description = null, CancellationToken ct = default);
        Task<ServiceResult<bool>> DeleteArtefactAsync(Guid id, CancellationToken ct = default);
    }
}

