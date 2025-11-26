namespace OpenUpMan.Domain
{
    public enum ProjectUserPermission
    {
        VIEWER,
        EDITOR,
        OWNER
    }

    public enum ProjectUserRole
    {
        AUTOR,
        REVISOR,
        PO,
        SM,
        DESARROLLADOR,
        TESTER,
        ADMIN
    }

    public class ProjectUser
    {
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public Guid UserId { get; private set; }
        public User? User { get; private set; }
        public ProjectUserPermission Permissions { get; private set; }
        public ProjectUserRole Role { get; private set; }

        // Parameterless constructor for EF
        protected ProjectUser() { }

        public ProjectUser(Guid projectId, Guid userId, ProjectUserPermission permissions = ProjectUserPermission.VIEWER, ProjectUserRole role = ProjectUserRole.AUTOR)
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
            Permissions = permissions;
            Role = role;
        }

        public void SetPermissions(ProjectUserPermission newPermissions)
        {
            Permissions = newPermissions;
        }

        public void SetRole(ProjectUserRole newRole)
        {
            Role = newRole;
        }
    }
}

