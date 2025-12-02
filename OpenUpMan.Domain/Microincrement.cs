namespace OpenUpMan.Domain
{
    public class Microincrement
    {
        public int Id { get; private set; }
        public int IterationId { get; private set; }
        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }
        public DateTime Date { get; private set; }
        public int? AuthorId { get; private set; }
        public string Type { get; private set; } = "functional";
        public int? ArtifactId { get; private set; }
        public string? EvidenceUrl { get; private set; }

        // Parameterless constructor for EF
        protected Microincrement() { }

        public Microincrement(int iterationId, string title, int? authorId, string type = "functional", string? description = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            }

            IterationId = iterationId;
            Title = title;
            Description = description;
            AuthorId = authorId;
            Type = type;
            Date = DateTime.UtcNow;
        }

        public void UpdateDetails(string title, string? description, string type)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            }

            Title = title;
            Description = description;
            Type = type;
        }

        public void SetArtifact(int artifactId)
        {
            ArtifactId = artifactId;
        }

        public void SetEvidenceUrl(string? url)
        {
            EvidenceUrl = url;
        }
    }
}

