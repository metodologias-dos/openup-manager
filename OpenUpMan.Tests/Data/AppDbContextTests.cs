using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Tests.Data;

public class AppDbContextTests
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;

    public AppDbContextTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
    }

    [Fact]
    public async Task CanAddAndRetrieveUser()
    {
        await using var context = new AppDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();

        var user = new User("testuser", "passwordhash");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var retrievedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
        Assert.NotNull(retrievedUser);
        Assert.Equal("testuser", retrievedUser.Username);
    }

    [Fact]
    public async Task CanAddAndRetrieveProject()
    {
        await using var context = new AppDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();

        var project = new Project("Test Project", null, "TP01");
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var retrievedProject = await context.Projects.FirstOrDefaultAsync(p => p.Code == "TP01");
        Assert.NotNull(retrievedProject);
        Assert.Equal("Test Project", retrievedProject.Name);
    }

    [Fact]
    public async Task CanAddAndRetrieveRoleAndPermission()
    {
        await using var context = new AppDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();

        var role = new Role(999, "TestRole");
        var permission = new Permission("test:permission");
        context.Roles.Add(role);
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        var rolePermission = new RolePermission(role.Id, permission.Id);
        context.RolePermissions.Add(rolePermission);
        await context.SaveChangesAsync();

        var retrievedRolePermission = await context.RolePermissions.Include(rp => rp.Permission)
            .FirstOrDefaultAsync(rp => rp.RoleId == role.Id);

        Assert.NotNull(retrievedRolePermission);
        Assert.NotNull(retrievedRolePermission.Permission);
        Assert.Equal("test:permission", retrievedRolePermission.Permission.Name);
    }
}
