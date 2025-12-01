namespace OpenUpMan.Domain
{
    public enum ProjectState
    {
        CREATED,
        ACTIVE,
        ARCHIVED,
        CLOSED
    }

    public class Project
    {
        public Guid Id { get; private set; }
        public string Identifier { get; private set; } = null!; // ej. "PROY-001"
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public DateTime StartDate { get; private set; }
        public Guid OwnerId { get; private set; }
        public ProjectState State { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Parameterless constructor for EF
        protected Project() { }

        public Project(string identifier, string name, DateTime startDate, Guid ownerId, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("Identifier cannot be null or empty.", nameof(identifier));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Id = Guid.NewGuid();
            Identifier = identifier;
            Name = name;
            StartDate = startDate;
            OwnerId = ownerId;
            Description = description;
            State = ProjectState.CREATED;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public void UpdateDetails(string name, string? description, DateTime startDate)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Name = name;
            Description = description;
            StartDate = startDate;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetState(ProjectState newState)
        {
            State = newState;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AssignOwner(Guid ownerId)
        {
            if (ownerId == Guid.Empty)
            {
                throw new ArgumentException("OwnerId cannot be an empty GUID.", nameof(ownerId));
            }

            OwnerId = ownerId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
