using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Data
{
    public class UserRepositoryTests
    {
        private static async Task<(SqliteConnection, AppDbContext, UserRepository)> CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();

            var repo = new UserRepository(context);
            return (connection, context, repo);
        }

        [Fact]
        public async Task AddAndGetUser_Succeeds()
        {
            var tuple = await CreateInMemoryDatabase();
            using var connection = tuple.Item1;
            using var context = tuple.Item2;
            var repo = tuple.Item3;

            var user = new User("repoUser", "hash123");
            await repo.AddAsync(user);
            await repo.SaveChangesAsync();

            var fromDb = await repo.GetByUsernameAsync("repoUser");
            Assert.NotNull(fromDb);
            Assert.Equal("repoUser", fromDb!.Username);
        }

        [Fact]
        public async Task DuplicateUsername_ThrowsOnSave()
        {
            var tuple = await CreateInMemoryDatabase();
            using var connection = tuple.Item1;
            using var context = tuple.Item2;
            var repo = tuple.Item3;

            var u1 = new User("dupUser", "h1");
            await repo.AddAsync(u1);
            await repo.SaveChangesAsync();

            var u2 = new User("dupUser", "h2");
            await repo.AddAsync(u2);
            // Save should raise a DbUpdateException because of unique index
            await Assert.ThrowsAsync<Microsoft.EntityFrameworkCore.DbUpdateException>(async () => await repo.SaveChangesAsync());
        }
    }
}
