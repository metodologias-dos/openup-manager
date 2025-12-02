namespace OpenUpMan.Domain
{
    public class Artifact
    {
        public int Id { get; private set; }
        public int ProjectId { get; private set; }
        public int PhaseId { get; private set; }
        public string Name { get; private set; } = null!;
        public string? ArtifactType { get; private set; }
        public bool Mandatory { get; private set; }
        public string? Description { get; private set; }
        public string CurrentState { get; private set; } = "PENDING";

        // Parameterless constructor for EF
        protected Artifact() { }

        public Artifact(int projectId, int phaseId, string name, string? artifactType = null, bool mandatory = false, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            ProjectId = projectId;
            PhaseId = phaseId;
            Name = name;
            ArtifactType = artifactType;
            Mandatory = mandatory;
            Description = description;
            CurrentState = "PENDING";
        }

        public void UpdateDetails(string name, string? artifactType, bool mandatory, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Name = name;
            ArtifactType = artifactType;
            Mandatory = mandatory;
            Description = description;
        }

        public void SetState(string state)
        {
            CurrentState = state;
        }
    }
}

