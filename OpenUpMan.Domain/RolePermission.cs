namespace OpenUpMan.Domain
{
    public class RolePermission
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        // Parameterless constructor for EF and seed data
        public RolePermission() { }

        public RolePermission(int roleId, int permissionId)
        {
            if (roleId <= 0)
            {
                throw new ArgumentException("RoleId must be positive.", nameof(roleId));
            }

            if (permissionId <= 0)
            {
                throw new ArgumentException("PermissionId must be positive.", nameof(permissionId));
            }

            RoleId = roleId;
            PermissionId = permissionId;
        }
        
        public Permission? Permission { get; set; }
    }
}
