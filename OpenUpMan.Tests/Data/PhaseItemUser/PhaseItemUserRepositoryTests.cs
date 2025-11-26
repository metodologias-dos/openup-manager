using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Data
{
    public class PhaseItemUserRepositoryTests
    {
        private static async Task<(SqliteConnection, AppDbContext, PhaseItemUserRepository)> CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();

            var repo = new PhaseItemUserRepository(context);
            return (connection, context, repo);
        }

        [Fact]
        public async Task AddAndGetPhaseItemUser_Succeeds()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("testuser", "hash");
                var project = new Project("PROJ-001", "Test", DateTime.UtcNow, user.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                var item = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iter 1", user.Id);
                context.Users.Add(user);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                context.PhaseItems.Add(item);
                await context.SaveChangesAsync();

                var phaseItemUser = new PhaseItemUser(item.Id, user.Id, "PARTICIPANT");
                await repo.AddAsync(phaseItemUser);
                await repo.SaveChangesAsync();

                var fromDb = await repo.GetByIdAsync(item.Id, user.Id);
                Assert.NotNull(fromDb);
                Assert.Equal(item.Id, fromDb.PhaseItemId);
                Assert.Equal(user.Id, fromDb.UserId);
                Assert.Equal("PARTICIPANT", fromDb.Role);
            }
        }

        [Fact]
        public async Task GetByPhaseItemId_ReturnsAllUsers()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user1 = new User("user1", "hash1");
                var user2 = new User("user2", "hash2");
                var project = new Project("PROJ-002", "Test", DateTime.UtcNow, user1.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                var item = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iter 1", user1.Id);
                context.Users.AddRange(user1, user2);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                context.PhaseItems.Add(item);
                await context.SaveChangesAsync();

                await repo.AddAsync(new PhaseItemUser(item.Id, user1.Id, "RESPONSIBLE"));
                await repo.AddAsync(new PhaseItemUser(item.Id, user2.Id, "PARTICIPANT"));
                await repo.SaveChangesAsync();

                var users = await repo.GetByPhaseItemIdAsync(item.Id);
                Assert.Equal(2, users.Count());
            }
        }

        [Fact]
        public async Task GetByUserId_ReturnsAllItems()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("testuser", "hash");
                var project = new Project("PROJ-003", "Test", DateTime.UtcNow, user.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                var item1 = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iter 1", user.Id);
                var item2 = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 2, "Iter 2", user.Id);
                context.Users.Add(user);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                context.PhaseItems.AddRange(item1, item2);
                await context.SaveChangesAsync();

                await repo.AddAsync(new PhaseItemUser(item1.Id, user.Id));
                await repo.AddAsync(new PhaseItemUser(item2.Id, user.Id));
                await repo.SaveChangesAsync();

                var items = await repo.GetByUserIdAsync(user.Id);
                Assert.Equal(2, items.Count());
            }
        }

        [Fact]
        public async Task RemovePhaseItemUser_Succeeds()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("testuser", "hash");
                var project = new Project("PROJ-004", "Test", DateTime.UtcNow, user.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                var item = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iter 1", user.Id);
                context.Users.Add(user);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                context.PhaseItems.Add(item);
                await context.SaveChangesAsync();

                var phaseItemUser = new PhaseItemUser(item.Id, user.Id);
                await repo.AddAsync(phaseItemUser);
                await repo.SaveChangesAsync();

                await repo.RemoveAsync(phaseItemUser);
                await repo.SaveChangesAsync();

                var fromDb = await repo.GetByIdAsync(item.Id, user.Id);
                Assert.Null(fromDb);
            }
        }
    }
}

