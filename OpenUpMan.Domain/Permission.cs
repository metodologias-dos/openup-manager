namespace OpenUpMan.Domain
{
    public class Permission
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }

        // Parameterless constructor for EF
        protected Permission() { }

        public Permission(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Name = name;
            Description = description;
        }

        public void UpdateDetails(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Name = name;
            Description = description;
        }
    }
}

