
using OpenUpMan.Domain;

namespace OpenUpMan.Data.Repositories
{
    public interface IArtefactRepository
    {
        Task<Artefact?> GetByIdAsync(Guid id);
        Task<Artefact?> GetByNameAsync(string name);
        Task<IEnumerable<Artefact>> GetAllAsync();
        Task AddAsync(Artefact artefact);
        Task UpdateAsync(Artefact artefact);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}

