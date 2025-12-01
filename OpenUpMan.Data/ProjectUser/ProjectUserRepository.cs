using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class ProjectUserRepository : IProjectUserRepository
    {
        private readonly AppDbContext _ctx;

        public ProjectUserRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ProjectUser?> GetByIdAsync(Guid projectId, Guid userId, CancellationToken ct = default)
        {
            return await _ctx.ProjectUsers
                .Include(pu => pu.Project)
                .Include(pu => pu.User)
                .Include(pu => pu.Role)
                .FirstOrDefaultAsync(pu => pu.ProjectId == projectId && pu.UserId == userId, ct);
        }

        public async Task<IEnumerable<ProjectUser>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default)
        {
            return await _ctx.ProjectUsers
                .Include(pu => pu.User)
                .Include(pu => pu.Role)
                .Where(pu => pu.ProjectId == projectId)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<ProjectUser>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _ctx.ProjectUsers
                .Include(pu => pu.Project)
                .Include(pu => pu.Role)
                .Where(pu => pu.UserId == userId)
                .ToListAsync(ct);
        }

        public async Task AddAsync(ProjectUser projectUser, CancellationToken ct = default)
        {
            await _ctx.ProjectUsers.AddAsync(projectUser, ct);
        }

        public async Task RemoveAsync(ProjectUser projectUser, CancellationToken ct = default)
        {
            _ctx.ProjectUsers.Remove(projectUser);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _ctx.SaveChangesAsync(ct);
        }
    }
}

