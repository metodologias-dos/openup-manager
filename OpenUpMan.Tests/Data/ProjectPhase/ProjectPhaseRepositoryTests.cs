using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Data
{
    public class ProjectPhaseRepositoryTests
    {
        private static async Task<(SqliteConnection, AppDbContext, ProjectPhaseRepository)> CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();

            var repo = new ProjectPhaseRepository(context);
            return (connection, context, repo);
        }

        [Fact]
        public async Task AddAndGetProjectPhase_Succeeds()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("owner", "hash");
                var project = new Project("PROJ-001", "Test", DateTime.UtcNow, user.Id);
                context.Users.Add(user);
                context.Projects.Add(project);
                await context.SaveChangesAsync();

                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                await repo.AddAsync(phase);
                await repo.SaveChangesAsync();

                var fromDb = await repo.GetByIdAsync(phase.Id);
                Assert.NotNull(fromDb);
                Assert.Equal(PhaseCode.INCEPTION, fromDb.Code);
                Assert.Equal("Inicio", fromDb.Name);
                Assert.Equal(1, fromDb.Order);
            }
        }

        [Fact]
        public async Task GetByProjectId_ReturnsOrderedPhases()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("owner", "hash");
                var project = new Project("PROJ-002", "Test", DateTime.UtcNow, user.Id);
                context.Users.Add(user);
                context.Projects.Add(project);
                await context.SaveChangesAsync();

                await repo.AddAsync(new ProjectPhase(project.Id, PhaseCode.CONSTRUCTION, "Construcción", 3));
                await repo.AddAsync(new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1));
                await repo.AddAsync(new ProjectPhase(project.Id, PhaseCode.ELABORATION, "Elaboración", 2));
                await repo.SaveChangesAsync();

                var phases = await repo.GetByProjectIdAsync(project.Id);
                var phaseList = phases.ToList();

                Assert.Equal(3, phaseList.Count);
                Assert.Equal(1, phaseList[0].Order);
                Assert.Equal(2, phaseList[1].Order);
                Assert.Equal(3, phaseList[2].Order);
            }
        }

        [Fact]
        public async Task GetByProjectIdAndCode_ReturnsCorrectPhase()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("owner", "hash");
                var project = new Project("PROJ-003", "Test", DateTime.UtcNow, user.Id);
                context.Users.Add(user);
                context.Projects.Add(project);
                await context.SaveChangesAsync();

                await repo.AddAsync(new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1));
                await repo.AddAsync(new ProjectPhase(project.Id, PhaseCode.ELABORATION, "Elaboración", 2));
                await repo.SaveChangesAsync();

                var phase = await repo.GetByProjectIdAndCodeAsync(project.Id, PhaseCode.ELABORATION);
                Assert.NotNull(phase);
                Assert.Equal(PhaseCode.ELABORATION, phase.Code);
            }
        }

        [Fact]
        public async Task UpdatePhase_Succeeds()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("owner", "hash");
                var project = new Project("PROJ-004", "Test", DateTime.UtcNow, user.Id);
                context.Users.Add(user);
                context.Projects.Add(project);
                await context.SaveChangesAsync();

                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                await repo.AddAsync(phase);
                await repo.SaveChangesAsync();

                phase.SetState(PhaseState.IN_PROGRESS);
                await repo.UpdateAsync(phase);
                await repo.SaveChangesAsync();

                var fromDb = await repo.GetByIdAsync(phase.Id);
                Assert.Equal(PhaseState.IN_PROGRESS, fromDb!.State);
            }
        }
    }
}

