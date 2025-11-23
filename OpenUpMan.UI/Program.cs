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
