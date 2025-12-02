namespace OpenUpMan.Domain
{
    public class Phase
    {
        public int Id { get; private set; }
        public int ProjectId { get; private set; }
        public string Name { get; private set; } = null!;
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public string Status { get; private set; } = "PENDING";
        public int? OrderIndex { get; private set; }

        // Parameterless constructor for EF
        protected Phase() { }

        public Phase(int projectId, string name, int? orderIndex = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            ProjectId = projectId;
            Name = name;
            OrderIndex = orderIndex;
            Status = "PENDING";
        }

        public void UpdateDetails(string name, DateTime? startDate, DateTime? endDate, int? orderIndex)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            OrderIndex = orderIndex;
        }

        public void SetStatus(string status)
        {
            Status = status;
        }

        public void SetDates(DateTime? startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}

