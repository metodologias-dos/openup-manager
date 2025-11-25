namespace OpenUpMan.Domain
{
    public enum ProjectUserRole
    {
        VIEWER,
        EDITOR,
        OWNER
    }

    public class ProjectUser
    {
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public Guid UserId { get; private set; }
        public User? User { get; private set; }
        public ProjectUserRole Role { get; private set; }

        // Parameterless constructor for EF
        protected ProjectUser() { }

        public ProjectUser(Guid projectId, Guid userId, ProjectUserRole role = ProjectUserRole.VIEWER)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentException("ProjectId cannot be an empty GUID.", nameof(projectId));
            }

            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be an empty GUID.", nameof(userId));
            }

            ProjectId = projectId;
            UserId = userId;
            Role = role;
        }

        public void SetRole(ProjectUserRole newRole)
        {
            Role = newRole;
        }
    }
}

