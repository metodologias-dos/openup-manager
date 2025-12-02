namespace OpenUpMan.Domain
{
    public class Project
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Code { get; private set; }
        public string? Description { get; private set; }
        public DateTime? StartDate { get; private set; }
        public string Status { get; private set; } = "CREATED";
        public int? CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        // Parameterless constructor for EF
        protected Project() { }

        public Project(string name, int? createdBy, string? code = null, string? description = null, DateTime? startDate = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Name = name;
            Code = code;
            Description = description;
            StartDate = startDate;
            CreatedBy = createdBy;
            Status = "CREATED";
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public void UpdateDetails(string name, string? description, DateTime? startDate, string? code = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Name = name;
            Description = description;
            StartDate = startDate;
            if (code != null) Code = code;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetStatus(string newStatus)
        {
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
