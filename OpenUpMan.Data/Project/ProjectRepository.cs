using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _ctx;

        public ProjectRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(Project project, CancellationToken ct = default)
        {
            await _ctx.Set<Project>().AddAsync(project, ct);
        }

        public async Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _ctx.Set<Project>()
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<Project?> GetByIdentifierAsync(string identifier, CancellationToken ct = default)
        {
            return await _ctx.Set<Project>().FirstOrDefaultAsync(p => p.Identifier == identifier, ct);
       }

        public async Task<IEnumerable<Project>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default)
        {
            return await _ctx.Set<Project>().Where(p => p.OwnerId == ownerId).ToListAsync(ct);
        }

        public async Task UpdateAsync(Project project, CancellationToken ct = default)
        {
            _ctx.Set<Project>().Update(project);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Project project, CancellationToken ct = default)
        {
            _ctx.Set<Project>().Remove(project);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _ctx.SaveChangesAsync(ct);
        }
    }
}
