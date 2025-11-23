using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Data
{
    public class ProjectRepositoryTests
    {
        private static async Task<(SqliteConnection, AppDbContext, ProjectRepository)> CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();

            var repo = new ProjectRepository(context);
            return (connection, context, repo);
        }

        [Fact]
        public async Task AddAndGetProject_Succeeds()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                // Primero crear un usuario válido para satisfacer la FK
                var owner = new User("projectOwner", "hash123");
                await context.Users.AddAsync(owner);
                await context.SaveChangesAsync();

                var project = new Project("PROY-100", "RepoProj", DateTime.UtcNow, owner.Id, "Desc");
                await repo.AddAsync(project);
                await repo.SaveChangesAsync();

                var fromDb = await repo.GetByIdentifierAsync("PROY-100");
                Assert.NotNull(fromDb);
                Assert.Equal("PROY-100", fromDb.Identifier);
            }
        }

        [Fact]
        public async Task DuplicateIdentifier_ThrowsOnSave()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                // Primero crear un usuario válido para satisfacer la FK
                var owner = new User("projectOwner2", "hash456");
                await context.Users.AddAsync(owner);
                await context.SaveChangesAsync();

                var p1 = new Project("PROY-dup", "P1", DateTime.UtcNow, owner.Id);
                await repo.AddAsync(p1);
                await repo.SaveChangesAsync();

                var p2 = new Project("PROY-dup", "P2", DateTime.UtcNow, owner.Id);
                await repo.AddAsync(p2);
                await Assert.ThrowsAsync<DbUpdateException>(async () => await repo.SaveChangesAsync());
            }
        }
    }
}

