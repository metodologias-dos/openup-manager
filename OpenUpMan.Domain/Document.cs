namespace OpenUpMan.Domain
{
    public class Document
    {
        public Guid Id { get; private set; }
        public Guid PhaseItemId { get; private set; }
        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }

        // Parameterless constructor for EF
        protected Document() { }

        public Document(Guid phaseItemId, string title, string? description = null)
        {
            if (phaseItemId == Guid.Empty)
            {
                throw new ArgumentException("PhaseItemId cannot be an empty GUID.", nameof(phaseItemId));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            }

            Id = Guid.NewGuid();
            PhaseItemId = phaseItemId;
            Title = title;
            Description = description;
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
    }
}

