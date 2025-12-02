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

        public async Task<ProjectUser?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _ctx.ProjectUsers.FindAsync(new object[] { id }, ct);
        }

        public async Task<ProjectUser?> GetByProjectAndUserAsync(int projectId, int userId, CancellationToken ct = default)
        {
            return await _ctx.ProjectUsers
                .FirstOrDefaultAsync(pu => pu.ProjectId == projectId && pu.UserId == userId, ct);
        }

        public async Task<IEnumerable<ProjectUser>> GetByProjectIdAsync(int projectId, CancellationToken ct = default)
        {
            return await _ctx.ProjectUsers
                .Where(pu => pu.ProjectId == projectId)
                .OrderBy(pu => pu.AddedAt)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<ProjectUser>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _ctx.ProjectUsers
                .Where(pu => pu.UserId == userId)
                .OrderBy(pu => pu.AddedAt)
                .ToListAsync(ct);
        }

        public async Task AddAsync(ProjectUser projectUser, CancellationToken ct = default)
        {
            await _ctx.ProjectUsers.AddAsync(projectUser, ct);
        }

        public Task UpdateAsync(ProjectUser projectUser, CancellationToken ct = default)
        {
            _ctx.ProjectUsers.Update(projectUser);
            return Task.CompletedTask;
        }

        public async Task RemoveAsync(int id, CancellationToken ct = default)
        {
            var projectUser = await GetByIdAsync(id, ct);
            if (projectUser != null)
            {
                _ctx.ProjectUsers.Remove(projectUser);
            }
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _ctx.SaveChangesAsync(ct);
        }
    }
}

