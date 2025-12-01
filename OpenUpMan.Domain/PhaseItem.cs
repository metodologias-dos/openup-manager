namespace OpenUpMan.Domain
{
    public enum PhaseItemType
    {
        ITERATION,
        MICROINCREMENT
    }

    public enum PhaseItemState
    {
        PLANNED,
        ACTIVE,
        DONE,
        CANCELLED
    }

    public class PhaseItem
    {
        public Guid Id { get; private set; }
        public Guid ProjectPhaseId { get; private set; }
        public PhaseItemType Type { get; private set; }
        public int Number { get; private set; }
        public Guid? ParentIterationId { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public PhaseItemState State { get; private set; }
        public Guid CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Parameterless constructor for EF
        protected PhaseItem() { }

        public PhaseItem(
            Guid projectPhaseId, 
            PhaseItemType type, 
            int number, 
            string name, 
            Guid createdBy,
            Guid? parentIterationId = null,
            string? description = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            if (projectPhaseId == Guid.Empty)
            {
                throw new ArgumentException("ProjectPhaseId cannot be an empty GUID.", nameof(projectPhaseId));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            if (createdBy == Guid.Empty)
            {
                throw new ArgumentException("CreatedBy cannot be an empty GUID.", nameof(createdBy));
            }

            if (number < 0)
            {
                throw new ArgumentException("Number must be non-negative.", nameof(number));
            }

            Id = Guid.NewGuid();
            ProjectPhaseId = projectPhaseId;
            Type = type;
            Number = number;
            ParentIterationId = parentIterationId;
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            State = PhaseItemState.PLANNED;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(string name, string? description, DateTime? startDate, DateTime? endDate)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
        }

        public void SetState(PhaseItemState newState)
        {
            State = newState;
        }

        public void SetParentIteration(Guid? parentIterationId)
        {
            ParentIterationId = parentIterationId;
        }
    }
}

