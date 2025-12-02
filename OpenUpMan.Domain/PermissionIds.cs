namespace OpenUpMan.Domain
{
    /// <summary>
    /// IDs de permisos predefinidos del sistema (granulares tipo AWS)
    /// </summary>
    public static class PermissionIds
    {
        // ========== PERMISOS DE PROYECTO ==========
        
        /// <summary>
        /// Permite ver detalles básicos del proyecto
        /// </summary>
        public const int ProyectoVer = 1;
        
        /// <summary>
        /// Permite actualizar información del proyecto (nombre, descripción, etc)
        /// </summary>
        public const int ProyectoActualizar = 2;
        
        /// <summary>
        /// Permite eliminar/borrar el proyecto
        /// </summary>
        public const int ProyectoBorrar = 3;
        
        /// <summary>
        /// Permite renombrar el proyecto
        /// </summary>
        public const int ProyectoRenombrar = 4;
        
        /// <summary>
        /// Permite cambiar el estado del proyecto
        /// </summary>
        public const int ProyectoCambiarEstado = 5;
        
        // ========== PERMISOS DE USUARIOS DEL PROYECTO ==========
        
        /// <summary>
        /// Permite agregar usuarios al proyecto
        /// </summary>
        public const int UsuariosAgregar = 6;
        
        /// <summary>
        /// Permite eliminar usuarios del proyecto
        /// </summary>
        public const int UsuariosEliminar = 7;
        
        /// <summary>
        /// Permite ver la lista de usuarios del proyecto
        /// </summary>
        public const int UsuariosVer = 8;
        
        /// <summary>
        /// Permite modificar roles de usuarios en el proyecto
        /// </summary>
        public const int UsuariosModificarRoles = 9;
        
        // ========== PERMISOS DE FASES ==========
        
        /// <summary>
        /// Permite ver fases del proyecto
        /// </summary>
        public const int FasesVer = 10;
        
        /// <summary>
        /// Permite crear nuevas fases
        /// </summary>
        public const int FasesCrear = 11;
        
        /// <summary>
        /// Permite actualizar información de fases
        /// </summary>
        public const int FasesActualizar = 12;
        
        /// <summary>
        /// Permite avanzar a la siguiente fase
        /// </summary>
        public const int FasesAvanzar = 13;
        
        /// <summary>
        /// Permite cambiar el estado de una fase
        /// </summary>
        public const int FasesCambiarEstado = 14;
        
        // ========== PERMISOS DE ITERACIONES ==========
        
        /// <summary>
        /// Permite ver iteraciones
        /// </summary>
        public const int IteracionesVer = 15;
        
        /// <summary>
        /// Permite crear nuevas iteraciones
        /// </summary>
        public const int IteracionesCrear = 16;
        
        /// <summary>
        /// Permite actualizar información de iteraciones
        /// </summary>
        public const int IteracionesActualizar = 17;
        
        /// <summary>
        /// Permite avanzar/completar iteraciones
        /// </summary>
        public const int IteracionesAvanzar = 18;
        
        /// <summary>
        /// Permite eliminar iteraciones
        /// </summary>
        public const int IteracionesEliminar = 19;
        
        // ========== PERMISOS DE MICROINCREMENTOS ==========
        
        /// <summary>
        /// Permite ver microincrementos
        /// </summary>
        public const int MicroincrementosVer = 20;
        
        /// <summary>
        /// Permite crear/registrar microincrementos
        /// </summary>
        public const int MicroincrementosCrear = 21;
        
        /// <summary>
        /// Permite actualizar microincrementos
        /// </summary>
        public const int MicroincrementosActualizar = 22;
        
        /// <summary>
        /// Permite eliminar microincrementos
        /// </summary>
        public const int MicroincrementosEliminar = 23;
        
        /// <summary>
        /// Permite agregar documentos/evidencias a microincrementos
        /// </summary>
        public const int MicroincrementosAgregarDocumentos = 24;
        
        // ========== PERMISOS DE ARTEFACTOS ==========
        
        /// <summary>
        /// Permite ver artefactos
        /// </summary>
        public const int ArtefactosVer = 25;
        
        /// <summary>
        /// Permite crear nuevos artefactos
        /// </summary>
        public const int ArtefactosCrear = 26;
        
        /// <summary>
        /// Permite actualizar información de artefactos
        /// </summary>
        public const int ArtefactosActualizar = 27;
        
        /// <summary>
        /// Permite eliminar artefactos
        /// </summary>
        public const int ArtefactosEliminar = 28;
        
        /// <summary>
        /// Permite subir versiones de artefactos
        /// </summary>
        public const int ArtefactosSubirVersion = 29;
        
        /// <summary>
        /// Permite descargar artefactos
        /// </summary>
        public const int ArtefactosDescargar = 30;
        
        /// <summary>
        /// Permite marcar artefactos como obligatorios
        /// </summary>
        public const int ArtefactosMarcarObligatorios = 31;
        
        /// <summary>
        /// Permite cambiar el estado de artefactos (PENDING/DELIVERED)
        /// </summary>
        public const int ArtefactosCambiarEstado = 32;
        
        // ========== PERMISOS ESPECIALES ==========
        
        /// <summary>
        /// Acceso de solo lectura a todo el proyecto
        /// </summary>
        public const int SoloLectura = 33;
        
        /// <summary>
        /// Permite gestionar los artefactos mínimos del proyecto
        /// </summary>
        public const int ArtefactosMinimos = 34;
        
        // ========== PERMISOS DE REPORTES Y MÉTRICAS ==========
        
        /// <summary>
        /// Permite ver reportes y métricas del proyecto
        /// </summary>
        public const int ReportesVer = 35;
        
        /// <summary>
        /// Permite generar reportes personalizados
        /// </summary>
        public const int ReportesGenerar = 36;
    }
}

