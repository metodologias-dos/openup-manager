using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Data;

public class ProjectRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;

    public ProjectRepositoryTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldAddProject()
    {
        // Arrange
        await using var context = new AppDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();
        var repository = new ProjectRepository(context);
        var project = new Project("Test Project", null, "TP01");

        // Act
        await repository.AddAsync(project);
        await repository.SaveChangesAsync();

        // Assert
        var retrievedProject = await context.Projects.SingleAsync();
        Assert.Equal("Test Project", retrievedProject.Name);
    }

    [Fact]
    public async Task GetByCodeAsync_ShouldReturnCorrectProject()
    {
        // Arrange
        await using var context = new AppDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();
        var repository = new ProjectRepository(context);
        var project = new Project("Test Project", null, "TP01");
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        // Act
        var retrievedProject = await repository.GetByCodeAsync("TP01");

        // Assert
        Assert.NotNull(retrievedProject);
        Assert.Equal("TP01", retrievedProject.Code);
    }

    [Fact]
    public async Task GetLastProjectCodeAsync_ShouldReturnLastCode()
    {
        // Arrange
        await using var context = new AppDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();
        var repository = new ProjectRepository(context);
        context.Projects.Add(new Project("Project 1", null, "P001"));
        context.Projects.Add(new Project("Project 2", null, "P002"));
        await context.SaveChangesAsync();

        // Act
        var lastCode = await repository.GetLastProjectCodeAsync();

        // Assert
        Assert.Equal("P002", lastCode);
    }
}
