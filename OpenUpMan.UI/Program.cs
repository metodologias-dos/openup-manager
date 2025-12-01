using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using OpenUpMan.Data;
using OpenUpMan.Data.Repositories;
using OpenUpMan.Services;
using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.UI;

sealed class Program
{
    
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Set Mexican culture for DD/MM/YYYY date format
        var culture = new CultureInfo("es-MX");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

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
        catch (Exception)
        {
            // don't fail startup for logging
        }

        // Configure logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddDbContext<AppDbContext>(options => 
            options.UseSqlite(connString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

        // Repositories and services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectService, ProjectService>();
        
        services.AddScoped<IProjectUserRepository, ProjectUserRepository>();
        services.AddScoped<IProjectUserService, ProjectUserService>();
        
        services.AddScoped<IProjectPhaseRepository, ProjectPhaseRepository>();
        services.AddScoped<IProjectPhaseService, ProjectPhaseService>();
        
        // Role repositories and services
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoleService, RoleService>();
        
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IPermissionService, PermissionService>();
        
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();

        // ViewModels
        services.AddTransient<OpenUpMan.UI.ViewModels.MainWindowViewModel>();

        ServiceProvider = services.BuildServiceProvider();

        // Ensure database and tables are created on first run (for embedded SQLite)
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<OpenUpMan.Data.Migrations.DatabaseMigrator>>();
            
            // Use DatabaseMigrator to handle schema updates automatically
            // autoRemoveObsoleteColumns=true will automatically remove columns that no longer exist in entities
            var migrator = new OpenUpMan.Data.Migrations.DatabaseMigrator(ctx, logger, autoRemoveObsoleteColumns: true);
            migrator.MigrateAsync().Wait();

            // Seed predefined roles
            SeedRoles(ctx, logger);

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

    private static void SeedRoles(AppDbContext context, ILogger logger)
    {
        try
        {
            // Verificar si ya existen roles
            if (context.Roles.Any())
            {
                logger.LogInformation("Roles already exist in database. Skipping seed.");
                return;
            }

            logger.LogInformation("Seeding predefined roles...");

            var roles = new[]
            {
                new Role(RoleIds.Admin, "Administrador", "Acceso completo al sistema"),
                new Role(RoleIds.ProductOwner, "Product Owner", "Gestión del producto"),
                new Role(RoleIds.ScrumMaster, "Scrum Master", "Facilitador del equipo"),
                new Role(RoleIds.Desarrollador, "Desarrollador", "Desarrollo de software"),
                new Role(RoleIds.Tester, "Tester", "Pruebas y QA"),
                new Role(RoleIds.Revisor, "Revisor", "Revisión de documentos y código"),
                new Role(RoleIds.Autor, "Autor", "Creación de contenido"),
                new Role(RoleIds.Viewer, "Viewer", "Solo lectura")
            };

            context.Roles.AddRange(roles);
            context.SaveChanges();

            logger.LogInformation("Successfully seeded {Count} predefined roles.", roles.Length);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding predefined roles");
            throw;
        }
    }
}
