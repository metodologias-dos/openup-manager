using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OpenUpMan.Data;

/// <summary>
/// Design-time factory for EF Core migrations.
/// This allows 'dotnet ef migrations' commands to work without needing a running application.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Use a temporary SQLite database for design-time operations
        // The actual database path will be configured at runtime in Program.cs
        optionsBuilder.UseSqlite("Data Source=designtime.db");
        
        return new AppDbContext(optionsBuilder.Options);
    }
}

