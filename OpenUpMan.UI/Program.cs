using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
using OpenUpMan.Data;
using OpenUpMan.Services;
using Microsoft.EntityFrameworkCore;

namespace OpenUpMan.UI;

sealed class Program
{
    
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        // Configure DI
        var services = new ServiceCollection();

        // Register DbContext with a local file in application data
        var dbPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "openup.db");
        // Use shared cache to allow multiple connections and enable WAL below for better concurrency
        var connString = $"Data Source={dbPath};Cache=Shared;Pooling=True;";

        // --- Print connection URIs for external tools (JDBC / Rider DB explorer) ---
        try
        {
            // JDBC prefers forward slashes; create a couple of common URI formats
            var dbPathForward = dbPath.Replace('\\', '/');
            var jdbcSimple = $"jdbc:sqlite:{dbPathForward}"; // e.g. jdbc:sqlite:C:/Users/you/.../openup.db
            var jdbcFileUri = $"jdbc:sqlite:file:{dbPathForward}?cache=shared&mode=rwc&journal_mode=WAL"; // with options

            Console.WriteLine("--- SQLite connection info ---");
            Console.WriteLine($".NET / EF Core connection string: {connString}");
            Console.WriteLine($"JDBC simple URI (try this in Rider): {jdbcSimple}");
            Console.WriteLine($"JDBC file URI with params: {jdbcFileUri}");
            Console.WriteLine("(DB file path: " + dbPath + ")");
            Console.WriteLine("-------------------------------");
        }
        catch (Exception ex)
        {
            // Don't fail startup for logging, but log the error to console for visibility.
            Console.Error.WriteLine($"Warning: failed to print SQLite connection info: {ex.Message}");
        }

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connString));

        // Repositories and services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        // ViewModels
        services.AddTransient<OpenUpMan.UI.ViewModels.MainWindowViewModel>();

        ServiceProvider = services.BuildServiceProvider();

        // Ensure database and tables are created on first run (for embedded SQLite)
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // For simple embedded scenarios: create DB schema if it doesn't exist.
            ctx.Database.EnsureCreated();

            // Enable Write-Ahead Logging (WAL) for better concurrency (readers won't block writers)
            var conn = ctx.Database.GetDbConnection();
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "PRAGMA journal_mode=WAL;";
                cmd.ExecuteNonQuery();
            }
            finally
            {
                // keep connection lifecycle to EF; close if we explicitly opened it here
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }
        catch (Exception ex)
        {
            // If creation fails, write to console — the app will still attempt to start.
            Console.WriteLine($"Warning: could not ensure database created: {ex.Message}");
        }

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
