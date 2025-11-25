using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Data
{
    public class PhaseItemRepositoryTests
    {
        private static async Task<(SqliteConnection, AppDbContext, PhaseItemRepository)> CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();

            var repo = new PhaseItemRepository(context);
            return (connection, context, repo);
        }

        [Fact]
        public async Task AddAndGetPhaseItem_Succeeds()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("creator", "hash");
                var project = new Project("PROJ-001", "Test", DateTime.UtcNow, user.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                context.Users.Add(user);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                await context.SaveChangesAsync();

                var item = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iteración 1", user.Id);
                await repo.AddAsync(item);
                await repo.SaveChangesAsync();

                var fromDb = await repo.GetByIdAsync(item.Id);
                Assert.NotNull(fromDb);
                Assert.Equal(PhaseItemType.ITERATION, fromDb.Type);
                Assert.Equal("Iteración 1", fromDb.Name);
            }
        }

        [Fact]
        public async Task GetIterationsByPhaseId_ReturnsOnlyIterations()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("creator", "hash");
                var project = new Project("PROJ-002", "Test", DateTime.UtcNow, user.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                context.Users.Add(user);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                await context.SaveChangesAsync();

                var iteration = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iter 1", user.Id);
                await repo.AddAsync(iteration);
                await repo.SaveChangesAsync();

                var micro = new PhaseItem(phase.Id, PhaseItemType.MICROINCREMENT, 1, "Micro 1", user.Id, iteration.Id);
                await repo.AddAsync(micro);
                await repo.SaveChangesAsync();

                var iterations = await repo.GetIterationsByPhaseIdAsync(phase.Id);
                Assert.Single(iterations);
                Assert.All(iterations, i => Assert.Equal(PhaseItemType.ITERATION, i.Type));
            }
        }

        [Fact]
        public async Task GetMicroincrementsByIterationId_ReturnsMicroincrements()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("creator", "hash");
                var project = new Project("PROJ-003", "Test", DateTime.UtcNow, user.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                context.Users.Add(user);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                await context.SaveChangesAsync();

                var iteration = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iter 1", user.Id);
                await repo.AddAsync(iteration);
                await repo.SaveChangesAsync();

                await repo.AddAsync(new PhaseItem(phase.Id, PhaseItemType.MICROINCREMENT, 1, "M1", user.Id, iteration.Id));
                await repo.AddAsync(new PhaseItem(phase.Id, PhaseItemType.MICROINCREMENT, 2, "M2", user.Id, iteration.Id));
                await repo.SaveChangesAsync();

                var micros = await repo.GetMicroincrementsByIterationIdAsync(iteration.Id);
                Assert.Equal(2, micros.Count());
                Assert.All(micros, m => Assert.Equal(PhaseItemType.MICROINCREMENT, m.Type));
            }
        }

        [Fact]
        public async Task GetByPhaseId_ReturnsAllItems()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("creator", "hash");
                var project = new Project("PROJ-004", "Test", DateTime.UtcNow, user.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                context.Users.Add(user);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                await context.SaveChangesAsync();

                var iteration = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iter 1", user.Id);
                await repo.AddAsync(iteration);
                await repo.AddAsync(new PhaseItem(phase.Id, PhaseItemType.MICROINCREMENT, 1, "M1", user.Id, iteration.Id));
                await repo.SaveChangesAsync();

                var items = await repo.GetByPhaseIdAsync(phase.Id);
                Assert.Equal(2, items.Count());
            }
        }

        [Fact]
        public async Task UpdatePhaseItem_Succeeds()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("creator", "hash");
                var project = new Project("PROJ-005", "Test", DateTime.UtcNow, user.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                context.Users.Add(user);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                await context.SaveChangesAsync();

                var item = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iter 1", user.Id);
                await repo.AddAsync(item);
                await repo.SaveChangesAsync();

                item.SetState(PhaseItemState.ACTIVE);
                await repo.UpdateAsync(item);
                await repo.SaveChangesAsync();

                var fromDb = await repo.GetByIdAsync(item.Id);
                Assert.Equal(PhaseItemState.ACTIVE, fromDb!.State);
            }
        }
    }
}

