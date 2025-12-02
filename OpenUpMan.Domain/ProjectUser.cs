namespace OpenUpMan.Domain
{
    public class ProjectUser
    {
        public int Id { get; private set; }
        public int ProjectId { get; private set; }
        public int UserId { get; private set; }
        public int RoleId { get; private set; }
        public DateTime AddedAt { get; private set; }

        // Parameterless constructor for EF
        protected ProjectUser() { }

        public ProjectUser(int projectId, int userId, int roleId)
        {
            if (projectId <= 0)
            {
                throw new ArgumentException("ProjectId must be positive.", nameof(projectId));
            }

            if (userId <= 0)
            {
                throw new ArgumentException("UserId must be positive.", nameof(userId));
            }

            if (roleId <= 0)
            {
                throw new ArgumentException("RoleId must be positive.", nameof(roleId));
            }

            ProjectId = projectId;
            UserId = userId;
            RoleId = roleId;
            AddedAt = DateTime.UtcNow;
        }

        public void SetRole(int newRoleId)
        {
            if (newRoleId <= 0)
            {
                throw new ArgumentException("RoleId must be positive.", nameof(newRoleId));
            }

            RoleId = newRoleId;
        }
    }
}

