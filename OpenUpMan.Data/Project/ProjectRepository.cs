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

        public async Task<Project?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _ctx.Projects.FindAsync(new object[] { id }, ct);
        }

        public async Task<Project?> GetByCodeAsync(string code, CancellationToken ct = default)
        {
            return await _ctx.Projects.FirstOrDefaultAsync(p => p.Code == code, ct);
        }

        public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken ct = default)
        {
            return await _ctx.Projects
                .Where(p => p.DeletedAt == null)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Project>> GetByCreatorAsync(int createdBy, CancellationToken ct = default)
        {
            return await _ctx.Projects
                .Where(p => p.CreatedBy == createdBy && p.DeletedAt == null)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Project>> GetByStatusAsync(string status, CancellationToken ct = default)
        {
            return await _ctx.Projects
                .Where(p => p.Status == status && p.DeletedAt == null)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Project project, CancellationToken ct = default)
        {
            await _ctx.Projects.AddAsync(project, ct);
        }

        public Task UpdateAsync(Project project, CancellationToken ct = default)
        {
            _ctx.Projects.Update(project);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var project = await GetByIdAsync(id, ct);
            if (project != null)
            {
                project.SoftDelete();
            }
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _ctx.SaveChangesAsync(ct);
        }
    }
}
