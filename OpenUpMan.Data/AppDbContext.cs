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
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<ProjectUser> ProjectUsers { get; set; } = null!;
        public DbSet<ProjectPhase> ProjectPhases { get; set; } = null!;
        public DbSet<PhaseItem> PhaseItems { get; set; } = null!;
        public DbSet<PhaseItemUser> PhaseItemUsers { get; set; } = null!;
        public DbSet<Document> Documents { get; set; } = null!;
        public DbSet<DocumentVersion> DocumentVersions { get; set; } = null!;
        public DbSet<Artefact> Artefacts { get; set; } = null!;
        public DbSet<PhaseArtefact> PhaseArtefacts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("users");
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).HasColumnName("Id");
                b.Property(u => u.Username).HasColumnName("Username").IsRequired();
                b.Property(u => u.PasswordHash).HasColumnName("PasswordHash").IsRequired();
                b.Property(u => u.CreatedAt).HasColumnName("CreatedAt").IsRequired();
                b.Property(u => u.PasswordChangedAt).HasColumnName("PasswordChangedAt");
                b.HasIndex(u => u.Username).IsUnique();
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.ToTable("rol");
                b.HasKey(r => r.Id);
                b.Property(r => r.Id).HasColumnName("Id");
                b.Property(r => r.Name).HasColumnName("Name").IsRequired();
                b.Property(r => r.Description).HasColumnName("Description");
            });

            modelBuilder.Entity<Permission>(b =>
            {
                b.ToTable("permission");
                b.HasKey(p => p.Id);
                b.Property(p => p.Id).HasColumnName("Id");
                b.Property(p => p.Name).HasColumnName("Name").IsRequired();
                b.Property(p => p.Description).HasColumnName("Description");
            });

            modelBuilder.Entity<RolePermission>(b =>
            {
                b.ToTable("rol_permission");
                b.HasKey(rp => new { rp.RoleId, rp.PermissionId });

                b.Property(rp => rp.RoleId).HasColumnName("RoleId");
                b.Property(rp => rp.PermissionId).HasColumnName("PermissionId");

                b.HasOne(rp => rp.Role)
                    .WithMany()
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(rp => rp.Permission)
                    .WithMany()
                    .HasForeignKey(rp => rp.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Project>(b =>
            {
                b.ToTable("projects");
                b.HasKey(p => p.Id);
                b.Property(p => p.Id).HasColumnName("Id");
                b.Property(p => p.Identifier).HasColumnName("Identifier").IsRequired();
                b.Property(p => p.Name).HasColumnName("Name").IsRequired();
                b.Property(p => p.Description).HasColumnName("Description");
                b.Property(p => p.StartDate).HasColumnName("StartDate").IsRequired();
                b.Property(p => p.OwnerId).HasColumnName("OwnerId").IsRequired();
                b.Property(p => p.State)
                    .HasColumnName("State")
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
                b.ToTable("project_users");
                b.HasKey(pu => new { pu.ProjectId, pu.UserId });

                b.Property(pu => pu.ProjectId).HasColumnName("ProjectId");
                b.Property(pu => pu.UserId).HasColumnName("UserId");
                b.Property(pu => pu.RoleId).HasColumnName("RoleId").IsRequired();

                b.HasOne(pu => pu.Project)
                    .WithMany()
                    .HasForeignKey(pu => pu.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(pu => pu.User)
                    .WithMany()
                    .HasForeignKey(pu => pu.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(pu => pu.Role)
                    .WithMany()
                    .HasForeignKey(pu => pu.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProjectPhase>(b =>
            {
                b.ToTable("project_phases");
                b.HasKey(pp => pp.Id);

                b.Property(pp => pp.Id).HasColumnName("Id");
                b.Property(pp => pp.ProjectId).HasColumnName("ProjectId").IsRequired();
                b.Property(pp => pp.Code)
                    .HasColumnName("Code")
                    .HasConversion<string>()
                    .IsRequired();

                b.Property(pp => pp.State)
                    .HasColumnName("State")
                    .HasConversion<string>()
                    .IsRequired();

                b.Property(pp => pp.Name).HasColumnName("Name").IsRequired();
                b.Property(pp => pp.Order).HasColumnName("Order").IsRequired();

                b.HasOne(pp => pp.Project)
                    .WithMany()
                    .HasForeignKey(pp => pp.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PhaseItem>(b =>
            {
                b.ToTable("phase_items");
                b.HasKey(pi => pi.Id);

                b.Property(pi => pi.Id).HasColumnName("Id");
                b.Property(pi => pi.ProjectPhaseId).HasColumnName("ProjectPhaseId").IsRequired();
                b.Property(pi => pi.Type)
                    .HasColumnName("Type")
                    .HasConversion<string>()
                    .IsRequired();

                b.Property(pi => pi.State)
                    .HasColumnName("State")
                    .HasConversion<string>()
                    .IsRequired();

                b.Property(pi => pi.Name).HasColumnName("Name").IsRequired();
                b.Property(pi => pi.Number).HasColumnName("Number").IsRequired();
                b.Property(pi => pi.ParentIterationId).HasColumnName("ParentIterationId");
                b.Property(pi => pi.Description).HasColumnName("Description");
                b.Property(pi => pi.StartDate).HasColumnName("StartDate");
                b.Property(pi => pi.EndDate).HasColumnName("EndDate");
                b.Property(pi => pi.CreatedBy).HasColumnName("CreatedBy").IsRequired();
                b.Property(pi => pi.CreatedAt).HasColumnName("CreatedAt").IsRequired();

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
                b.ToTable("phase_item_users");
                b.HasKey(piu => new { piu.PhaseItemId, piu.UserId });

                b.Property(piu => piu.PhaseItemId).HasColumnName("PhaseItemId");
                b.Property(piu => piu.UserId).HasColumnName("UserId");
                b.Property(piu => piu.Role).HasColumnName("Role").IsRequired();

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
                b.ToTable("documents");
                b.HasKey(d => d.Id);

                b.Property(d => d.Id).HasColumnName("Id");
                b.Property(d => d.PhaseItemId).HasColumnName("PhaseItemId").IsRequired();
                b.Property(d => d.Title).HasColumnName("Title").IsRequired();
                b.Property(d => d.Description).HasColumnName("Description");
                b.Property(d => d.CreatedBy).HasColumnName("CreatedBy").IsRequired();
                b.Property(d => d.CreatedAt).HasColumnName("CreatedAt").IsRequired();
                b.Property(d => d.LastVersionNumber).HasColumnName("LastVersionNumber").IsRequired();

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
                b.ToTable("document_versions");
                b.HasKey(dv => dv.Id);

                b.Property(dv => dv.Id).HasColumnName("Id");
                b.Property(dv => dv.DocumentId).HasColumnName("DocumentId").IsRequired();
                b.Property(dv => dv.VersionNumber).HasColumnName("VersionNumber").IsRequired();
                b.Property(dv => dv.CreatedAt).HasColumnName("CreatedAt").IsRequired();
                b.Property(dv => dv.CreatedBy).HasColumnName("CreatedBy").IsRequired();
                b.Property(dv => dv.FilePath).HasColumnName("FilePath").IsRequired();
                b.Property(dv => dv.Observations).HasColumnName("Observations");

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

            modelBuilder.Entity<Artefact>(b =>
            {
                b.ToTable("artefacts");
                b.HasKey(a => a.Id);

                b.Property(a => a.Id).HasColumnName("Id");
                b.Property(a => a.Name).HasColumnName("Name").IsRequired();
                b.Property(a => a.Description).HasColumnName("Description");
            });

            modelBuilder.Entity<PhaseArtefact>(b =>
            {
                b.ToTable("phase_artefacts");
                b.HasKey(pa => new { pa.PhaseId, pa.ArtefactId });

                b.Property(pa => pa.PhaseId).HasColumnName("PhaseId");
                b.Property(pa => pa.ArtefactId).HasColumnName("ArtefactId");
                b.Property(pa => pa.DocumentId).HasColumnName("DocumentId");
                b.Property(pa => pa.Registrado).HasColumnName("Registrado").IsRequired();

                b.HasOne(pa => pa.Phase)
                    .WithMany()
                    .HasForeignKey(pa => pa.PhaseId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(pa => pa.Artefact)
                    .WithMany()
                    .HasForeignKey(pa => pa.ArtefactId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(pa => pa.Document)
                    .WithMany()
                    .HasForeignKey(pa => pa.DocumentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
