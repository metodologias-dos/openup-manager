using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repo;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(IProjectRepository repo, ILogger<ProjectService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ProjectServiceResult> CreateProjectAsync(
            string identifier,
            string name,
            DateTime startDate,
            Guid ownerId,
            string? description = null,
            CancellationToken ct = default)
        {
            try
            {
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ProjectServiceResultType.Error,
                        Message: "El identificador del proyecto no puede estar vacío.",
                        Project: null
                    );
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ProjectServiceResultType.Error,
                        Message: "El nombre del proyecto no puede estar vacío.",
                        Project: null
                    );
                }

                if (ownerId == Guid.Empty)
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ProjectServiceResultType.Error,
                        Message: "El propietario del proyecto no es válido.",
                        Project: null
                    );
                }

                // Verificar si ya existe un proyecto con el mismo identificador
                var existing = await _repo.GetByIdentifierAsync(identifier, ct);
                if (existing != null)
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ProjectServiceResultType.AlreadyExists,
                        Message: "Ya existe un proyecto con ese identificador.",
                        Project: null
                    );
                }

                // Crear el proyecto
                var project = new Project(identifier, name, startDate, ownerId, description);
                await _repo.AddAsync(project, ct);
                await _repo.SaveChangesAsync(ct);

                return new ProjectServiceResult(
                    Success: true,
                    ResultType: ProjectServiceResultType.Success,
                    Message: "Proyecto creado exitosamente.",
                    Project: project
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project with identifier: {Identifier}", identifier);
                return new ProjectServiceResult(
                    Success: false,
                    ResultType: ProjectServiceResultType.Error,
                    Message: "Error al crear el proyecto. Por favor, inténtelo de nuevo.",
                    Project: null
                );
            }
        }

        public async Task<ProjectServiceResult> GetProjectByIdentifierAsync(string identifier, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ProjectServiceResultType.Error,
                        Message: "El identificador del proyecto no puede estar vacío.",
                        Project: null
                    );
                }

                var project = await _repo.GetByIdentifierAsync(identifier, ct);
                if (project == null)
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ProjectServiceResultType.NotFound,
                        Message: "No se encontró el proyecto.",
                        Project: null
                    );
                }

                return new ProjectServiceResult(
                    Success: true,
                    ResultType: ProjectServiceResultType.Success,
                    Message: "Proyecto encontrado.",
                    Project: project
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project with identifier: {Identifier}", identifier);
                return new ProjectServiceResult(
                    Success: false,
                    ResultType: ProjectServiceResultType.Error,
                    Message: "Error al obtener el proyecto. Por favor, inténtelo de nuevo.",
                    Project: null
                );
            }
        }

        public async Task<ProjectServiceResult> GetProjectsByOwnerAsync(Guid ownerId, CancellationToken ct = default)
        {
            try
            {
                if (ownerId == Guid.Empty)
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ProjectServiceResultType.Error,
                        Message: "El propietario no es válido.",
                        Projects: null
                    );
                }

                var projects = await _repo.GetByOwnerAsync(ownerId, ct);

                return new ProjectServiceResult(
                    Success: true,
                    ResultType: ProjectServiceResultType.Success,
                    Message: "Proyectos obtenidos exitosamente.",
                    Projects: projects
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects for owner: {OwnerId}", ownerId);
                return new ProjectServiceResult(
                    Success: false,
                    ResultType: ProjectServiceResultType.Error,
                    Message: "Error al obtener los proyectos. Por favor, inténtelo de nuevo.",
                    Projects: null
                );
            }
        }

        public async Task<ProjectServiceResult> UpdateProjectAsync(
            string identifier,
            string name,
            string? description,
            DateTime startDate,
            CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ProjectServiceResultType.Error,
                        Message: "El identificador del proyecto no puede estar vacío.",
                        Project: null
                    );
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ProjectServiceResultType.Error,
                        Message: "El nombre del proyecto no puede estar vacío.",
                        Project: null
                    );
                }

                var project = await _repo.GetByIdentifierAsync(identifier, ct);
                if (project == null)
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ProjectServiceResultType.NotFound,
                        Message: "No se encontró el proyecto.",
                        Project: null
                    );
                }

                project.UpdateDetails(name, description, startDate);
                await _repo.SaveChangesAsync(ct);

                return new ProjectServiceResult(
                    Success: true,
                    ResultType: ProjectServiceResultType.Success,
                    Message: "Proyecto actualizado exitosamente.",
                    Project: project
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project with identifier: {Identifier}", identifier);
                return new ProjectServiceResult(
                    Success: false,
                    ResultType: ProjectServiceResultType.Error,
                    Message: "Error al actualizar el proyecto. Por favor, inténtelo de nuevo.",
                    Project: null
                );
            }
        }

        public async Task<ProjectServiceResult> ChangeProjectStateAsync(
            string identifier,
            ProjectState newState,
            CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ProjectServiceResultType.Error,
                        Message: "El identificador del proyecto no puede estar vacío.",
                        Project: null
                    );
                }

                var project = await _repo.GetByIdentifierAsync(identifier, ct);
                if (project == null)
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ProjectServiceResultType.NotFound,
                        Message: "No se encontró el proyecto.",
                        Project: null
                    );
                }

                project.SetState(newState);
                await _repo.SaveChangesAsync(ct);

                return new ProjectServiceResult(
                    Success: true,
                    ResultType: ProjectServiceResultType.Success,
                    Message: "Estado del proyecto actualizado exitosamente.",
                    Project: project
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing state for project with identifier: {Identifier}", identifier);
                return new ProjectServiceResult(
                    Success: false,
                    ResultType: ProjectServiceResultType.Error,
                    Message: "Error al cambiar el estado del proyecto. Por favor, inténtelo de nuevo.",
                    Project: null
                );
            }
        }
    }
}

