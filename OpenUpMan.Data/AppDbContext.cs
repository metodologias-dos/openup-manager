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
        public DbSet<Phase> Phases { get; set; } = null!;
        public DbSet<Iteration> Iterations { get; set; } = null!;
        public DbSet<Microincrement> Microincrements { get; set; } = null!;
        public DbSet<Artifact> Artifacts { get; set; } = null!;
        public DbSet<ArtifactVersion> ArtifactVersions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Users
            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("users");
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).HasColumnName("id").ValueGeneratedOnAdd();
                b.Property(u => u.Username).HasColumnName("username").IsRequired();
                b.Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired();
                b.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
                b.HasIndex(u => u.Username).IsUnique();
            });

            // Permissions
            modelBuilder.Entity<Permission>(b =>
            {
                b.ToTable("permissions");
                b.HasKey(p => p.Id);
                b.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
                b.Property(p => p.Name).HasColumnName("name").IsRequired();
                b.Property(p => p.Description).HasColumnName("description");
                b.HasIndex(p => p.Name).IsUnique();
            });

            // Roles
            modelBuilder.Entity<Role>(b =>
            {
                b.ToTable("roles");
                b.HasKey(r => r.Id);
                b.Property(r => r.Id).HasColumnName("id").ValueGeneratedOnAdd();
                b.Property(r => r.Name).HasColumnName("name").IsRequired();
                b.Property(r => r.Description).HasColumnName("description");
                b.HasIndex(r => r.Name).IsUnique();
            });

            // RolePermissions
            modelBuilder.Entity<RolePermission>(b =>
            {
                b.ToTable("role_permissions");
                b.HasKey(rp => rp.Id);
                b.Property(rp => rp.Id).HasColumnName("id").ValueGeneratedOnAdd();
                b.Property(rp => rp.RoleId).HasColumnName("role_id").IsRequired();
                b.Property(rp => rp.PermissionId).HasColumnName("permission_id").IsRequired();

                b.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne<Permission>()
                    .WithMany()
                    .HasForeignKey(rp => rp.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Projects
            modelBuilder.Entity<Project>(b =>
            {
                b.ToTable("projects");
                b.HasKey(p => p.Id);
                b.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
                b.Property(p => p.Name).HasColumnName("name").IsRequired();
                b.Property(p => p.Code).HasColumnName("code");
                b.Property(p => p.Description).HasColumnName("description");
                b.Property(p => p.StartDate).HasColumnName("start_date");
                b.Property(p => p.Status).HasColumnName("status").IsRequired();
                b.Property(p => p.CreatedBy).HasColumnName("created_by");
                b.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
                b.Property(p => p.UpdatedAt).HasColumnName("updated_at");
                b.Property(p => p.DeletedAt).HasColumnName("deleted_at");

                b.HasIndex(p => p.Code).IsUnique();

                b.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(p => p.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ProjectUsers
            modelBuilder.Entity<ProjectUser>(b =>
            {
                b.ToTable("project_users");
                b.HasKey(pu => pu.Id);
                b.Property(pu => pu.Id).HasColumnName("id").ValueGeneratedOnAdd();
                b.Property(pu => pu.ProjectId).HasColumnName("project_id").IsRequired();
                b.Property(pu => pu.UserId).HasColumnName("user_id").IsRequired();
                b.Property(pu => pu.RoleId).HasColumnName("role_id").IsRequired();
                b.Property(pu => pu.AddedAt).HasColumnName("added_at").IsRequired();

                b.HasOne<Project>()
                    .WithMany()
                    .HasForeignKey(pu => pu.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(pu => pu.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey(pu => pu.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Phases
            modelBuilder.Entity<Phase>(b =>
            {
                b.ToTable("phases");
                b.HasKey(p => p.Id);
                b.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
                b.Property(p => p.ProjectId).HasColumnName("project_id").IsRequired();
                b.Property(p => p.Name).HasColumnName("name").IsRequired();
                b.Property(p => p.StartDate).HasColumnName("start_date");
                b.Property(p => p.EndDate).HasColumnName("end_date");
                b.Property(p => p.Status).HasColumnName("status").IsRequired();
                b.Property(p => p.OrderIndex).HasColumnName("order_index");

                b.HasOne<Project>()
                    .WithMany()
                    .HasForeignKey(p => p.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Iterations
            modelBuilder.Entity<Iteration>(b =>
            {
                b.ToTable("iterations");
                b.HasKey(i => i.Id);
                b.Property(i => i.Id).HasColumnName("id").ValueGeneratedOnAdd();
                b.Property(i => i.PhaseId).HasColumnName("phase_id").IsRequired();
                b.Property(i => i.Name).HasColumnName("name");
                b.Property(i => i.Goal).HasColumnName("goal");
                b.Property(i => i.StartDate).HasColumnName("start_date");
                b.Property(i => i.EndDate).HasColumnName("end_date");
                b.Property(i => i.CompletionPercentage).HasColumnName("completion_percentage").IsRequired();

                b.HasOne<Phase>()
                    .WithMany()
                    .HasForeignKey(i => i.PhaseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Microincrements
            modelBuilder.Entity<Microincrement>(b =>
            {
                b.ToTable("microincrements");
                b.HasKey(m => m.Id);
                b.Property(m => m.Id).HasColumnName("id").ValueGeneratedOnAdd();
                b.Property(m => m.IterationId).HasColumnName("iteration_id").IsRequired();
                b.Property(m => m.Title).HasColumnName("title").IsRequired();
                b.Property(m => m.Description).HasColumnName("description");
                b.Property(m => m.Date).HasColumnName("date").IsRequired();
                b.Property(m => m.AuthorId).HasColumnName("author_id");
                b.Property(m => m.Type).HasColumnName("type").IsRequired();
                b.Property(m => m.ArtifactId).HasColumnName("artifact_id");
                b.Property(m => m.EvidenceUrl).HasColumnName("evidence_url");

                b.HasOne<Iteration>()
                    .WithMany()
                    .HasForeignKey(m => m.IterationId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(m => m.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne<Artifact>()
                    .WithMany()
                    .HasForeignKey(m => m.ArtifactId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Artifacts
            modelBuilder.Entity<Artifact>(b =>
            {
                b.ToTable("artifacts");
                b.HasKey(a => a.Id);
                b.Property(a => a.Id).HasColumnName("id").ValueGeneratedOnAdd();
                b.Property(a => a.ProjectId).HasColumnName("project_id").IsRequired();
                b.Property(a => a.PhaseId).HasColumnName("phase_id").IsRequired();
                b.Property(a => a.Name).HasColumnName("name").IsRequired();
                b.Property(a => a.ArtifactType).HasColumnName("artifact_type");
                b.Property(a => a.Mandatory)
                    .HasColumnName("mandatory")
                    .HasConversion<int>()
                    .IsRequired();
                b.Property(a => a.Description).HasColumnName("description");
                b.Property(a => a.CurrentState).HasColumnName("current_state").IsRequired();

                b.HasOne<Project>()
                    .WithMany()
                    .HasForeignKey(a => a.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne<Phase>()
                    .WithMany()
                    .HasForeignKey(a => a.PhaseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ArtifactVersions
            modelBuilder.Entity<ArtifactVersion>(b =>
            {
                b.ToTable("artifact_versions");
                b.HasKey(av => av.Id);
                b.Property(av => av.Id).HasColumnName("id").ValueGeneratedOnAdd();
                b.Property(av => av.ArtifactId).HasColumnName("artifact_id").IsRequired();
                b.Property(av => av.VersionNumber).HasColumnName("version_number").IsRequired();
                b.Property(av => av.CreatedBy).HasColumnName("created_by");
                b.Property(av => av.CreatedAt).HasColumnName("created_at").IsRequired();
                b.Property(av => av.Notes).HasColumnName("notes");
                b.Property(av => av.FileBlob).HasColumnName("file_blob");
                b.Property(av => av.FileMime).HasColumnName("file_mime");
                b.Property(av => av.BuildInfo).HasColumnName("build_info");

                b.HasIndex(av => new { av.ArtifactId, av.VersionNumber }).IsUnique();

                b.HasOne<Artifact>()
                    .WithMany()
                    .HasForeignKey(av => av.ArtifactId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(av => av.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
