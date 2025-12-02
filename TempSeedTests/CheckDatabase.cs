using System;
using Microsoft.Data.Sqlite;

// Quick script to check the SQLite database
var connectionString = "Data Source=C:\\Users\\eriar\\RiderProjects\\openup-manager\\OpenUpMan.UI\\designtime.db";

using var connection = new SqliteConnection(connectionString);
connection.Open();

Console.WriteLine("=== CHECKING DATABASE ===\n");

// Check permissions count
using (var cmd = new SqliteCommand("SELECT COUNT(*) FROM permissions", connection))
{
    var count = Convert.ToInt32(cmd.ExecuteScalar());
    Console.WriteLine($"Permissions count: {count} (Expected: 36)");
}

// Check roles count
using (var cmd = new SqliteCommand("SELECT COUNT(*) FROM roles", connection))
{
    var count = Convert.ToInt32(cmd.ExecuteScalar());
    Console.WriteLine($"Roles count: {count} (Expected: 8)");
}

// Check role_permissions count
using (var cmd = new SqliteCommand("SELECT COUNT(*) FROM role_permissions", connection))
{
    var count = Convert.ToInt32(cmd.ExecuteScalar());
    Console.WriteLine($"Role-Permissions relationships: {count}");
}

Console.WriteLine("\n=== PERMISSIONS SAMPLE ===");
using (var cmd = new SqliteCommand("SELECT id, name FROM permissions ORDER BY id LIMIT 10", connection))
using (var reader = cmd.ExecuteReader())
{
    while (reader.Read())
    {
        Console.WriteLine($"ID: {reader.GetInt32(0)}, Name: {reader.GetString(1)}");
    }
}

Console.WriteLine("\n=== ROLES ===");
using (var cmd = new SqliteCommand("SELECT id, name FROM roles ORDER BY id", connection))
using (var reader = cmd.ExecuteReader())
{
    while (reader.Read())
    {
        Console.WriteLine($"ID: {reader.GetInt32(0)}, Name: {reader.GetString(1)}");
    }
}

Console.WriteLine("\n=== ROLE PERMISSIONS FOR 'Autor' (RoleId=7) ===");
using (var cmd = new SqliteCommand(@"
    SELECT COUNT(*) 
    FROM role_permissions 
    WHERE role_id = 7", connection))
{
    var count = Convert.ToInt32(cmd.ExecuteScalar());
    Console.WriteLine($"Autor has {count} permissions (Expected: ~35)");
}

Console.WriteLine("\n=== ROLE PERMISSIONS FOR 'Revisor' (RoleId=6) ===");
using (var cmd = new SqliteCommand(@"
    SELECT COUNT(*) 
    FROM role_permissions 
    WHERE role_id = 6", connection))
{
    var count = Convert.ToInt32(cmd.ExecuteScalar());
    Console.WriteLine($"Revisor has {count} permissions (Expected: ~9)");
}

Console.WriteLine("\n=== DONE ===");

