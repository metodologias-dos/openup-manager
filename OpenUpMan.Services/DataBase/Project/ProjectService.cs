using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IProjectUserRepository _projectUserRepo;
        private readonly IProjectPhaseRepository _projectPhaseRepo;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(
            IProjectRepository repo, 
            IUserRepository userRepo,
            IProjectUserRepository projectUserRepo,
            IProjectPhaseRepository projectPhaseRepo,
            ILogger<ProjectService> logger)
        {
            _repo = repo;
            _userRepo = userRepo;
            _projectUserRepo = projectUserRepo;
            _projectPhaseRepo = projectPhaseRepo;
            _logger = logger;
        }

        public async Task<ProjectServiceResult> CreateProjectAsync(string identifier, string name, DateTime startDate, Guid ownerId, string? description = null, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(name))
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Identificador y nombre son requeridos."
                    );
                }

                var existing = await _repo.GetByIdentifierAsync(identifier, ct);
                if (existing != null)
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Ya existe un proyecto con ese identificador."
                    );
                }

                var project = new Project(identifier, name, startDate, ownerId, description);
                await _repo.AddAsync(project, ct);
                await _repo.SaveChangesAsync(ct);

                _logger.LogInformation("Proyecto creado: {ProjectId} - {Identifier}", project.Id, identifier);

                return new ProjectServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Proyecto creado exitosamente.",
                    Project: project
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear proyecto");
                return new ProjectServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear el proyecto."
                );
            }
        }

        public async Task<ProjectServiceResult> GetProjectByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var project = await _repo.GetByIdAsync(id, ct);
                if (project == null)
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Proyecto no encontrado."
                    );
                }

                return new ProjectServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Proyecto encontrado.",
                    Project: project
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyecto");
                return new ProjectServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener el proyecto."
                );
            }
        }

        public async Task<ProjectServiceResult> GetProjectByIdentifierAsync(string identifier, CancellationToken ct = default)
        {
            try
            {
                var project = await _repo.GetByIdentifierAsync(identifier, ct);
                if (project == null)
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Proyecto no encontrado."
                    );
                }

                return new ProjectServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Proyecto encontrado.",
                    Project: project
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyecto");
                return new ProjectServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener el proyecto."
                );
            }
        }

        public async Task<IEnumerable<Project>> GetProjectsByOwnerAsync(Guid ownerId, CancellationToken ct = default)
        {
            return await _repo.GetByOwnerAsync(ownerId, ct);
        }

        public async Task<ProjectServiceResult> UpdateProjectAsync(Guid id, string name, string? description, DateTime startDate, CancellationToken ct = default)
        {
            try
            {
                var project = await _repo.GetByIdAsync(id, ct);
                if (project == null)
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Proyecto no encontrado."
                    );
                }

                project.UpdateDetails(name, description, startDate);
                await _repo.UpdateAsync(project, ct);
                await _repo.SaveChangesAsync(ct);

                return new ProjectServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Proyecto actualizado exitosamente.",
                    Project: project
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar proyecto");
                return new ProjectServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar el proyecto."
                );
            }
        }

        public async Task<ProjectServiceResult> ChangeProjectStateAsync(Guid id, ProjectState newState, CancellationToken ct = default)
        {
            try
            {
                var project = await _repo.GetByIdAsync(id, ct);
                if (project == null)
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Proyecto no encontrado."
                    );
                }

                project.SetState(newState);
                await _repo.UpdateAsync(project, ct);
                await _repo.SaveChangesAsync(ct);

                return new ProjectServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Estado del proyecto actualizado.",
                    Project: project
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado del proyecto");
                return new ProjectServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al cambiar el estado del proyecto."
                );
            }
        }

        public async Task<ProjectServiceResult> DeleteProjectAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var project = await _repo.GetByIdAsync(id, ct);
                if (project == null)
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Proyecto no encontrado."
                    );
                }

                _logger.LogInformation("Iniciando borrado del proyecto {ProjectId} - {Identifier}", id, project.Identifier);

                // 1. Eliminar todos los ProjectUser asociados al proyecto
                var projectUsers = await _projectUserRepo.GetByProjectIdAsync(id, ct);
                foreach (var pu in projectUsers)
                {
                    await _projectUserRepo.RemoveAsync(pu, ct);
                }
                _logger.LogInformation("Eliminados {Count} usuarios del proyecto {ProjectId}", projectUsers.Count(), id);

                // 2. Eliminar todas las fases del proyecto (ProjectPhase)
                var projectPhases = await _projectPhaseRepo.GetByProjectIdAsync(id, ct);
                foreach (var phase in projectPhases)
                {
                    // La eliminación en cascada de PhaseItems y PhaseItemUsers ya está configurada en EF Core
                    // (ver AppDbContext.cs, líneas 116 y 138)
                    await _projectPhaseRepo.DeleteAsync(phase, ct);
                }
                _logger.LogInformation("Eliminadas {Count} fases del proyecto {ProjectId}", projectPhases.Count(), id);

                // 3. Eliminar el proyecto
                await _repo.DeleteAsync(project, ct);
                
                // 4. Guardar todos los cambios en la base de datos
                await _repo.SaveChangesAsync(ct);

                _logger.LogInformation("Proyecto {ProjectId} - {Identifier} eliminado exitosamente", id, project.Identifier);

                return new ProjectServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Proyecto eliminado exitosamente."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar proyecto {ProjectId}", id);
                return new ProjectServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: $"Error al eliminar el proyecto: {ex.Message}"
                );
            }
        }
    }
}

