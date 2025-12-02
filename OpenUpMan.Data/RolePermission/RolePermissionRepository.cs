using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly AppDbContext _context;

        public RolePermissionRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId, CancellationToken ct = default)
        {
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync(ct);
            
            var permissionIds = rolePermissions.Select(rp => rp.PermissionId);
            return await _context.Permissions
                .Where(p => permissionIds.Contains(p.Id))
                .OrderBy(p => p.Name)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(int roleId, CancellationToken ct = default)
        {
            return await GetPermissionsByRoleIdAsync(roleId, ct);
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByRolesAsync(IEnumerable<int> roleIds, CancellationToken ct = default)
        {
            var rolePermissions = await _context.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .ToListAsync(ct);
            
            var permissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct();
            return await _context.Permissions
                .Where(p => permissionIds.Contains(p.Id))
                .OrderBy(p => p.Name)
                .ToListAsync(ct);
        }

        public async Task<bool> RoleHasPermissionAsync(IEnumerable<int> roleIds, int permissionId, CancellationToken ct = default)
        {
            return await _context.RolePermissions
                .AnyAsync(rp => roleIds.Contains(rp.RoleId) && rp.PermissionId == permissionId, ct);
        }

        public async Task<IEnumerable<Role>> GetRolesByPermissionIdAsync(int permissionId, CancellationToken ct = default)
        {
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.PermissionId == permissionId)
                .ToListAsync(ct);
            
            var roleIds = rolePermissions.Select(rp => rp.RoleId);
            return await _context.Roles
                .Where(r => roleIds.Contains(r.Id))
                .OrderBy(r => r.Name)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<RolePermission>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.RolePermissions.ToListAsync(ct);
        }

        public async Task AddAsync(RolePermission rolePermission, CancellationToken ct = default)
        {
            if (rolePermission == null)
                throw new ArgumentNullException(nameof(rolePermission));

            await _context.RolePermissions.AddAsync(rolePermission, ct);
        }

        public async Task DeleteAsync(int roleId, int permissionId, CancellationToken ct = default)
        {
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, ct);

            if (rolePermission != null)
            {
                _context.RolePermissions.Remove(rolePermission);
            }
        }

        public async Task<bool> ExistsAsync(int roleId, int permissionId, CancellationToken ct = default)
        {
            return await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}

