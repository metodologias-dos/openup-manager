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
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<ProjectUser> ProjectUsers { get; set; } = null!;
        public DbSet<ProjectPhase> ProjectPhases { get; set; } = null!;
        public DbSet<PhaseItem> PhaseItems { get; set; } = null!;
        public DbSet<PhaseItemUser> PhaseItemUsers { get; set; } = null!;
        public DbSet<Document> Documents { get; set; } = null!;
        public DbSet<DocumentVersion> DocumentVersions { get; set; } = null!;

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

            modelBuilder.Entity<Project>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Identifier).IsRequired();
                b.Property(p => p.Name).IsRequired();
                b.Property(p => p.StartDate).IsRequired();
                b.Property(p => p.State)
                    .HasConversion<string>()
                    .IsRequired();
                
                // Configurar DateTime para que se trate como UTC
                b.Property(p => p.CreatedAt)
                    .HasConversion(
                        v => v,
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
                
                b.Property(p => p.UpdatedAt)
                    .HasConversion(
                        v => v,
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);

                b.HasIndex(p => p.Identifier).IsUnique();

                b.HasOne(p => p.Owner)
                    .WithMany()
                    .HasForeignKey(p => p.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProjectUser>(b =>
            {
                b.HasKey(pu => new { pu.ProjectId, pu.UserId });

                b.Property(pu => pu.Permissions)
                    .HasConversion<string>()
                    .IsRequired();

                b.Property(pu => pu.Role)
                    .HasConversion<string>()
                    .IsRequired();

                b.HasOne(pu => pu.Project)
                    .WithMany()
                    .HasForeignKey(pu => pu.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(pu => pu.User)
                    .WithMany()
                    .HasForeignKey(pu => pu.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProjectPhase>(b =>
            {
                b.HasKey(pp => pp.Id);

                b.Property(pp => pp.Code)
                    .HasConversion<string>()
                    .IsRequired();

                b.Property(pp => pp.State)
                    .HasConversion<string>()
                    .IsRequired();

                b.Property(pp => pp.Name).IsRequired();
                b.Property(pp => pp.Order)
                    .HasColumnName("DisplayOrder")
                    .IsRequired();

                b.HasOne(pp => pp.Project)
                    .WithMany()
                    .HasForeignKey(pp => pp.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PhaseItem>(b =>
            {
                b.HasKey(pi => pi.Id);

                b.Property(pi => pi.Type)
                    .HasColumnName("ItemType")
                    .HasConversion<string>()
                    .IsRequired();

                b.Property(pi => pi.State)
                    .HasConversion<string>()
                    .IsRequired();

                b.Property(pi => pi.Name).IsRequired();
                b.Property(pi => pi.Number).IsRequired();

                b.HasOne(pi => pi.ProjectPhase)
                    .WithMany()
                    .HasForeignKey(pi => pi.ProjectPhaseId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(pi => pi.ParentIteration)
                    .WithMany()
                    .HasForeignKey(pi => pi.ParentIterationId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(pi => pi.Creator)
                    .WithMany()
                    .HasForeignKey(pi => pi.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PhaseItemUser>(b =>
            {
                b.HasKey(piu => new { piu.PhaseItemId, piu.UserId });

                b.Property(piu => piu.Role).IsRequired();

                b.HasOne(piu => piu.PhaseItem)
                    .WithMany()
                    .HasForeignKey(piu => piu.PhaseItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(piu => piu.User)
                    .WithMany()
                    .HasForeignKey(piu => piu.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Document>(b =>
            {
                b.HasKey(d => d.Id);

                b.Property(d => d.Title).IsRequired();
                b.Property(d => d.LastVersionNumber).IsRequired();

                b.HasOne(d => d.PhaseItem)
                    .WithMany()
                    .HasForeignKey(d => d.PhaseItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(d => d.Creator)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DocumentVersion>(b =>
            {
                b.HasKey(dv => dv.Id);

                b.Property(dv => dv.VersionNumber).IsRequired();
                b.Property(dv => dv.FilePath).IsRequired();

                b.HasIndex(dv => new { dv.DocumentId, dv.VersionNumber }).IsUnique();

                b.HasOne(dv => dv.Document)
                    .WithMany()
                    .HasForeignKey(dv => dv.DocumentId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(dv => dv.Creator)
                    .WithMany()
                    .HasForeignKey(dv => dv.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
