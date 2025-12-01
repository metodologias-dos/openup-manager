namespace OpenUpMan.Domain
{
    public enum PhaseCode
    {
        INCEPTION,
        ELABORATION,
        CONSTRUCTION,
        TRANSITION
    }

    public enum PhaseState
    {
        NOT_STARTED,
        IN_PROGRESS,
        COMPLETED
    }

    public class ProjectPhase
    {
        public Guid Id { get; private set; }
        public Guid ProjectId { get; private set; }
        public PhaseCode Code { get; private set; }
        public string Name { get; private set; } = null!;
        public int Order { get; private set; }
        public PhaseState State { get; private set; }

        // Parameterless constructor for EF
        protected ProjectPhase() { }

        public ProjectPhase(Guid projectId, PhaseCode code, string name, int order)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentException("ProjectId cannot be an empty GUID.", nameof(projectId));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            if (order < 0)
            {
                throw new ArgumentException("Order must be non-negative.", nameof(order));
            }

            Id = Guid.NewGuid();
            ProjectId = projectId;
            Code = code;
            Name = name;
            Order = order;
            State = PhaseState.NOT_STARTED;
        }

        public void UpdateDetails(string name, int order)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            if (order < 0)
            {
                throw new ArgumentException("Order must be non-negative.", nameof(order));
            }

            Name = name;
            Order = order;
        }

        public void SetState(PhaseState newState)
        {
            State = newState;
        }
    }
}

