using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repo;
        private readonly IPhaseRepository _phaseRepo;
        private readonly IArtifactRepository _artifactRepo;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(
            IProjectRepository repo,
            IPhaseRepository phaseRepo,
            IArtifactRepository artifactRepo,
            ILogger<ProjectService> logger)
        {
            _repo = repo;
            _phaseRepo = phaseRepo;
            _artifactRepo = artifactRepo;
            _logger = logger;
        }

        public async Task<ProjectServiceResult> CreateProjectAsync(string name, int? createdBy, string? code = null, string? description = null, DateTime? startDate = null, bool createPhases = true, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ProjectServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El nombre es requerido."
                    );
                }

                // Autogenerar código del proyecto si no se proporciona
                if (string.IsNullOrWhiteSpace(code))
                {
                    code = await GenerateProjectCodeAsync(ct);
                }

                var project = new Project(name, createdBy, code, description, startDate);
                await _repo.AddAsync(project, ct);
                await _repo.SaveChangesAsync(ct); // Save to get the auto-generated ID

                _logger.LogInformation("Proyecto creado: {ProjectId} - {Name} (Código: {Code})", project.Id, name, project.Code);

                // Auto-create the 4 OpenUP phases
                if (createPhases)
                {
                    var openUpPhases = new[]
                    {
                        ("Inicio (Inception)", 1),
                        ("Elaboración (Elaboration)", 2),
                        ("Construcción (Construction)", 3),
                        ("Transición (Transition)", 4)
                    };

                    foreach (var (phaseName, order) in openUpPhases)
                    {
                        var phase = new Phase(project.Id, phaseName, order);
                        await _phaseRepo.AddAsync(phase, ct);
                        await _repo.SaveChangesAsync(ct); // Save to get Phase Id

                        // Create default artifacts for Inception
                        if (order == 1) // Inception
                        {
                            var artifacts = new[]
                            {
                                new Artifact(project.Id, phase.Id, "Documento de Visión", "Document", true, "Define el alcance y los objetivos del proyecto."),
                                new Artifact(project.Id, phase.Id, "Lista de Stakeholders", "List", true, "Identifica a los interesados en el proyecto."),
                                new Artifact(project.Id, phase.Id, "Lista de Riesgos Iniciales", "List", true, "Enumera los riesgos conocidos al inicio."),
                                new Artifact(project.Id, phase.Id, "Plan de Proyecto (versión inicial)", "Plan", true, "Planificación preliminar de fases e hitos."),
                                new Artifact(project.Id, phase.Id, "Modelo de Casos de Uso de Alto Nivel", "Model", true, "Visión general de los casos de uso principales.")
                            };

                            foreach (var artifact in artifacts)
                            {
                                await _artifactRepo.AddAsync(artifact, ct);
                            }
                        }
                    }

                    _logger.LogInformation("Fases OpenUP creadas automáticamente para proyecto {ProjectId}", project.Id);
                }

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

        public async Task<ProjectServiceResult> GetProjectByIdAsync(int id, CancellationToken ct = default)
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

        public async Task<ProjectServiceResult> GetProjectByCodeAsync(string code, CancellationToken ct = default)
        {
            try
            {
                var projects = await _repo.GetAllAsync(ct);
                var project = projects.FirstOrDefault(p => p.Code == code);

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
                _logger.LogError(ex, "Error al obtener proyecto por código");
                return new ProjectServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener el proyecto."
                );
            }
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync(CancellationToken ct = default)
        {
            return await _repo.GetAllAsync(ct);
        }

        public async Task<ProjectServiceResult> UpdateProjectAsync(int id, string name, string? description, DateTime? startDate, string? code = null, CancellationToken ct = default)
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

                project.UpdateDetails(name, description, startDate, code);
                await _repo.UpdateAsync(project, ct);

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

        public async Task<ProjectServiceResult> SetStatusAsync(int id, string status, CancellationToken ct = default)
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

                project.SetStatus(status);
                await _repo.UpdateAsync(project, ct);

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

        public async Task<ProjectServiceResult> DeleteProjectAsync(int id, CancellationToken ct = default)
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

                _logger.LogInformation("Iniciando borrado del proyecto {ProjectId}", id);

                // Soft delete - sets DeletedAt timestamp
                project.SoftDelete();
                await _repo.UpdateAsync(project, ct);

                _logger.LogInformation("Proyecto {ProjectId} marcado como eliminado (soft delete)", id);

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

        private async Task<string> GenerateProjectCodeAsync(CancellationToken ct = default)
        {
            var currentYear = DateTime.UtcNow.Year;
            var allProjects = await _repo.GetAllAsync(ct);

            // Filtrar proyectos del año actual que coincidan con el patrón PROY-YYYY-XXX
            var projectsThisYear = allProjects
                .Where(p => !string.IsNullOrEmpty(p.Code) && p.Code.StartsWith($"PROY-{currentYear}-"))
                .ToList();

            int nextNumber = 1;
            if (projectsThisYear.Any())
            {
                // Extraer el último número secuencial
                var numbers = projectsThisYear
                    .Select(p =>
                    {
                        var parts = p.Code!.Split('-');
                        if (parts.Length == 3 && int.TryParse(parts[2], out int num))
                            return num;
                        return 0;
                    })
                    .Where(n => n > 0)
                    .ToList();

                if (numbers.Any())
                {
                    nextNumber = numbers.Max() + 1;
                }
            }

            return $"PROY-{currentYear}-{nextNumber:D3}";
        }
    }
}

