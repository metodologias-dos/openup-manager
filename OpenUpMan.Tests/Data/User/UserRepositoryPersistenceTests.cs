using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Data
{
    public class UserRepositoryPersistenceTests
    {
        [Fact]
        public async Task FileSqlite_PersistsAcrossContexts()
        {
            var dbFile = Path.Combine(Path.GetTempPath(), "openup_persist_test.db");
            
            // Clean up any existing file
            CleanupDatabaseFile(dbFile);

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
                Assert.Equal("persistUser", fromDb.Username);
            }

            // Clean up after test
            CleanupDatabaseFile(dbFile);
        }

        private static void CleanupDatabaseFile(string dbFile)
        {
            if (!File.Exists(dbFile))
                return;

            // Clear all SQLite connection pools to release file handles
            SqliteConnection.ClearAllPools();
            
            // Force garbage collection to ensure connections are disposed
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Retry deletion with longer delay
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    File.Delete(dbFile);
                    break;
                }
                catch (IOException)
                {
                    if (i == 19)
                    {
                        // Last attempt failed, but don't fail the test
                        // Just log or ignore - the file will be cleaned up eventually
                        return;
                    }
                    Thread.Sleep(50);
                }
            }
        }
    }
}
