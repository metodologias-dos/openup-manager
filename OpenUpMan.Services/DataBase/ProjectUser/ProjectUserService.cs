using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class ProjectUserService : IProjectUserService
    {
        private readonly IProjectUserRepository _repo;
        private readonly IProjectRepository _projectRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<ProjectUserService> _logger;

        public ProjectUserService(
            IProjectUserRepository repo,
            IProjectRepository projectRepo,
            IUserRepository userRepo,
            ILogger<ProjectUserService> logger)
        {
            _repo = repo;
            _projectRepo = projectRepo;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<ProjectUserServiceResult> AddUserToProjectAsync(Guid projectId, Guid userId, ProjectUserRole role = ProjectUserRole.VIEWER, CancellationToken ct = default)
        {
            try
            {
                var existing = await _repo.GetByIdAsync(projectId, userId, ct);
                if (existing != null)
                {
                    return new ProjectUserServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El usuario ya está asignado al proyecto."
                    );
                }

                var projectUser = new ProjectUser(projectId, userId, role);
                await _repo.AddAsync(projectUser, ct);
                await _repo.SaveChangesAsync(ct);

                _logger.LogInformation("Usuario {UserId} agregado al proyecto {ProjectId} con rol {Role}", userId, projectId, role);

                return new ProjectUserServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Usuario agregado al proyecto exitosamente.",
                    ProjectUser: projectUser
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar usuario al proyecto");
                return new ProjectUserServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al agregar usuario al proyecto."
                );
            }
        }

        public async Task<ProjectUserServiceResult> RemoveUserFromProjectAsync(Guid projectId, Guid userId, CancellationToken ct = default)
        {
            try
            {
                var projectUser = await _repo.GetByIdAsync(projectId, userId, ct);
                if (projectUser == null)
                {
                    return new ProjectUserServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El usuario no está asignado al proyecto."
                    );
                }

                await _repo.RemoveAsync(projectUser, ct);
                await _repo.SaveChangesAsync(ct);

                return new ProjectUserServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Usuario removido del proyecto exitosamente."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover usuario del proyecto");
                return new ProjectUserServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al remover usuario del proyecto."
                );
            }
        }

        public async Task<ProjectUserServiceResult> ChangeUserRoleAsync(Guid projectId, Guid userId, ProjectUserRole newRole, CancellationToken ct = default)
        {
            try
            {
                var projectUser = await _repo.GetByIdAsync(projectId, userId, ct);
                if (projectUser == null)
                {
                    return new ProjectUserServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El usuario no está asignado al proyecto."
                    );
                }

                projectUser.SetRole(newRole);
                await _repo.SaveChangesAsync(ct);

                return new ProjectUserServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Rol del usuario actualizado exitosamente.",
                    ProjectUser: projectUser
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar rol del usuario");
                return new ProjectUserServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al cambiar el rol del usuario."
                );
            }
        }

        public async Task<IEnumerable<ProjectUser>> GetProjectUsersAsync(Guid projectId, CancellationToken ct = default)
        {
            return await _repo.GetByProjectIdAsync(projectId, ct);
        }

        public async Task<IEnumerable<ProjectUser>> GetUserProjectsAsync(Guid userId, CancellationToken ct = default)
        {
            return await _repo.GetByUserIdAsync(userId, ct);
        }
    }
}

