namespace OpenUpMan.Domain
{
    public class RolePermission
    {
        public int Id { get; private set; }
        public int RoleId { get; private set; }
        public int PermissionId { get; private set; }

        // Parameterless constructor for EF
        protected RolePermission() { }

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
    }
}

