using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Data
{
    public class DocumentRepositoryTests
    {
        private static async Task<(SqliteConnection, AppDbContext, DocumentRepository)> CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();

            var repo = new DocumentRepository(context);
            return (connection, context, repo);
        }

        [Fact]
        public async Task AddAndGetDocument_Succeeds()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("creator", "hash");
                var project = new Project("PROJ-001", "Test", DateTime.UtcNow, user.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                var item = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iter 1", user.Id);
                context.Users.Add(user);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                context.PhaseItems.Add(item);
                await context.SaveChangesAsync();

                var document = new Document(item.Id, "Documento de Requisitos", user.Id, "Descripción");
                await repo.AddAsync(document);
                await repo.SaveChangesAsync();

                var fromDb = await repo.GetByIdAsync(document.Id);
                Assert.NotNull(fromDb);
                Assert.Equal("Documento de Requisitos", fromDb.Title);
                Assert.Equal(0, fromDb.LastVersionNumber);
            }
        }

        [Fact]
        public async Task GetByPhaseItemId_ReturnsDocuments()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("creator", "hash");
                var project = new Project("PROJ-002", "Test", DateTime.UtcNow, user.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                var item = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iter 1", user.Id);
                context.Users.Add(user);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                context.PhaseItems.Add(item);
                await context.SaveChangesAsync();

                await repo.AddAsync(new Document(item.Id, "Doc 1", user.Id));
                await repo.AddAsync(new Document(item.Id, "Doc 2", user.Id));
                await repo.SaveChangesAsync();

                var documents = await repo.GetByPhaseItemIdAsync(item.Id);
                Assert.Equal(2, documents.Count());
            }
        }

        [Fact]
        public async Task GetByCreatorId_ReturnsDocuments()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("creator", "hash");
                var project = new Project("PROJ-003", "Test", DateTime.UtcNow, user.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                var item1 = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iter 1", user.Id);
                var item2 = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 2, "Iter 2", user.Id);
                context.Users.Add(user);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                context.PhaseItems.AddRange(item1, item2);
                await context.SaveChangesAsync();

                await repo.AddAsync(new Document(item1.Id, "Doc 1", user.Id));
                await repo.AddAsync(new Document(item2.Id, "Doc 2", user.Id));
                await repo.SaveChangesAsync();

                var documents = await repo.GetByCreatorIdAsync(user.Id);
                Assert.Equal(2, documents.Count());
            }
        }

        [Fact]
        public async Task UpdateDocument_Succeeds()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("creator", "hash");
                var project = new Project("PROJ-004", "Test", DateTime.UtcNow, user.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                var item = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iter 1", user.Id);
                context.Users.Add(user);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                context.PhaseItems.Add(item);
                await context.SaveChangesAsync();

                var document = new Document(item.Id, "Old Title", user.Id);
                await repo.AddAsync(document);
                await repo.SaveChangesAsync();

                document.UpdateDetails("New Title", "New Description");
                await repo.UpdateAsync(document);
                await repo.SaveChangesAsync();

                var fromDb = await repo.GetByIdAsync(document.Id);
                Assert.Equal("New Title", fromDb!.Title);
                Assert.Equal("New Description", fromDb.Description);
            }
        }

        [Fact]
        public async Task IncrementVersion_UpdatesLastVersionNumber()
        {
            var (connection, context, repo) = await CreateInMemoryDatabase();
            using (connection)
            using (context)
            {
                var user = new User("creator", "hash");
                var project = new Project("PROJ-005", "Test", DateTime.UtcNow, user.Id);
                var phase = new ProjectPhase(project.Id, PhaseCode.INCEPTION, "Inicio", 1);
                var item = new PhaseItem(phase.Id, PhaseItemType.ITERATION, 1, "Iter 1", user.Id);
                context.Users.Add(user);
                context.Projects.Add(project);
                context.ProjectPhases.Add(phase);
                context.PhaseItems.Add(item);
                await context.SaveChangesAsync();

                var document = new Document(item.Id, "Doc", user.Id);
                await repo.AddAsync(document);
                await repo.SaveChangesAsync();

                document.IncrementVersion();
                await repo.UpdateAsync(document);
                await repo.SaveChangesAsync();

                var fromDb = await repo.GetByIdAsync(document.Id);
                Assert.Equal(1, fromDb!.LastVersionNumber);
            }
        }
    }
}

