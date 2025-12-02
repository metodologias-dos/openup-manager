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
        public const int Admin = 1;

        /// <summary>
        /// Rol de Product Owner - Gestión del producto
        /// </summary>
        public const int ProductOwner = 2;

        /// <summary>
        /// Rol de Scrum Master - Facilitador del equipo
        /// </summary>
        public const int ScrumMaster = 3;

        /// <summary>
        /// Rol de Desarrollador - Desarrollo de software
        /// </summary>
        public const int Desarrollador = 4;

        /// <summary>
        /// Rol de Tester - Pruebas y QA
        /// </summary>
        public const int Tester = 5;

        /// <summary>
        /// Rol de Revisor - Revisión de documentos y código
        /// </summary>
        public const int Revisor = 6;

        /// <summary>
        /// Rol de Autor - Creación de contenido
        /// </summary>
        public const int Autor = 7;

        /// <summary>
        /// Rol de Viewer - Solo lectura
        /// </summary>
        public const int Viewer = 8;
    }
}

