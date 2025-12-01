namespace OpenUpMan.Domain
{
    /// <summary>
    /// Clase estática que contiene los IDs de los roles predefinidos del sistema.
    /// Estos IDs deben coincidir con los roles creados en la inicialización de la base de datos.
    /// </summary>
    public static class RoleIds
    {
        /// <summary>
        /// Rol de Administrador - Acceso completo al sistema
        /// </summary>
        public static readonly Guid Admin = Guid.Parse("00000000-0000-0000-0000-000000000001");

        /// <summary>
        /// Rol de Product Owner - Gestión del producto
        /// </summary>
        public static readonly Guid ProductOwner = Guid.Parse("00000000-0000-0000-0000-000000000002");

        /// <summary>
        /// Rol de Scrum Master - Facilitador del equipo
        /// </summary>
        public static readonly Guid ScrumMaster = Guid.Parse("00000000-0000-0000-0000-000000000003");

        /// <summary>
        /// Rol de Desarrollador - Desarrollo de software
        /// </summary>
        public static readonly Guid Desarrollador = Guid.Parse("00000000-0000-0000-0000-000000000004");

        /// <summary>
        /// Rol de Tester - Pruebas y QA
        /// </summary>
        public static readonly Guid Tester = Guid.Parse("00000000-0000-0000-0000-000000000005");

        /// <summary>
        /// Rol de Revisor - Revisión de documentos y código
        /// </summary>
        public static readonly Guid Revisor = Guid.Parse("00000000-0000-0000-0000-000000000006");

        /// <summary>
        /// Rol de Autor - Creación de contenido
        /// </summary>
        public static readonly Guid Autor = Guid.Parse("00000000-0000-0000-0000-000000000007");

        /// <summary>
        /// Rol de Viewer - Solo lectura
        /// </summary>
        public static readonly Guid Viewer = Guid.Parse("00000000-0000-0000-0000-000000000008");
    }
}

