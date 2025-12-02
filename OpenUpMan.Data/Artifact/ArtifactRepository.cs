using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class ArtifactRepository : IArtifactRepository
    {
        private readonly AppDbContext _context;

        public ArtifactRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Artifact?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Artifacts.FindAsync(new object[] { id }, ct);
        }

        public async Task<IEnumerable<Artifact>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Artifacts.ToListAsync(ct);
        }

        public async Task<IEnumerable<Artifact>> GetByProjectIdAsync(int projectId, CancellationToken ct = default)
        {
            return await _context.Artifacts
                .Where(a => a.ProjectId == projectId)
                .OrderBy(a => a.PhaseId)
                .ThenBy(a => a.Name)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Artifact>> GetByPhaseIdAsync(int phaseId, CancellationToken ct = default)
        {
            return await _context.Artifacts
                .Where(a => a.PhaseId == phaseId)
                .OrderBy(a => a.Name)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Artifact>> GetMandatoryByProjectIdAsync(int projectId, CancellationToken ct = default)
        {
            return await _context.Artifacts
                .Where(a => a.ProjectId == projectId && a.Mandatory)
                .OrderBy(a => a.PhaseId)
                .ThenBy(a => a.Name)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Artifact artifact, CancellationToken ct = default)
        {
            if (artifact == null)
                throw new ArgumentNullException(nameof(artifact));

            await _context.Artifacts.AddAsync(artifact, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Artifact artifact, CancellationToken ct = default)
        {
            if (artifact == null)
                throw new ArgumentNullException(nameof(artifact));

            _context.Artifacts.Update(artifact);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var artifact = await GetByIdAsync(id, ct);
            if (artifact != null)
            {
                _context.Artifacts.Remove(artifact);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Artifacts.AnyAsync(a => a.Id == id, ct);
        }

        // Artifact version operations
        public async Task<IEnumerable<ArtifactVersion>> GetVersionHistoryAsync(int artifactId, CancellationToken ct = default)
        {
            return await _context.ArtifactVersions
                .Where(av => av.ArtifactId == artifactId)
                .OrderByDescending(av => av.VersionNumber)
                .ToListAsync(ct);
        }

        public async Task<ArtifactVersion?> GetLatestVersionAsync(int artifactId, CancellationToken ct = default)
        {
            return await _context.ArtifactVersions
                .Where(av => av.ArtifactId == artifactId)
                .OrderByDescending(av => av.VersionNumber)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<ArtifactVersion?> GetVersionAsync(int artifactId, int versionNumber, CancellationToken ct = default)
        {
            return await _context.ArtifactVersions
                .FirstOrDefaultAsync(av => av.ArtifactId == artifactId && av.VersionNumber == versionNumber, ct);
        }

        public async Task<int> AddVersionAsync(ArtifactVersion version, CancellationToken ct = default)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            // Get the next version number
            var latestVersion = await GetLatestVersionAsync(version.ArtifactId, ct);
            var nextVersionNumber = latestVersion != null ? latestVersion.VersionNumber + 1 : 1;
            
            // Use reflection to set the version number (since it's private set)
            var versionNumberProperty = typeof(ArtifactVersion).GetProperty("VersionNumber");
            versionNumberProperty?.SetValue(version, nextVersionNumber);

            await _context.ArtifactVersions.AddAsync(version, ct);
            await _context.SaveChangesAsync(ct);
            
            return nextVersionNumber;
        }
    }
}

