# Sistema de Roles y Permisos Granulares

## Descripción General

El sistema OpenUpMan implementa un modelo de permisos granulares similar a AWS, donde cada acción sobre el sistema requiere un permiso específico. Los permisos se asignan a roles y estos roles se asignan a usuarios por proyecto.

## Arquitectura

### 1. **Permisos** (Granulares)
Los permisos son las acciones específicas que un usuario puede realizar en el sistema. Están definidos en `PermissionIds.cs` con IDs constantes.

#### Categorías de Permisos:

**Proyecto**
- `proyecto:ver` - Ver detalles del proyecto
- `proyecto:actualizar` - Actualizar información del proyecto
- `proyecto:borrar` - Eliminar el proyecto
- `proyecto:renombrar` - Renombrar el proyecto
- `proyecto:cambiar-estado` - Cambiar estado del proyecto

**Usuarios**
- `usuarios:agregar` - Agregar usuarios al proyecto
- `usuarios:eliminar` - Eliminar usuarios del proyecto
- `usuarios:ver` - Ver usuarios del proyecto
- `usuarios:modificar-roles` - Modificar roles de usuarios

**Fases**
- `fases:ver` - Ver fases del proyecto
- `fases:crear` - Crear nuevas fases
- `fases:actualizar` - Actualizar información de fases
- `fases:avanzar` - Avanzar a la siguiente fase
- `fases:cambiar-estado` - Cambiar estado de fase

**Iteraciones**
- `iteraciones:ver` - Ver iteraciones
- `iteraciones:crear` - Crear nuevas iteraciones
- `iteraciones:actualizar` - Actualizar iteraciones
- `iteraciones:avanzar` - Avanzar/completar iteraciones
- `iteraciones:eliminar` - Eliminar iteraciones

**Microincrementos**
- `microincrementos:ver` - Ver microincrementos
- `microincrementos:crear` - Crear microincrementos
- `microincrementos:actualizar` - Actualizar microincrementos
- `microincrementos:eliminar` - Eliminar microincrementos
- `microincrementos:agregar-documentos` - Agregar documentos a microincrementos

**Artefactos**
- `artefactos:ver` - Ver artefactos
- `artefactos:crear` - Crear artefactos
- `artefactos:actualizar` - Actualizar artefactos
- `artefactos:eliminar` - Eliminar artefactos
- `artefactos:subir-version` - Subir versiones de artefactos
- `artefactos:descargar` - Descargar artefactos
- `artefactos:marcar-obligatorios` - Marcar artefactos como obligatorios
- `artefactos:cambiar-estado` - Cambiar estado de artefactos
- `artefactos:minimos` - Gestionar artefactos mínimos

**Especiales**
- `solo-lectura` - Acceso de solo lectura a todo

**Reportes**
- `reportes:ver` - Ver reportes y métricas
- `reportes:generar` - Generar reportes personalizados

---

### 2. **Roles** (Colecciones de Permisos)

Los roles agrupan permisos y se asignan a usuarios por proyecto.

#### Roles Predefinidos:

| ROL | ID | DESCRIPCIÓN |
|-----|-----|-------------|
| **Autor** | 7 | Creador del proyecto con todos los permisos |
| **Administrador** | 1 | Acceso completo (igual que Autor) |
| **Product Owner** | 2 | Gestión del producto, casi todos los permisos excepto borrar proyecto |
| **Scrum Master** | 3 | Facilitador del equipo, gestión de microiteraciones |
| **Desarrollador** | 4 | Entrega de artefactos y documentos |
| **Tester** | 5 | Pruebas y entrega de artefactos |
| **Revisor** | 6 | Solo lectura de todo el proyecto |
| **Viewer** | 8 | Visualización básica del proyecto |

---

### 3. **Matriz de Roles vs Permisos**

#### Autor / Administrador (Todos los permisos)
✅ Proyecto: ver, actualizar, borrar, renombrar, cambiar-estado  
✅ Usuarios: agregar, eliminar, ver, modificar-roles  
✅ Fases: ver, crear, actualizar, avanzar, cambiar-estado  
✅ Iteraciones: ver, crear, actualizar, avanzar, eliminar  
✅ Microincrementos: ver, crear, actualizar, eliminar, agregar-documentos  
✅ Artefactos: ver, crear, actualizar, eliminar, subir-version, descargar, marcar-obligatorios, cambiar-estado, mínimos  
✅ Reportes: ver, generar  

#### Revisor (Solo Lectura)
✅ solo-lectura  
✅ Proyecto: ver  
✅ Usuarios: ver  
✅ Fases: ver  
✅ Iteraciones: ver  
✅ Microincrementos: ver  
✅ Artefactos: ver, descargar  
✅ Reportes: ver  

#### Product Owner (Gestión sin borrar)
✅ Proyecto: ver, actualizar, renombrar, cambiar-estado  
✅ Usuarios: ver  
✅ Fases: ver, crear, actualizar, avanzar, cambiar-estado  
✅ Iteraciones: ver, crear, actualizar, avanzar  
✅ Microincrementos: ver, crear, actualizar, agregar-documentos  
✅ Artefactos: ver, crear, actualizar, subir-version, descargar, marcar-obligatorios, cambiar-estado, mínimos  
✅ Reportes: ver, generar  

#### Scrum Master (Facilitador)
✅ Proyecto: ver  
✅ Usuarios: ver  
✅ Fases: ver  
✅ Iteraciones: ver, actualizar  
✅ Microincrementos: ver, crear, actualizar, agregar-documentos  
✅ Artefactos: ver, crear, actualizar, subir-version, descargar, mínimos  
✅ Reportes: ver  

#### Desarrollador / Tester (Entrega)
✅ Proyecto: ver  
✅ Usuarios: ver  
✅ Fases: ver  
✅ Iteraciones: ver  
✅ Microincrementos: ver, agregar-documentos  
✅ Artefactos: ver, subir-version, descargar  
✅ Reportes: ver  

#### Viewer (Vista Básica)
✅ Proyecto: ver  
✅ Fases: ver  
✅ Iteraciones: ver  
✅ Reportes: ver  

---

## Inicialización Automática

El sistema inicializa automáticamente los roles y permisos al crear la base de datos mediante **Entity Framework Core Seed Data** en `AppDbContext.cs`.

### Archivos Clave:

1. **`PermissionIds.cs`** - Define los IDs de permisos (constantes)
2. **`RoleIds.cs`** - Define los IDs de roles (constantes)
3. **`RolesAndPermissionsSeed.cs`** - Contiene los datos de inicialización
4. **`AppDbContext.cs`** - Aplica el seed data mediante `HasData()`

### Proceso de Inicialización:

```csharp
// En AppDbContext.OnModelCreating()
SeedRolesAndPermissions(modelBuilder);
```

Esto garantiza que:
- Los 36 permisos siempre existan
- Los 8 roles siempre estén disponibles
- Las relaciones rol-permiso estén configuradas correctamente

---

## Uso en Código

### Verificar Permisos

```csharp
// Ejemplo de verificación de permiso
public async Task<bool> UserHasPermission(int userId, int projectId, int permissionId)
{
    return await _context.ProjectUsers
        .Where(pu => pu.UserId == userId && pu.ProjectId == projectId)
        .Join(_context.RolePermissions,
            pu => pu.RoleId,
            rp => rp.RoleId,
            (pu, rp) => rp)
        .AnyAsync(rp => rp.PermissionId == permissionId);
}
```

### Asignar Rol a Usuario en Proyecto

```csharp
// Ejemplo de asignación de rol
var projectUser = new ProjectUser(
    projectId: 1,
    userId: 2,
    roleId: RoleIds.Desarrollador
);

await _context.ProjectUsers.AddAsync(projectUser);
await _context.SaveChangesAsync();
```

---

## Migración

Para crear la migración que incluye los datos iniciales:

```bash
dotnet ef migrations add AddRolesAndPermissionsSeedData --project OpenUpMan.Data
dotnet ef database update --project OpenUpMan.Data
```

---

## Extensibilidad

### Agregar Nuevos Permisos:

1. Agregar constante en `PermissionIds.cs`
2. Agregar permiso en `RolesAndPermissionsSeed.GetPermissions()`
3. Asignar permiso a roles en `RolesAndPermissionsSeed.GetRolePermissions()`
4. Crear nueva migración

### Agregar Nuevos Roles:

1. Agregar constante en `RoleIds.cs`
2. Agregar rol en `RolesAndPermissionsSeed.GetRoles()`
3. Configurar permisos en `RolesAndPermissionsSeed.GetRolePermissions()`
4. Crear nueva migración

---

## Notas Importantes

⚠️ **Los IDs están predefinidos**: Los permisos y roles tienen IDs fijos para facilitar referencias en el código.

⚠️ **No modificar IDs existentes**: Cambiar IDs requeriría migración de datos.

⚠️ **Permisos inmutables**: Los permisos son de solo lectura después de la inicialización.

⚠️ **Un usuario puede tener múltiples roles**: Un usuario puede tener diferentes roles en diferentes proyectos.

✅ **Permisos granulares**: Permite control fino sobre quién puede hacer qué.

✅ **Similar a AWS IAM**: Modelo probado y escalable.

✅ **Seed automático**: No requiere scripts SQL manuales.

