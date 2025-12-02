using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Role?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Roles.FindAsync(new object[] { id }, ct);
        }

        public async Task<Role?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == name, ct);
        }

        public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Roles
                .OrderBy(r => r.Name)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Role>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
        {
            return await _context.Roles
                .Where(r => ids.Contains(r.Id))
                .ToListAsync(ct);
        }

        public async Task AddAsync(Role role, CancellationToken ct = default)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            await _context.Roles.AddAsync(role, ct);
        }

        public Task UpdateAsync(Role role, CancellationToken ct = default)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            _context.Roles.Update(role);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var role = await GetByIdAsync(id, ct);
            if (role != null)
            {
                _context.Roles.Remove(role);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Roles.AnyAsync(r => r.Id == id, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}

