namespace OpenUpMan.Domain
{
    public class Iteration
    {
        public int Id { get; private set; }
        public int PhaseId { get; private set; }
        public string? Name { get; private set; }
        public string? Goal { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public int CompletionPercentage { get; private set; }

        // Parameterless constructor for EF
        protected Iteration() { }

        public Iteration(int phaseId, string? name = null, string? goal = null)
        {
            PhaseId = phaseId;
            Name = name;
            Goal = goal;
            CompletionPercentage = 0;
        }

        public void UpdateDetails(string? name, string? goal, DateTime? startDate, DateTime? endDate)
        {
            Name = name;
            Goal = goal;
            StartDate = startDate;
            EndDate = endDate;
        }

        public void SetCompletionPercentage(int percentage)
        {
            if (percentage < 0 || percentage > 100)
            {
                throw new ArgumentException("Completion percentage must be between 0 and 100.", nameof(percentage));
            }

            CompletionPercentage = percentage;
        }
    }
}

