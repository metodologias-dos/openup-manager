using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Permission?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Permissions.FindAsync(new object[] { id }, ct);
        }

        public async Task<Permission?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == name, ct);
        }

        public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Permissions
                .OrderBy(p => p.Name)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Permission permission, CancellationToken ct = default)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            await _context.Permissions.AddAsync(permission, ct);
        }

        public Task UpdateAsync(Permission permission, CancellationToken ct = default)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            _context.Permissions.Update(permission);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var permission = await GetByIdAsync(id, ct);
            if (permission != null)
            {
                _context.Permissions.Remove(permission);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Permissions.AnyAsync(p => p.Id == id, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}

