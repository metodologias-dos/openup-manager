// Added domain entity for User with basic fields and constructors
namespace OpenUpMan.Domain
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }

        // Parameterless constructor for EF
        protected User() { }

        public User(string username, string passwordHash)
        {
            Id = Guid.NewGuid();
            Username = username;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));
            }
            PasswordHash = passwordHash;
        }
    }
}

