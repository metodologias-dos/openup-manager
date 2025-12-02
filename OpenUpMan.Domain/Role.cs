namespace OpenUpMan.Domain
{
    public class Role
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }

        // Parameterless constructor for EF
        protected Role() { }

        public Role(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Name = name;
            Description = description;
        }

        // Constructor for seed data with predefined Id
        public Role(int id, string name, string? description = null)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id must be positive.", nameof(id));
            }
            
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            Id = id;
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

