namespace OpenUpMan.Domain
{
    public class RolePermission
    {
        public Guid RoleId { get; private set; }
        public Role? Role { get; private set; }
        public Guid PermissionId { get; private set; }
        public Permission? Permission { get; private set; }

        // Parameterless constructor for EF
        protected RolePermission() { }

        public RolePermission(Guid roleId, Guid permissionId)
        {
            if (roleId == Guid.Empty)
            {
                throw new ArgumentException("RoleId cannot be an empty GUID.", nameof(roleId));
            }

            if (permissionId == Guid.Empty)
            {
                throw new ArgumentException("PermissionId cannot be an empty GUID.", nameof(permissionId));
            }

            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
}

