namespace OpenUpMan.Domain
{
    public class PhaseItemUser
    {
        public Guid PhaseItemId { get; private set; }
        public PhaseItem? PhaseItem { get; private set; }
        public Guid UserId { get; private set; }
        public User? User { get; private set; }
        public string Role { get; private set; }  // PARTICIPANT, RESPONSIBLE, etc.

        // Parameterless constructor for EF
        protected PhaseItemUser()
        {
            Role = string.Empty;
        }

        public PhaseItemUser(Guid phaseItemId, Guid userId, string role = "PARTICIPANT")
        {
            if (phaseItemId == Guid.Empty)
            {
                throw new ArgumentException("PhaseItemId cannot be an empty GUID.", nameof(phaseItemId));
            }

            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be an empty GUID.", nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("Role cannot be null or empty.", nameof(role));
            }

            PhaseItemId = phaseItemId;
            UserId = userId;
            Role = role;
        }

        public void SetRole(string newRole)
        {
            if (string.IsNullOrWhiteSpace(newRole))
            {
                throw new ArgumentException("Role cannot be null or empty.", nameof(newRole));
            }

            Role = newRole;
        }
    }
}

