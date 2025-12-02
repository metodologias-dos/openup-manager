using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Data;

public class UserRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;

    public UserRepositoryTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldAddUser()
    {
        // Arrange
        await using var context = new AppDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();
        var repository = new UserRepository(context);
        var user = new User("testuser", "passwordhash");

        // Act
        await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        // Assert
        var retrievedUser = await context.Users.SingleAsync();
        Assert.Equal("testuser", retrievedUser.Username);
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnCorrectUser()
    {
        // Arrange
        await using var context = new AppDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();
        var repository = new UserRepository(context);
        var user = new User("testuser", "passwordhash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var retrievedUser = await repository.GetByUsernameAsync("testuser");

        // Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal("testuser", retrievedUser.Username);
    }
    
    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        await using var context = new AppDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();
        var repository = new UserRepository(context);

        // Act
        var retrievedUser = await repository.GetByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(retrievedUser);
    }
}
