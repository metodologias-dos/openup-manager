using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class ProjectPhaseRepository : IProjectPhaseRepository
    {
        private readonly AppDbContext _ctx;

        public ProjectPhaseRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ProjectPhase?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _ctx.ProjectPhases
                .Include(pp => pp.Project)
                .FirstOrDefaultAsync(pp => pp.Id == id, ct);
        }

        public async Task<IEnumerable<ProjectPhase>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default)
        {
            return await _ctx.ProjectPhases
                .Where(pp => pp.ProjectId == projectId)
                .OrderBy(pp => pp.Order)
                .ToListAsync(ct);
        }

        public async Task<ProjectPhase?> GetByProjectIdAndCodeAsync(Guid projectId, PhaseCode code, CancellationToken ct = default)
        {
            return await _ctx.ProjectPhases
                .FirstOrDefaultAsync(pp => pp.ProjectId == projectId && pp.Code == code, ct);
        }

        public async Task AddAsync(ProjectPhase projectPhase, CancellationToken ct = default)
        {
            await _ctx.ProjectPhases.AddAsync(projectPhase, ct);
        }

        public async Task UpdateAsync(ProjectPhase projectPhase, CancellationToken ct = default)
        {
            _ctx.ProjectPhases.Update(projectPhase);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _ctx.SaveChangesAsync(ct);
        }
    }
}

