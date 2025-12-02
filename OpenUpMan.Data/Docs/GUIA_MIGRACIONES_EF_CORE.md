# Guía de Migraciones con Entity Framework Core

## 📋 Tabla de Contenidos
1. [Introducción](#introducción)
2. [Comandos Básicos](#comandos-básicos)
3. [Cómo Crear Migraciones](#cómo-crear-migraciones)
4. [Ejemplos Prácticos](#ejemplos-prácticos)
5. [Comportamiento de EF Core](#comportamiento-de-ef-core)
6. [Preguntas Frecuentes](#preguntas-frecuentes)

---

## Introducción

Este proyecto utiliza **Entity Framework Core 9.0.0** para gestionar el esquema de la base de datos SQLite.

### ¿Qué son las Migraciones?

Las migraciones son archivos que contienen instrucciones para actualizar el esquema de la base de datos cuando modificas tus entidades del dominio.

### ¿Por qué usar Migraciones de EF Core?

| Ventaja | Descripción |
|---------|-------------|
| 🔄 **Control de versiones** | Todas las modificaciones del esquema quedan registradas en archivos |
| 👥 **Trabajo en equipo** | Varios desarrolladores pueden trabajar sin conflictos |
| ↩️ **Reversibilidad** | Puedes volver a versiones anteriores del esquema |
| 📜 **Historial** | Documentación automática de cambios en la BD |
| 🛡️ **Seguridad** | Revisas los cambios antes de aplicarlos |
| 📦 **Deployment** | Puedes generar scripts SQL para producción |

---

## Comandos Básicos

### Pre-requisitos

Asegúrate de tener instaladas las herramientas de EF Core:

```bash
# Instalar herramientas globalmente (solo una vez)
dotnet tool install --global dotnet-ef --version 9.0.0

# O actualizar si ya están instaladas
dotnet tool update --global dotnet-ef --version 9.0.0

# Verificar instalación
dotnet ef --version
```

### Comandos Esenciales

```bash
# IMPORTANTE: Siempre ejecuta desde la raíz de la solución
cd C:\Users\eriar\RiderProjects\openup-manager

# 1️⃣ CREAR una nueva migración
dotnet ef migrations add NombreDeLaMigracion --project OpenUpMan.Data --startup-project OpenUpMan.UI

# 2️⃣ LISTAR todas las migraciones
dotnet ef migrations list --project OpenUpMan.Data --startup-project OpenUpMan.UI

# 3️⃣ ELIMINAR la última migración (si NO se ha aplicado)
dotnet ef migrations remove --project OpenUpMan.Data --startup-project OpenUpMan.UI

# 4️⃣ APLICAR migraciones manualmente (normalmente es automático al ejecutar la app)
dotnet ef database update --project OpenUpMan.Data --startup-project OpenUpMan.UI

# 5️⃣ GENERAR script SQL
dotnet ef migrations script --output migracion.sql --project OpenUpMan.Data --startup-project OpenUpMan.UI

# 6️⃣ REVERTIR a una migración anterior
dotnet ef database update NombreMigracionAnterior --project OpenUpMan.Data --startup-project OpenUpMan.UI
```

---

## Cómo Crear Migraciones

### Proceso Paso a Paso

#### 1. Modifica tu Entidad de Dominio

Por ejemplo, agregar un campo `Email` a la entidad `User`:

```csharp
// OpenUpMan.Domain/User.cs
namespace OpenUpMan.Domain
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public string? Email { get; private set; } // ← NUEVO CAMPO
        // ...resto del código...
    }
}
```

#### 2. Actualiza la Configuración en AppDbContext (si es necesario)

```csharp
// OpenUpMan.Data/AppDbContext.cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>(b =>
    {
        // ...configuración existente...
        b.Property(u => u.Email).HasColumnName("Email"); // ← NUEVO
    });
}
```

#### 3. Genera la Migración

```bash
# Desde la raíz del proyecto
cd C:\Users\eriar\RiderProjects\openup-manager

# Crear migración con nombre descriptivo
dotnet ef migrations add AgregarEmailAUsuario --project OpenUpMan.Data --startup-project OpenUpMan.UI
```

#### 4. Revisa los Archivos Generados

Se crean 3 archivos en `OpenUpMan.Data/Migrations/`:

```
Migrations/
├── [timestamp]_AgregarEmailAUsuario.cs              # ← Código de la migración
├── [timestamp]_AgregarEmailAUsuario.Designer.cs     # Metadatos
└── AppDbContextModelSnapshot.cs                      # Estado actual del modelo
```

**Ejemplo del archivo de migración:**
```csharp
public partial class AgregarEmailAUsuario : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Email",
            table: "users",
            type: "TEXT",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Email",
            table: "users");
    }
}
```

#### 5. Aplica la Migración

**Opción A: Automático (Recomendado para desarrollo)**
```bash
# Solo ejecuta la aplicación, las migraciones se aplican automáticamente
dotnet run --project OpenUpMan.UI
```

**Opción B: Manual**
```bash
dotnet ef database update --project OpenUpMan.Data --startup-project OpenUpMan.UI
```

---

## Ejemplos Prácticos

### Ejemplo 1: Agregar una Propiedad Nueva

**Escenario:** Agregar campo `PhoneNumber` a la entidad `User`

```csharp
// 1. Modificar User.cs
public class User
{
    // ...propiedades existentes...
    public string? PhoneNumber { get; private set; } // ← NUEVO
}

// 2. Actualizar AppDbContext.cs
modelBuilder.Entity<User>(b =>
{
    // ...configuración existente...
    b.Property(u => u.PhoneNumber).HasColumnName("PhoneNumber");
});
```

```bash
# 3. Crear migración
dotnet ef migrations add AgregarTelefonoUsuario --project OpenUpMan.Data --startup-project OpenUpMan.UI

# 4. Ejecutar aplicación (aplica automáticamente)
dotnet run --project OpenUpMan.UI
```

✅ **Resultado:** Columna `PhoneNumber` agregada a la tabla `users`

---

### Ejemplo 2: Eliminar una Propiedad Obsoleta

**Escenario:** Eliminar campo `OldField` de la entidad `Project`

```csharp
// 1. Eliminar de Project.cs
public class Project
{
    // public string? OldField { get; private set; } // ← ELIMINADO
}

// 2. Eliminar de AppDbContext.cs
modelBuilder.Entity<Project>(b =>
{
    // b.Property(p => p.OldField).HasColumnName("OldField"); // ← ELIMINADO
});
```

```bash
# 3. Crear migración
dotnet ef migrations add EliminarCampoObsoleto --project OpenUpMan.Data --startup-project OpenUpMan.UI

# 4. REVISAR el archivo generado - verás un DropColumn
```

**Archivo generado:**
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(
        name: "OldField",
        table: "projects");
}
```

```bash
# 5. Ejecutar aplicación
dotnet run --project OpenUpMan.UI
```

⚠️ **Advertencia:** La columna y todos sus datos serán eliminados permanentemente.

---

### Ejemplo 3: Crear una Nueva Entidad/Tabla

**Escenario:** Crear entidad `Department` con su tabla correspondiente

```csharp
// 1. Crear OpenUpMan.Domain/Department.cs
namespace OpenUpMan.Domain
{
    public class Department
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }

        protected Department() { }

        public Department(string name, string? description = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
        }
    }
}

// 2. Agregar DbSet en AppDbContext.cs
public DbSet<Department> Departments { get; set; } = null!;

// 3. Configurar en OnModelCreating
modelBuilder.Entity<Department>(b =>
{
    b.ToTable("departments");
    b.HasKey(d => d.Id);
    b.Property(d => d.Id).HasColumnName("Id");
    b.Property(d => d.Name).HasColumnName("Name").IsRequired();
    b.Property(d => d.Description).HasColumnName("Description");
});
```

```bash
# 4. Crear migración
dotnet ef migrations add AgregarTablaDepartamentos --project OpenUpMan.Data --startup-project OpenUpMan.UI

# 5. Ejecutar aplicación
dotnet run --project OpenUpMan.UI
```

✅ **Resultado:** Nueva tabla `departments` creada con todas sus columnas

---

### Ejemplo 4: Modificar Tipo de Dato

**Escenario:** Cambiar `Name` de `User` de 50 a 100 caracteres

```csharp
// En AppDbContext.cs
modelBuilder.Entity<User>(b =>
{
    b.Property(u => u.Name)
     .HasColumnName("Name")
     .HasMaxLength(100); // Cambiar de 50 a 100
});
```

```bash
dotnet ef migrations add AumentarLongitudNombreUsuario --project OpenUpMan.Data --startup-project OpenUpMan.UI
```

⚠️ **Nota:** En SQLite, algunos cambios de tipo requieren recrear la tabla.

---

### Ejemplo 5: Renombrar una Columna (SIN perder datos)

**⚠️ IMPORTANTE:** EF Core por defecto genera DROP + ADD, lo que pierde datos.

**Escenario:** Renombrar `Username` a `UserName`

```csharp
// 1. Cambiar en User.cs
public string UserName { get; private set; } = null!; // Antes: Username

// 2. Cambiar en AppDbContext.cs
b.Property(u => u.UserName).HasColumnName("UserName");
```

```bash
# 3. Crear migración
dotnet ef migrations add RenombrarUsername --project OpenUpMan.Data --startup-project OpenUpMan.UI
```

**4. EDITAR MANUALMENTE el archivo de migración:**

```csharp
// ❌ NO USAR - EF Core genera esto (pierde datos):
// migrationBuilder.DropColumn(name: "Username", table: "users");
// migrationBuilder.AddColumn<string>(name: "UserName", table: "users");

// ✅ USAR ESTO en su lugar:
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameColumn(
        name: "Username",
        table: "users",
        newName: "UserName");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameColumn(
        name: "UserName",
        table: "users",
        newName: "Username");
}
```

---

## Comportamiento de EF Core

### ¿Qué hace EF Core automáticamente?

| Acción | ¿Genera Migración? | ¿Se Aplica al Iniciar App? | ¿Pérdida de Datos? |
|--------|-------------------|---------------------------|-------------------|
| Agregar propiedad | ✅ Sí (AddColumn) | ✅ Sí | ❌ No |
| Eliminar propiedad | ✅ Sí (DropColumn) | ✅ Sí | ⚠️ **SÍ** |
| Agregar entidad/tabla | ✅ Sí (CreateTable) | ✅ Sí | ❌ No |
| Eliminar entidad/tabla | ✅ Sí (DropTable) | ✅ Sí | ⚠️ **SÍ** |
| Renombrar propiedad | ✅ Sí (Drop+Add) | ✅ Sí | ⚠️ **SÍ** (editar manualmente) |
| Cambiar tipo de dato | ✅ Sí (AlterColumn) | ✅ Sí | ⚠️ Depende |
| Agregar índice | ✅ Sí (CreateIndex) | ✅ Sí | ❌ No |
| Agregar FK | ✅ Sí (AddForeignKey) | ✅ Sí | ❌ No |

### Proceso de EF Core

```
1. Modificas entidades en código
           ↓
2. Ejecutas: dotnet ef migrations add NombreMigración
           ↓
3. EF Core compara el modelo actual con el snapshot
           ↓
4. Genera archivos de migración en OpenUpMan.Data/Migrations/
           ↓
5. TÚ revisas los archivos generados
           ↓
6. Ejecutas la aplicación
           ↓
7. ctx.Database.Migrate() aplica migraciones pendientes
           ↓
8. Base de datos actualizada ✅
```

### Comparación con el Migrador Personalizado Anterior

| Característica | Migrador Personalizado | EF Core Migrations |
|---------------|----------------------|-------------------|
| **Detección de cambios** | Automática al iniciar | Manual: tú creas la migración |
| **Revisión de cambios** | ❌ No | ✅ Sí (archivos generados) |
| **Historial** | ❌ No | ✅ Control de versiones |
| **Reversibilidad** | ❌ Imposible | ✅ Fácil (método Down) |
| **Eliminar columnas** | ✅ Automático si configurado | ⚠️ Manual (generas DROP) |
| **Trabajo en equipo** | ⚠️ Conflictos | ✅ Sin conflictos |
| **Scripts SQL** | ❌ No disponible | ✅ Exportable |
| **Seguridad** | ⚠️ Riesgoso (cambios inmediatos) | ✅ Seguro (revisas antes) |

---

## Preguntas Frecuentes

### ❓ ¿Cuándo debo crear una migración?

**Siempre que modifiques:**
- Propiedades de entidades (agregar, eliminar, renombrar)
- Entidades completas (agregar, eliminar)
- Configuración de DbContext (índices, claves foráneas, tipos)
- Relaciones entre entidades

### ❓ ¿Las migraciones se aplican automáticamente?

**Sí**, al iniciar la aplicación, `ctx.Database.Migrate()` en `Program.cs` aplica todas las migraciones pendientes automáticamente.

```csharp
// En OpenUpMan.UI/Program.cs
ctx.Database.Migrate(); // ← Aplica migraciones automáticamente
```

### ❓ ¿EF Core elimina columnas automáticamente?

**Sí, pero con un paso intermedio:**

1. Eliminas la propiedad de la entidad
2. Ejecutas `dotnet ef migrations add`
3. EF Core **genera** un archivo con `DropColumn`
4. El archivo queda en tu carpeta `Migrations/`
5. **Nada pasa aún** a la base de datos
6. Cuando ejecutas la app, la columna se elimina

**Ventaja:** Puedes revisar el archivo antes de que se aplique.

### ❓ ¿Cómo revierto una migración?

```bash
# Si NO has aplicado la migración (solo la creaste)
dotnet ef migrations remove --project OpenUpMan.Data --startup-project OpenUpMan.UI

# Si YA aplicaste la migración (está en la BD)
dotnet ef database update MigraciónAnterior --project OpenUpMan.Data --startup-project OpenUpMan.UI
```

### ❓ ¿Qué pasa si dos desarrolladores crean migraciones?

1. Desarrollador A crea `20251201120000_MigrationA.cs`
2. Desarrollador B crea `20251201130000_MigrationB.cs`
3. Ambos hacen commit
4. Al hacer merge, Git combina los archivos sin conflicto
5. EF Core aplica ambas migraciones en orden de timestamp ✅

### ❓ ¿Dónde está la base de datos?

```
%LocalAppData%\openup.db
C:\Users\TuUsuario\AppData\Local\openup.db
```

### ❓ ¿Puedo ver el SQL antes de aplicarlo?

**Sí:**
```bash
dotnet ef migrations script --project OpenUpMan.Data --startup-project OpenUpMan.UI
```

Esto genera el SQL completo sin ejecutarlo.

### ❓ ¿Cómo borro toda la BD y empiezo de nuevo?

```bash
# Opción 1: Borrar archivo de BD
Remove-Item "$env:LOCALAPPDATA\openup.db" -Force

# Opción 2: Revertir a migración 0 (BD vacía)
dotnet ef database update 0 --project OpenUpMan.Data --startup-project OpenUpMan.UI
```

### ❓ ¿Qué hago en producción?

**NO uses `ctx.Database.Migrate()` en producción.**

En su lugar:
1. Genera script SQL:
   ```bash
   dotnet ef migrations script --output deploy.sql --project OpenUpMan.Data --startup-project OpenUpMan.UI
   ```
2. Revisa el script
3. Ejecuta manualmente en producción
4. Comenta `ctx.Database.Migrate()` en producción

---

## 🚨 Reglas de Oro

### ✅ SIEMPRE hacer:
1. Crear migración después de modificar entidades
2. Revisar archivos generados antes de aplicar
3. Hacer commit de los archivos de migración
4. Probar localmente antes de push
5. Hacer backup antes de eliminar columnas/tablas

### ❌ NUNCA hacer:
1. Editar migraciones ya aplicadas (crear nueva en su lugar)
2. Borrar archivos de migración del proyecto
3. Aplicar migraciones directamente en producción sin revisar
4. Ignorar archivos de migración en `.gitignore`
5. Renombrar columnas sin editar manualmente la migración

---

## 🛠️ Solución de Problemas

### Error: "No DbContext was found"

**Solución:**
```bash
# Asegúrate de especificar ambos proyectos
--project OpenUpMan.Data --startup-project OpenUpMan.UI
```

### Error: "Build failed"

**Solución:**
```bash
# Compila primero
dotnet build
```

### Error: "The migration has already been applied"

**No es un error** - significa que la base de datos está actualizada.

### Error: "Unable to create an object of type AppDbContext"

**Solución:** Verifica que existe `DesignTimeDbContextFactory.cs` en `OpenUpMan.Data/`

---

## 📚 Recursos Adicionales

- [Documentación Oficial EF Core](https://learn.microsoft.com/es-es/ef/core/managing-schemas/migrations/)
- [Comandos CLI](https://learn.microsoft.com/es-es/ef/core/cli/dotnet)
- Ver también:
  - `README_EF_CORE_MIGRATION.md` (raíz del proyecto)
  - `EF_CORE_QUICK_REFERENCE.md` (raíz del proyecto)
  - `MIGRATION_SUMMARY.md` (raíz del proyecto)

---

## 📞 Ayuda Rápida

```bash
# Comando más usado
dotnet ef migrations add NombreDescriptivo --project OpenUpMan.Data --startup-project OpenUpMan.UI

# Ver estado
dotnet ef migrations list --project OpenUpMan.Data --startup-project OpenUpMan.UI

# Deshacer última migración (si no se aplicó)
dotnet ef migrations remove --project OpenUpMan.Data --startup-project OpenUpMan.UI
```

---

**Última actualización: 1 de diciembre de 2025**
**Versión EF Core: 9.0.0**

