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
        public string? Objective { get; private set; }
        public string? Scope { get; private set; }
        public string? Observations { get; private set; }

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

        public void UpdateDetails(string name, DateTime? startDate, DateTime? endDate, int? orderIndex, 
            string? objective = null, string? scope = null, string? observations = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            OrderIndex = orderIndex;
            Objective = objective;
            Scope = scope;
            Observations = observations;
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

        public void SetObjective(string? objective)
        {
            Objective = objective;
        }

        public void SetScope(string? scope)
        {
            Scope = scope;
        }

        public void SetObservations(string? observations)
        {
            Observations = observations;
        }
    }
}
