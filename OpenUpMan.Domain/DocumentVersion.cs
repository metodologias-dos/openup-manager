namespace OpenUpMan.Domain
{
    public class DocumentVersion
    {
        public Guid Id { get; private set; }
        public Guid DocumentId { get; private set; }
        public Guid CreatedBy { get; private set; }
        public int VersionNumber { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? Observations { get; private set; }
        public string Extension { get; private set; } = null!;
        public byte[] Binario { get; private set; } = null!;

        // Parameterless constructor for EF
        protected DocumentVersion() { }

        public DocumentVersion(
            Guid documentId, 
            int versionNumber, 
            Guid createdBy, 
            string extension,
            byte[] binario,
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

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentException("Extension cannot be null or empty.", nameof(extension));
            }

            if (binario == null || binario.Length == 0)
            {
                throw new ArgumentException("Binario cannot be null or empty.", nameof(binario));
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
            Extension = extension;
            Binario = binario;
            Observations = observations;
        }

        public void UpdateObservations(string? observations)
        {
            Observations = observations;
        }
    }
}

