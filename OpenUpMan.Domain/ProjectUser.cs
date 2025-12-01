namespace OpenUpMan.Domain
{
    public class ProjectUser
    {
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public Guid UserId { get; private set; }
        public User? User { get; private set; }
        public Guid RoleId { get; private set; }
        public Role? Role { get; private set; }

        // Parameterless constructor for EF
        protected ProjectUser() { }

        public ProjectUser(Guid projectId, Guid userId, Guid roleId)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentException("ProjectId cannot be an empty GUID.", nameof(projectId));
            }

            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be an empty GUID.", nameof(userId));
            }

            if (roleId == Guid.Empty)
            {
                throw new ArgumentException("RoleId cannot be an empty GUID.", nameof(roleId));
            }

            ProjectId = projectId;
            UserId = userId;
            RoleId = roleId;
        }

        public void SetRole(Guid newRoleId)
        {
            if (newRoleId == Guid.Empty)
            {
                throw new ArgumentException("RoleId cannot be an empty GUID.", nameof(newRoleId));
            }

            RoleId = newRoleId;
        }
    }
}

