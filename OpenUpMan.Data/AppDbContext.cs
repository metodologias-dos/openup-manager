using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Username).IsRequired();
                b.Property(u => u.PasswordHash).IsRequired();
                b.HasIndex(u => u.Username).IsUnique();
            });
        }
    }
}
