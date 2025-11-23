using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenUpMan.Data;
using OpenUpMan.Domain;
using Xunit;

namespace OpenUpMan.Tests.Data
{
    public class UserRepositoryPersistenceTests
    {
        [Fact]
        public async Task FileSqlite_PersistsAcrossContexts()
        {
            var dbFile = Path.Combine(Path.GetTempPath(), "openup_persist_test.db");
            if (File.Exists(dbFile)) File.Delete(dbFile);

            var connString = $"Data Source={dbFile}";

            var options1 = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connString)
                .Options;

            using (var ctx = new AppDbContext(options1))
            {
                await ctx.Database.EnsureCreatedAsync();
                var repo = new UserRepository(ctx);
                var user = new User("persistUser", "hash123");
                await repo.AddAsync(user);
                await repo.SaveChangesAsync();
            }

            var options2 = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connString)
                .Options;

            using (var ctx2 = new AppDbContext(options2))
            {
                var repo2 = new UserRepository(ctx2);
                var fromDb = await repo2.GetByUsernameAsync("persistUser");
                Assert.NotNull(fromDb);
                Assert.Equal("persistUser", fromDb!.Username);
            }

            if (File.Exists(dbFile))
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        File.Delete(dbFile);
                        break;
                    }
                    catch (IOException)
                    {
                        await Task.Delay(100);
                    }
                }
            }
        }
    }
}
