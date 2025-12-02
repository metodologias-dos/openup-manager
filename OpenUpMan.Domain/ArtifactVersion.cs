namespace OpenUpMan.Domain
{
    public class ArtifactVersion
    {
        public int Id { get; private set; }
        public int ArtifactId { get; private set; }
        public int VersionNumber { get; private set; }
        public int? CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? Notes { get; private set; }
        public byte[]? FileBlob { get; private set; }
        public string? FileMime { get; private set; }
        public string? BuildInfo { get; private set; }

        // Parameterless constructor for EF
        protected ArtifactVersion() { }

        public ArtifactVersion(int artifactId, int? createdBy, string? notes = null, byte[]? fileBlob = null, string? fileMime = null, string? buildInfo = null)
        {
            ArtifactId = artifactId;
            CreatedBy = createdBy;
            Notes = notes;
            FileBlob = fileBlob;
            FileMime = fileMime;
            BuildInfo = buildInfo;
            CreatedAt = DateTime.UtcNow;
            VersionNumber = 0; // Will be set by database trigger
        }

        public void UpdateFile(byte[]? fileBlob, string? fileMime)
        {
            FileBlob = fileBlob;
            FileMime = fileMime;
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }

        public void UpdateBuildInfo(string? buildInfo)
        {
            BuildInfo = buildInfo;
        }
    }
}

