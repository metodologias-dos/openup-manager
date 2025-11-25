namespace OpenUpMan.Domain
{
    public class Document
    {
        public Guid Id { get; private set; }
        public Guid PhaseItemId { get; private set; }
        public PhaseItem? PhaseItem { get; private set; }
        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }
        public Guid CreatedBy { get; private set; }
        public User? Creator { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int LastVersionNumber { get; private set; }

        // Parameterless constructor for EF
        protected Document() { }

        public Document(Guid phaseItemId, string title, Guid createdBy, string? description = null)
        {
            if (phaseItemId == Guid.Empty)
            {
                throw new ArgumentException("PhaseItemId cannot be an empty GUID.", nameof(phaseItemId));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            }

            if (createdBy == Guid.Empty)
            {
                throw new ArgumentException("CreatedBy cannot be an empty GUID.", nameof(createdBy));
            }

            Id = Guid.NewGuid();
            PhaseItemId = phaseItemId;
            Title = title;
            Description = description;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            LastVersionNumber = 0;
        }

        public void UpdateDetails(string title, string? description)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            }

            Title = title;
            Description = description;
        }

        public void IncrementVersion()
        {
            LastVersionNumber++;
        }
    }
}

