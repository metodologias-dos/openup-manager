namespace OpenUpMan.Domain
{
    public class DocumentVersion
    {
        public Guid Id { get; private set; }
        public Guid DocumentId { get; private set; }
        public Document? Document { get; private set; }
        public int VersionNumber { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }
        public User? Creator { get; private set; }
        public string FilePath { get; private set; } = null!;
        public string? Observations { get; private set; }

        // Parameterless constructor for EF
        protected DocumentVersion() { }

        public DocumentVersion(
            Guid documentId, 
            int versionNumber, 
            Guid createdBy, 
            string filePath,
            string? observations = null)
        {
            if (documentId == Guid.Empty)
            {
                throw new ArgumentException("DocumentId cannot be an empty GUID.", nameof(documentId));
            }

            if (createdBy == Guid.Empty)
            {
                throw new ArgumentException("CreatedBy cannot be an empty GUID.", nameof(createdBy));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("FilePath cannot be null or empty.", nameof(filePath));
            }

            if (versionNumber < 1)
            {
                throw new ArgumentException("VersionNumber must be positive.", nameof(versionNumber));
            }

            Id = Guid.NewGuid();
            DocumentId = documentId;
            VersionNumber = versionNumber;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            FilePath = filePath;
            Observations = observations;
        }

        public void UpdateObservations(string? observations)
        {
            Observations = observations;
        }
    }
}

