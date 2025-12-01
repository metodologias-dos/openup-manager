using Microsoft.Extensions.Logging;
using OpenUpMan.Data.Repositories;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class ArtefactService : IArtefactService
    {
        private readonly IArtefactRepository _repo;
        private readonly ILogger<ArtefactService> _logger;

        public ArtefactService(IArtefactRepository repo, ILogger<ArtefactService> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResult<Artefact>> CreateArtefactAsync(string name, string? description = null, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ServiceResult<Artefact>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El nombre del artefacto es requerido.",
                        Data: null
                    );
                }

                var existing = await _repo.GetByNameAsync(name);
                if (existing != null)
                {
                    return new ServiceResult<Artefact>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: $"Ya existe un artefacto con el nombre '{name}'.",
                        Data: null
                    );
                }

                var artefact = new Artefact(name, description);
                await _repo.AddAsync(artefact);

                _logger.LogInformation("Artefacto creado exitosamente: {ArtefactName} con ID: {ArtefactId}", name, artefact.Id);

                return new ServiceResult<Artefact>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Artefacto creado exitosamente.",
                    Data: artefact
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el artefacto: {ArtefactName}", name);
                return new ServiceResult<Artefact>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear el artefacto. Por favor, inténtelo de nuevo.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<Artefact>> GetArtefactByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new ServiceResult<Artefact>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de artefacto inválido.",
                        Data: null
                    );
                }

                var artefact = await _repo.GetByIdAsync(id);
                if (artefact == null)
                {
                    return new ServiceResult<Artefact>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Artefacto no encontrado.",
                        Data: null
                    );
                }

                return new ServiceResult<Artefact>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Artefacto encontrado.",
                    Data: artefact
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el artefacto con ID: {ArtefactId}", id);
                return new ServiceResult<Artefact>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener el artefacto.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<Artefact>> GetArtefactByNameAsync(string name, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ServiceResult<Artefact>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Nombre de artefacto inválido.",
                        Data: null
                    );
                }

                var artefact = await _repo.GetByNameAsync(name);
                if (artefact == null)
                {
                    return new ServiceResult<Artefact>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Artefacto no encontrado.",
                        Data: null
                    );
                }

                return new ServiceResult<Artefact>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Artefacto encontrado.",
                    Data: artefact
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el artefacto con nombre: {ArtefactName}", name);
                return new ServiceResult<Artefact>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener el artefacto.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<IEnumerable<Artefact>>> GetAllArtefactsAsync(CancellationToken ct = default)
        {
            try
            {
                var artefacts = await _repo.GetAllAsync();
                var artefactsList = artefacts.ToList();
                return new ServiceResult<IEnumerable<Artefact>>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: $"Se encontraron {artefactsList.Count} artefactos.",
                    Data: artefactsList
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los artefactos");
                return new ServiceResult<IEnumerable<Artefact>>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener los artefactos.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<Artefact>> UpdateArtefactAsync(Guid id, string name, string? description = null, CancellationToken ct = default)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new ServiceResult<Artefact>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de artefacto inválido.",
                        Data: null
                    );
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    return new ServiceResult<Artefact>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "El nombre del artefacto es requerido.",
                        Data: null
                    );
                }

                var artefact = await _repo.GetByIdAsync(id);
                if (artefact == null)
                {
                    return new ServiceResult<Artefact>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Artefacto no encontrado.",
                        Data: null
                    );
                }

                artefact.UpdateDetails(name, description);
                await _repo.UpdateAsync(artefact);

                _logger.LogInformation("Artefacto actualizado exitosamente: {ArtefactId}", id);

                return new ServiceResult<Artefact>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Artefacto actualizado exitosamente.",
                    Data: artefact
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el artefacto: {ArtefactId}", id);
                return new ServiceResult<Artefact>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar el artefacto.",
                    Data: null
                );
            }
        }

        public async Task<ServiceResult<bool>> DeleteArtefactAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new ServiceResult<bool>(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "ID de artefacto inválido.",
                        Data: false
                    );
                }

                var exists = await _repo.ExistsAsync(id);
                if (!exists)
                {
                    return new ServiceResult<bool>(
                        Success: false,
                        ResultType: ServiceResultType.NotFound,
                        Message: "Artefacto no encontrado.",
                        Data: false
                    );
                }

                await _repo.DeleteAsync(id);

                _logger.LogInformation("Artefacto eliminado exitosamente: {ArtefactId}", id);

                return new ServiceResult<bool>(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Artefacto eliminado exitosamente.",
                    Data: true
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el artefacto: {ArtefactId}", id);
                return new ServiceResult<bool>(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al eliminar el artefacto.",
                    Data: false
                );
            }
        }
    }
}

