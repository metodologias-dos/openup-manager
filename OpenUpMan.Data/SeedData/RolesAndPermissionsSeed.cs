using OpenUpMan.Domain;

namespace OpenUpMan.Data.SeedData
{
    /// <summary>
    /// Datos de inicialización para roles, permisos y sus relaciones
    /// </summary>
    public static class RolesAndPermissionsSeed
    {
        /// <summary>
        /// Obtiene todos los permisos del sistema
        /// </summary>
        public static List<Permission> GetPermissions()
        {
            return new List<Permission>
            {
                // Permisos de Proyecto
                new Permission { Id = PermissionIds.ProyectoVer, Name = "proyecto:ver", Description = "Ver detalles del proyecto" },
                new Permission { Id = PermissionIds.ProyectoActualizar, Name = "proyecto:actualizar", Description = "Actualizar información del proyecto" },
                new Permission { Id = PermissionIds.ProyectoBorrar, Name = "proyecto:borrar", Description = "Eliminar el proyecto" },
                new Permission { Id = PermissionIds.ProyectoRenombrar, Name = "proyecto:renombrar", Description = "Renombrar el proyecto" },
                new Permission { Id = PermissionIds.ProyectoCambiarEstado, Name = "proyecto:cambiar-estado", Description = "Cambiar estado del proyecto" },
                
                // Permisos de Usuarios
                new Permission { Id = PermissionIds.UsuariosAgregar, Name = "usuarios:agregar", Description = "Agregar usuarios al proyecto" },
                new Permission { Id = PermissionIds.UsuariosEliminar, Name = "usuarios:eliminar", Description = "Eliminar usuarios del proyecto" },
                new Permission { Id = PermissionIds.UsuariosVer, Name = "usuarios:ver", Description = "Ver usuarios del proyecto" },
                new Permission { Id = PermissionIds.UsuariosModificarRoles, Name = "usuarios:modificar-roles", Description = "Modificar roles de usuarios" },
                
                // Permisos de Fases
                new Permission { Id = PermissionIds.FasesVer, Name = "fases:ver", Description = "Ver fases del proyecto" },
                new Permission { Id = PermissionIds.FasesCrear, Name = "fases:crear", Description = "Crear nuevas fases" },
                new Permission { Id = PermissionIds.FasesActualizar, Name = "fases:actualizar", Description = "Actualizar información de fases" },
                new Permission { Id = PermissionIds.FasesAvanzar, Name = "fases:avanzar", Description = "Avanzar a la siguiente fase" },
                new Permission { Id = PermissionIds.FasesCambiarEstado, Name = "fases:cambiar-estado", Description = "Cambiar estado de fase" },
                
                // Permisos de Iteraciones
                new Permission { Id = PermissionIds.IteracionesVer, Name = "iteraciones:ver", Description = "Ver iteraciones" },
                new Permission { Id = PermissionIds.IteracionesCrear, Name = "iteraciones:crear", Description = "Crear nuevas iteraciones" },
                new Permission { Id = PermissionIds.IteracionesActualizar, Name = "iteraciones:actualizar", Description = "Actualizar iteraciones" },
                new Permission { Id = PermissionIds.IteracionesAvanzar, Name = "iteraciones:avanzar", Description = "Avanzar/completar iteraciones" },
                new Permission { Id = PermissionIds.IteracionesEliminar, Name = "iteraciones:eliminar", Description = "Eliminar iteraciones" },
                
                // Permisos de Microincrementos
                new Permission { Id = PermissionIds.MicroincrementosVer, Name = "microincrementos:ver", Description = "Ver microincrementos" },
                new Permission { Id = PermissionIds.MicroincrementosCrear, Name = "microincrementos:crear", Description = "Crear microincrementos" },
                new Permission { Id = PermissionIds.MicroincrementosActualizar, Name = "microincrementos:actualizar", Description = "Actualizar microincrementos" },
                new Permission { Id = PermissionIds.MicroincrementosEliminar, Name = "microincrementos:eliminar", Description = "Eliminar microincrementos" },
                new Permission { Id = PermissionIds.MicroincrementosAgregarDocumentos, Name = "microincrementos:agregar-documentos", Description = "Agregar documentos a microincrementos" },
                
                // Permisos de Artefactos
                new Permission { Id = PermissionIds.ArtefactosVer, Name = "artefactos:ver", Description = "Ver artefactos" },
                new Permission { Id = PermissionIds.ArtefactosCrear, Name = "artefactos:crear", Description = "Crear artefactos" },
                new Permission { Id = PermissionIds.ArtefactosActualizar, Name = "artefactos:actualizar", Description = "Actualizar artefactos" },
                new Permission { Id = PermissionIds.ArtefactosEliminar, Name = "artefactos:eliminar", Description = "Eliminar artefactos" },
                new Permission { Id = PermissionIds.ArtefactosSubirVersion, Name = "artefactos:subir-version", Description = "Subir versiones de artefactos" },
                new Permission { Id = PermissionIds.ArtefactosDescargar, Name = "artefactos:descargar", Description = "Descargar artefactos" },
                new Permission { Id = PermissionIds.ArtefactosMarcarObligatorios, Name = "artefactos:marcar-obligatorios", Description = "Marcar artefactos como obligatorios" },
                new Permission { Id = PermissionIds.ArtefactosCambiarEstado, Name = "artefactos:cambiar-estado", Description = "Cambiar estado de artefactos" },
                
                // Permisos Especiales
                new Permission { Id = PermissionIds.SoloLectura, Name = "solo-lectura", Description = "Acceso de solo lectura a todo" },
                new Permission { Id = PermissionIds.ArtefactosMinimos, Name = "artefactos:minimos", Description = "Gestionar artefactos mínimos" },
                
                // Permisos de Reportes
                new Permission { Id = PermissionIds.ReportesVer, Name = "reportes:ver", Description = "Ver reportes y métricas" },
                new Permission { Id = PermissionIds.ReportesGenerar, Name = "reportes:generar", Description = "Generar reportes personalizados" },
            };
        }

        /// <summary>
        /// Obtiene todos los roles del sistema
        /// </summary>
        public static List<Role> GetRoles()
        {
            return new List<Role>
            {
                new Role(RoleIds.Admin, "Administrador", "Acceso completo al sistema, equivalente al Autor"),
                new Role(RoleIds.ProductOwner, "Product Owner", "Gestión del producto y artefactos"),
                new Role(RoleIds.ScrumMaster, "Scrum Master", "Facilitador del equipo y gestión de microiteraciones"),
                new Role(RoleIds.Desarrollador, "Desarrollador", "Desarrollo y entrega de artefactos"),
                new Role(RoleIds.Tester, "Tester", "Pruebas y entrega de artefactos de testing"),
                new Role(RoleIds.Revisor, "Revisor", "Solo lectura de todo el proyecto"),
                new Role(RoleIds.Autor, "Autor", "Creador del proyecto con todos los permisos"),
                new Role(RoleIds.Viewer, "Viewer", "Visualización básica del proyecto"),
            };
        }

        /// <summary>
        /// Obtiene las relaciones entre roles y permisos
        /// </summary>
        public static List<RolePermission> GetRolePermissions()
        {
            var rolePermissions = new List<RolePermission>();
            int id = 1;

            // ========== ROL: AUTOR (todos los permisos) ==========
            var autorPermissions = new[]
            {
                PermissionIds.ProyectoVer,
                PermissionIds.ProyectoActualizar,
                PermissionIds.ProyectoBorrar,
                PermissionIds.ProyectoRenombrar,
                PermissionIds.ProyectoCambiarEstado,
                PermissionIds.UsuariosAgregar,
                PermissionIds.UsuariosEliminar,
                PermissionIds.UsuariosVer,
                PermissionIds.UsuariosModificarRoles,
                PermissionIds.FasesVer,
                PermissionIds.FasesCrear,
                PermissionIds.FasesActualizar,
                PermissionIds.FasesAvanzar,
                PermissionIds.FasesCambiarEstado,
                PermissionIds.IteracionesVer,
                PermissionIds.IteracionesCrear,
                PermissionIds.IteracionesActualizar,
                PermissionIds.IteracionesAvanzar,
                PermissionIds.IteracionesEliminar,
                PermissionIds.MicroincrementosVer,
                PermissionIds.MicroincrementosCrear,
                PermissionIds.MicroincrementosActualizar,
                PermissionIds.MicroincrementosEliminar,
                PermissionIds.MicroincrementosAgregarDocumentos,
                PermissionIds.ArtefactosVer,
                PermissionIds.ArtefactosCrear,
                PermissionIds.ArtefactosActualizar,
                PermissionIds.ArtefactosEliminar,
                PermissionIds.ArtefactosSubirVersion,
                PermissionIds.ArtefactosDescargar,
                PermissionIds.ArtefactosMarcarObligatorios,
                PermissionIds.ArtefactosCambiarEstado,
                PermissionIds.ArtefactosMinimos,
                PermissionIds.ReportesVer,
                PermissionIds.ReportesGenerar,
            };

            foreach (var permissionId in autorPermissions)
            {
                rolePermissions.Add(new RolePermission { Id = id++, RoleId = RoleIds.Autor, PermissionId = permissionId });
            }

            // ========== ROL: ADMIN (mismos permisos que Autor) ==========
            foreach (var permissionId in autorPermissions)
            {
                rolePermissions.Add(new RolePermission { Id = id++, RoleId = RoleIds.Admin, PermissionId = permissionId });
            }

            // ========== ROL: REVISOR (solo lectura) ==========
            var revisorPermissions = new[]
            {
                PermissionIds.SoloLectura,
                PermissionIds.ProyectoVer,
                PermissionIds.UsuariosVer,
                PermissionIds.FasesVer,
                PermissionIds.IteracionesVer,
                PermissionIds.MicroincrementosVer,
                PermissionIds.ArtefactosVer,
                PermissionIds.ArtefactosDescargar,
                PermissionIds.ReportesVer,
            };

            foreach (var permissionId in revisorPermissions)
            {
                rolePermissions.Add(new RolePermission { Id = id++, RoleId = RoleIds.Revisor, PermissionId = permissionId });
            }

            // ========== ROL: PRODUCT OWNER (casi todos menos borrar) ==========
            var poPermissions = new[]
            {
                PermissionIds.ProyectoVer,
                PermissionIds.ProyectoActualizar,
                PermissionIds.ProyectoRenombrar,
                PermissionIds.ProyectoCambiarEstado,
                PermissionIds.UsuariosVer,
                PermissionIds.FasesVer,
                PermissionIds.FasesCrear,
                PermissionIds.FasesActualizar,
                PermissionIds.FasesAvanzar,
                PermissionIds.FasesCambiarEstado,
                PermissionIds.IteracionesVer,
                PermissionIds.IteracionesCrear,
                PermissionIds.IteracionesActualizar,
                PermissionIds.IteracionesAvanzar,
                PermissionIds.MicroincrementosVer,
                PermissionIds.MicroincrementosCrear,
                PermissionIds.MicroincrementosActualizar,
                PermissionIds.MicroincrementosAgregarDocumentos,
                PermissionIds.ArtefactosVer,
                PermissionIds.ArtefactosCrear,
                PermissionIds.ArtefactosActualizar,
                PermissionIds.ArtefactosSubirVersion,
                PermissionIds.ArtefactosDescargar,
                PermissionIds.ArtefactosMarcarObligatorios,
                PermissionIds.ArtefactosCambiarEstado,
                PermissionIds.ArtefactosMinimos,
                PermissionIds.ReportesVer,
                PermissionIds.ReportesGenerar,
            };

            foreach (var permissionId in poPermissions)
            {
                rolePermissions.Add(new RolePermission { Id = id++, RoleId = RoleIds.ProductOwner, PermissionId = permissionId });
            }

            // ========== ROL: SCRUM MASTER (similar al PO, sin avanzar iteraciones completas) ==========
            var smPermissions = new[]
            {
                PermissionIds.ProyectoVer,
                PermissionIds.UsuariosVer,
                PermissionIds.FasesVer,
                PermissionIds.IteracionesVer,
                PermissionIds.IteracionesActualizar,
                PermissionIds.MicroincrementosVer,
                PermissionIds.MicroincrementosCrear,
                PermissionIds.MicroincrementosActualizar,
                PermissionIds.MicroincrementosAgregarDocumentos,
                PermissionIds.ArtefactosVer,
                PermissionIds.ArtefactosCrear,
                PermissionIds.ArtefactosActualizar,
                PermissionIds.ArtefactosSubirVersion,
                PermissionIds.ArtefactosDescargar,
                PermissionIds.ArtefactosMinimos,
                PermissionIds.ReportesVer,
            };

            foreach (var permissionId in smPermissions)
            {
                rolePermissions.Add(new RolePermission { Id = id++, RoleId = RoleIds.ScrumMaster, PermissionId = permissionId });
            }

            // ========== ROL: DESARROLLADOR (solo agregar documentos y artefactos) ==========
            var devPermissions = new[]
            {
                PermissionIds.ProyectoVer,
                PermissionIds.UsuariosVer,
                PermissionIds.FasesVer,
                PermissionIds.IteracionesVer,
                PermissionIds.MicroincrementosVer,
                PermissionIds.MicroincrementosAgregarDocumentos,
                PermissionIds.ArtefactosVer,
                PermissionIds.ArtefactosSubirVersion,
                PermissionIds.ArtefactosDescargar,
                PermissionIds.ReportesVer,
            };

            foreach (var permissionId in devPermissions)
            {
                rolePermissions.Add(new RolePermission { Id = id++, RoleId = RoleIds.Desarrollador, PermissionId = permissionId });
            }

            // ========== ROL: TESTER (similar a Desarrollador) ==========
            var testerPermissions = new[]
            {
                PermissionIds.ProyectoVer,
                PermissionIds.UsuariosVer,
                PermissionIds.FasesVer,
                PermissionIds.IteracionesVer,
                PermissionIds.MicroincrementosVer,
                PermissionIds.MicroincrementosAgregarDocumentos,
                PermissionIds.ArtefactosVer,
                PermissionIds.ArtefactosSubirVersion,
                PermissionIds.ArtefactosDescargar,
                PermissionIds.ReportesVer,
            };

            foreach (var permissionId in testerPermissions)
            {
                rolePermissions.Add(new RolePermission { Id = id++, RoleId = RoleIds.Tester, PermissionId = permissionId });
            }

            // ========== ROL: VIEWER (solo lectura básica) ==========
            var viewerPermissions = new[]
            {
                PermissionIds.ProyectoVer,
                PermissionIds.FasesVer,
                PermissionIds.IteracionesVer,
                PermissionIds.ReportesVer,
            };

            foreach (var permissionId in viewerPermissions)
            {
                rolePermissions.Add(new RolePermission { Id = id++, RoleId = RoleIds.Viewer, PermissionId = permissionId });
            }

            return rolePermissions;
        }
    }
}

