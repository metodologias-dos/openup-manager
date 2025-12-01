using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;
using System.Text;

namespace OpenUpMan.Data.Migrations;

/// <summary>
/// Fully automated database schema migrator that inspects EF Core entities and synchronizes the database schema.
/// Similar to Spring Data JPA's automatic schema management (ddl-auto=update).
/// 
/// Features:
/// - Automatically discovers all entities from DbContext
/// - Creates missing tables based on entity definitions
/// - Adds missing columns to existing tables
/// - Removes obsolete columns (optional, disabled by default for safety)
/// - Manages indexes and unique constraints
/// - Handles all EF Core data types automatically
/// </summary>
public class DatabaseMigrator
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseMigrator>? _logger;
    private readonly bool _autoRemoveObsoleteColumns;

    /// <summary>
    /// Creates a new DatabaseMigrator instance
    /// </summary>
    /// <param name="context">The EF Core DbContext to migrate</param>
    /// <param name="logger">Optional logger for migration events</param>
    /// <param name="autoRemoveObsoleteColumns">If true, automatically removes columns that no longer exist in entities (requires table recreation in SQLite)</param>
    public DatabaseMigrator(
        AppDbContext context, 
        ILogger<DatabaseMigrator>? logger = null,
        bool autoRemoveObsoleteColumns = false)
    {
        _context = context;
        _logger = logger;
        _autoRemoveObsoleteColumns = autoRemoveObsoleteColumns;
    }

    /// <summary>
    /// Ensures the database schema is up to date with the current EF Core model.
    /// Automatically creates, updates, or removes tables and columns based on entity definitions.
    /// </summary>
    public async Task MigrateAsync()
    {
        var conn = _context.Database.GetDbConnection();
        var wasOpen = conn.State == ConnectionState.Open;
        
        try
        {
            if (!wasOpen)
                await conn.OpenAsync();

            _logger?.LogInformation("Starting automatic database schema migration...");

            // Check if database exists (if any table from model exists)
            var model = _context.Model;
            var hasAnyTable = await HasAnyModelTableAsync(conn, model);
            
            if (!hasAnyTable)
            {
                _logger?.LogInformation("Database does not exist. Creating new database with current schema...");
                // Don't use EnsureCreatedAsync() as it may close the connection (destroying in-memory databases)
                // Instead, manually create all tables
                await CreateMissingTablesAsync(conn, model);
                _logger?.LogInformation("Database created successfully.");
                return;
            }

            // Synchronize schema with model
            await SynchronizeSchemaAsync(conn, model);
            
            _logger?.LogInformation("Database schema migration completed successfully.");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during database migration. Migration failed and will be retried on next startup.");
            throw new InvalidOperationException("Database migration failed. See inner exception for details.", ex);
        }
        finally
        {
            if (!wasOpen && conn.State == ConnectionState.Open)
                await conn.CloseAsync();
        }
    }

    /// <summary>
    /// Synchronizes the database schema with the EF Core model.
    /// </summary>
    private async Task SynchronizeSchemaAsync(DbConnection conn, IModel model)
    {
        // Step 1: Create missing tables
        await CreateMissingTablesAsync(conn, model);
        
        // Step 2: Add missing columns to existing tables
        await AddMissingColumnsAsync(conn, model);
        
        // Step 3: Update indexes
        await UpdateIndexesAsync(conn, model);
        
        // Step 4: Remove obsolete columns (if enabled)
        if (_autoRemoveObsoleteColumns)
        {
            await RemoveObsoleteColumnsAsync(conn, model);
        }
        else
        {
            await LogObsoleteColumnsAsync(conn, model);
        }
    }

    #region Table Management

    private async Task<bool> HasAnyModelTableAsync(DbConnection conn, IModel model)
    {
        var entityTypes = model.GetEntityTypes();
        foreach (var entityType in entityTypes)
        {
            var tableName = entityType.GetTableName();
            if (tableName != null && await TableExistsAsync(conn, tableName))
            {
                return true;
            }
        }
        return false;
    }

    private async Task CreateMissingTablesAsync(DbConnection conn, IModel model)
    {
        var entityTypes = model.GetEntityTypes();

        foreach (var entityType in entityTypes)
        {
            var tableName = entityType.GetTableName();
            if (tableName == null) continue;

            if (!await TableExistsAsync(conn, tableName))
            {
                _logger?.LogInformation("Creating missing table: {TableName}", tableName);
                await CreateTableAsync(conn, entityType, tableName);
                _logger?.LogInformation("Table {TableName} created successfully.", tableName);
            }
        }
    }

    private static async Task<bool> TableExistsAsync(DbConnection conn, string tableName)
    {
        using var cmd = conn.CreateCommand();
        // Use COLLATE NOCASE for case-insensitive comparison to handle legacy databases
        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName COLLATE NOCASE";
        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);
        var result = await cmd.ExecuteScalarAsync();
        return result != null;
    }

    private async Task CreateTableAsync(DbConnection conn, IEntityType entityType, string tableName)
    {
        // For normal table creation, physical name = logical name
        var createSql = GenerateCreateTableSql(entityType, tableName, logicalTableName: tableName);
        
        using var cmd = conn.CreateCommand();
        cmd.CommandText = createSql;
        await cmd.ExecuteNonQueryAsync();

        // Create indexes after table creation
        await CreateIndexesForTableAsync(conn, entityType, tableName);
    }

    private static string GenerateCreateTableSql(IEntityType entityType, string physicalTableName, string? logicalTableName = null)
    {
        // Use logical table name (from entity config) for column mapping, physical name for SQL statement
        var tableNameForMapping = logicalTableName ?? physicalTableName;
        
        var sb = new StringBuilder();
        sb.AppendLine($"CREATE TABLE {physicalTableName} (");

        var storeObjectId = StoreObjectIdentifier.Table(tableNameForMapping);
        var properties = entityType.GetProperties().ToList();
        var primaryKey = entityType.FindPrimaryKey();
        var primaryKeyProperties = primaryKey?.Properties.Select(p => p.Name).ToHashSet() ?? new HashSet<string>();

        var columnDefinitions = new List<string>();

        foreach (var property in properties)
        {
            var columnName = property.GetColumnName(storeObjectId);
            if (columnName == null) continue; // Skip properties that don't map to this table
            
            var columnType = GetSqliteType(property);
            var isNullable = property.IsNullable;
            var isPrimaryKey = primaryKeyProperties.Contains(property.Name);

            // Escape column name with double quotes to handle SQL keywords
            var columnDef = new StringBuilder($"    \"{columnName}\" {columnType}");

            if (!isNullable || isPrimaryKey)
            {
                columnDef.Append(" NOT NULL");
            }

            if (isPrimaryKey && primaryKeyProperties.Count == 1)
            {
                columnDef.Append(" PRIMARY KEY");
            }

            columnDefinitions.Add(columnDef.ToString());
        }

        // Ensure at least one column is defined
        if (columnDefinitions.Count == 0)
        {
            throw new InvalidOperationException($"Cannot create table '{physicalTableName}' with no columns. Entity type must have at least one property.");
        }

        sb.AppendLine(string.Join(",\n", columnDefinitions));

        // Add composite primary key if needed
        if (primaryKey != null && primaryKeyProperties.Count > 1)
        {
            var pkColumns = string.Join(", ", primaryKey.Properties.Select(p => p.GetColumnName(storeObjectId)).Where(c => c != null).Select(c => $"\"{c}\""));
            sb.AppendLine($",    PRIMARY KEY ({pkColumns})");
        }

        sb.Append(")");

        return sb.ToString();
    }

    #endregion

    #region Column Management

    private async Task AddMissingColumnsAsync(DbConnection conn, IModel model)
    {
        var entityTypes = model.GetEntityTypes();

        foreach (var entityType in entityTypes)
        {
            var tableName = entityType.GetTableName();
            if (tableName == null || !await TableExistsAsync(conn, tableName)) continue;

            var storeObjectId = StoreObjectIdentifier.Table(tableName);
            var existingColumns = await GetTableColumnsInfoAsync(conn, tableName);
            var existingColumnNames = existingColumns.Select(c => c.Name).ToHashSet();

            var properties = entityType.GetProperties();

            foreach (var property in properties)
            {
                var columnName = property.GetColumnName(storeObjectId);
                if (columnName == null) continue; // Skip properties that don't map to this table
                
                if (!existingColumnNames.Contains(columnName))
                {
                    _logger?.LogInformation("Adding missing column: {TableName}.{ColumnName}", tableName, columnName);
                    await AddColumnAsync(conn, tableName, property, storeObjectId);
                    _logger?.LogInformation("Column {TableName}.{ColumnName} added successfully.", tableName, columnName);
                }
            }
        }
    }

    private static async Task AddColumnAsync(DbConnection conn, string tableName, IProperty property, StoreObjectIdentifier storeObjectId)
    {
        var columnName = property.GetColumnName(storeObjectId);
        if (columnName == null) return; // Skip if no column name
        
        var columnType = GetSqliteType(property);
        var isNullable = property.IsNullable;

        using var cmd = conn.CreateCommand();
        // Escape column name with double quotes to handle SQL keywords
        var sb = new StringBuilder($"ALTER TABLE \"{tableName}\" ADD COLUMN \"{columnName}\" {columnType}");

        if (!isNullable)
        {
            // For NOT NULL columns, we need to provide a default value
            var defaultValue = GetDefaultValueForType(property);
            sb.Append($" NOT NULL DEFAULT {defaultValue}");
        }

        cmd.CommandText = sb.ToString();
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task RemoveObsoleteColumnsAsync(DbConnection conn, IModel model)
    {
        var entityTypes = model.GetEntityTypes();

        foreach (var entityType in entityTypes)
        {
            var tableName = entityType.GetTableName();
            if (tableName == null || !await TableExistsAsync(conn, tableName)) continue;

            var storeObjectId = StoreObjectIdentifier.Table(tableName);
            var existingColumns = await GetTableColumnsInfoAsync(conn, tableName);
            var expectedColumnNames = entityType.GetProperties()
                .Select(p => p.GetColumnName(storeObjectId))
                .Where(c => c != null)
                .ToHashSet();

            var obsoleteColumns = existingColumns
                .Where(c => !expectedColumnNames.Contains(c.Name))
                .ToList();

            if (obsoleteColumns.Any())
            {
                _logger?.LogInformation("Removing obsolete columns from {TableName}: {Columns}", 
                    tableName, 
                    string.Join(", ", obsoleteColumns.Select(c => c.Name)));
                
                await RecreateTableWithoutColumnsAsync(conn, entityType, tableName);
            }
        }
    }

    private async Task LogObsoleteColumnsAsync(DbConnection conn, IModel model)
    {
        var entityTypes = model.GetEntityTypes();

        foreach (var entityType in entityTypes)
        {
            var tableName = entityType.GetTableName();
            if (tableName == null || !await TableExistsAsync(conn, tableName)) continue;

            var storeObjectId = StoreObjectIdentifier.Table(tableName);
            var existingColumns = await GetTableColumnsInfoAsync(conn, tableName);
            var expectedColumnNames = entityType.GetProperties()
                .Select(p => p.GetColumnName(storeObjectId))
                .Where(c => c != null)
                .ToHashSet();

            var obsoleteColumns = existingColumns
                .Where(c => !expectedColumnNames.Contains(c.Name))
                .ToList();

            foreach (var column in obsoleteColumns)
            {
                _logger?.LogWarning("Obsolete column detected: {TableName}.{ColumnName}. " +
                    "Enable autoRemoveObsoleteColumns=true to automatically remove it.", 
                    tableName, column.Name);
            }
        }
    }

    private async Task RecreateTableWithoutColumnsAsync(DbConnection conn, IEntityType entityType, string tableName)
    {
        var storeObjectId = StoreObjectIdentifier.Table(tableName);
        
        // Check if entity has any properties (columns) to keep
        var columnsToKeep = entityType.GetProperties()
            .Select(p => p.GetColumnName(storeObjectId))
            .Where(c => c != null)
            .ToList();
        
        // If no columns to keep, the entity model has been removed/emptied - just drop the table
        if (!columnsToKeep.Any())
        {
            _logger?.LogWarning("Entity {EntityName} has no columns. Dropping table {TableName}.", 
                entityType.ClrType.Name, tableName);
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"DROP TABLE IF EXISTS \"{tableName}\"";
                await cmd.ExecuteNonQueryAsync();
            }
            return;
        }
        
        // SQLite doesn't support DROP COLUMN, so we need to recreate the table
        var tempTableName = $"{tableName}_temp_{Guid.NewGuid():N}";
        
        // Create new table with correct schema
        // Use original table name for column mapping, but temp name for physical table
        var createSql = GenerateCreateTableSql(entityType, tempTableName, logicalTableName: tableName);
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = createSql;
            await cmd.ExecuteNonQueryAsync();
        }

        // Copy data from old table to new table
        var columnList = string.Join(", ", columnsToKeep.Select(c => $"\"{c}\""));
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = $"INSERT INTO \"{tempTableName}\" ({columnList}) SELECT {columnList} FROM \"{tableName}\"";
            await cmd.ExecuteNonQueryAsync();
        }

        // Drop old table
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = $"DROP TABLE \"{tableName}\"";
            await cmd.ExecuteNonQueryAsync();
        }

        // Rename new table to original name
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = $"ALTER TABLE \"{tempTableName}\" RENAME TO \"{tableName}\"";
            await cmd.ExecuteNonQueryAsync();
        }

        // Recreate indexes
        await CreateIndexesForTableAsync(conn, entityType, tableName);
    }

    #endregion

    #region Index Management

    private async Task UpdateIndexesAsync(DbConnection conn, IModel model)
    {
        var entityTypes = model.GetEntityTypes();

        foreach (var entityType in entityTypes)
        {
            var tableName = entityType.GetTableName();
            if (tableName == null || !await TableExistsAsync(conn, tableName)) continue;

            await CreateIndexesForTableAsync(conn, entityType, tableName);
        }
    }

    private async Task CreateIndexesForTableAsync(DbConnection conn, IEntityType entityType, string tableName)
    {
        var storeObjectId = StoreObjectIdentifier.Table(tableName);
        var indexes = entityType.GetIndexes();

        foreach (var index in indexes)
        {
            var indexName = index.GetDatabaseName();
            if (indexName == null) continue; // Skip indexes without names
            
            var isUnique = index.IsUnique;
            var columns = string.Join(", ", index.Properties.Select(p => p.GetColumnName(storeObjectId)).Where(c => c != null).Select(c => $"\"{c}\""));

            // Check if index already exists
            if (await IndexExistsAsync(conn, indexName))
            {
                continue;
            }

            _logger?.LogInformation("Creating index: {IndexName} on {TableName}", indexName, tableName);

            using var cmd = conn.CreateCommand();
            var uniqueKeyword = isUnique ? "UNIQUE " : "";
            cmd.CommandText = $"CREATE {uniqueKeyword}INDEX IF NOT EXISTS \"{indexName}\" ON \"{tableName}\" ({columns})";
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private static async Task<bool> IndexExistsAsync(DbConnection conn, string indexName)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='index' AND name=@indexName";
        var param = cmd.CreateParameter();
        param.ParameterName = "@indexName";
        param.Value = indexName;
        cmd.Parameters.Add(param);
        var result = await cmd.ExecuteScalarAsync();
        return result != null;
    }

    #endregion

    #region Utility Methods

    private static async Task<List<DbColumnInfo>> GetTableColumnsInfoAsync(DbConnection conn, string tableName)
    {
        var columns = new List<DbColumnInfo>();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"PRAGMA table_info({tableName})";
        
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            columns.Add(new DbColumnInfo(
                reader.GetInt32(0),              // cid
                reader.GetString(1),             // name
                reader.GetString(2),             // type
                reader.GetInt32(3) != 0,         // notnull
                await reader.IsDBNullAsync(4) ? null : reader.GetValue(4), // dflt_value
                reader.GetInt32(5) != 0          // pk
            ));
        }

        return columns;
    }

    private static string GetSqliteType(IProperty property)
    {
        var clrType = property.ClrType;
        var underlyingType = Nullable.GetUnderlyingType(clrType) ?? clrType;

        if (underlyingType == typeof(int) || underlyingType == typeof(long) || 
            underlyingType == typeof(short) || underlyingType == typeof(byte) ||
            underlyingType == typeof(bool))
        {
            return "INTEGER";
        }
        else if (underlyingType == typeof(double) || underlyingType == typeof(float) || underlyingType == typeof(decimal))
        {
            return "REAL";
        }
        else if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateTimeOffset))
        {
            return "TEXT"; // SQLite stores dates as TEXT, REAL, or INTEGER
        }
        else if (underlyingType == typeof(Guid))
        {
            return "TEXT";
        }
        else if (underlyingType == typeof(byte[]))
        {
            return "BLOB";
        }
        else
        {
            return "TEXT"; // Default to TEXT for strings and unknown types
        }
    }

    private static string GetDefaultValueForType(IProperty property)
    {
        var clrType = property.ClrType;
        var underlyingType = Nullable.GetUnderlyingType(clrType) ?? clrType;

        if (underlyingType == typeof(int) || underlyingType == typeof(long) || 
            underlyingType == typeof(short) || underlyingType == typeof(byte) ||
            underlyingType == typeof(bool))
        {
            return "0";
        }
        else if (underlyingType == typeof(double) || underlyingType == typeof(float) || underlyingType == typeof(decimal))
        {
            return "0.0";
        }
        else if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateTimeOffset))
        {
            return "'1900-01-01T00:00:00'";
        }
        else if (underlyingType == typeof(Guid))
        {
            return "'00000000-0000-0000-0000-000000000000'";
        }
        else
        {
            return "''"; // Empty string for TEXT types
        }
    }

    #endregion

    #region Data Models

    private sealed record DbColumnInfo(
        int Id,
        string Name,
        string Type,
        bool NotNull,
        object? DefaultValue,
        bool IsPrimaryKey
    );

    #endregion
}

