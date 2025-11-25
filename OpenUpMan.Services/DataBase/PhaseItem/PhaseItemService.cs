using Microsoft.Extensions.Logging;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class PhaseItemService : IPhaseItemService
    {
        private readonly IPhaseItemRepository _repo;
        private readonly IProjectPhaseRepository _phaseRepo;
        private readonly ILogger<PhaseItemService> _logger;

        public PhaseItemService(
            IPhaseItemRepository repo,
            IProjectPhaseRepository phaseRepo,
            ILogger<PhaseItemService> logger)
        {
            _repo = repo;
            _phaseRepo = phaseRepo;
            _logger = logger;
        }

        public async Task<PhaseItemServiceResult> CreateIterationAsync(Guid projectPhaseId, int number, string name, Guid createdBy, string? description = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken ct = default)
        {
            try
            {
                var iteration = new PhaseItem(
                    projectPhaseId,
                    PhaseItemType.ITERATION,
                    number,
                    name,
                    createdBy,
                    null, // no parent iteration
                    description,
                    startDate,
                    endDate
                );

                await _repo.AddAsync(iteration, ct);
                await _repo.SaveChangesAsync(ct);

                _logger.LogInformation("Iteración creada: {ItemId} - {Name}", iteration.Id, name);

                return new PhaseItemServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Iteración creada exitosamente.",
                    PhaseItem: iteration
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear iteración");
                return new PhaseItemServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear la iteración."
                );
            }
        }

        public async Task<PhaseItemServiceResult> CreateMicroincrementAsync(Guid projectPhaseId, Guid parentIterationId, int number, string name, Guid createdBy, string? description = null, DateTime? startDate = null, CancellationToken ct = default)
        {
            try
            {
                var microincrement = new PhaseItem(
                    projectPhaseId,
                    PhaseItemType.MICROINCREMENT,
                    number,
                    name,
                    createdBy,
                    parentIterationId,
                    description,
                    startDate,
                    null // microincrements typically don't have end dates
                );

                await _repo.AddAsync(microincrement, ct);
                await _repo.SaveChangesAsync(ct);

                _logger.LogInformation("Microincremento creado: {ItemId} - {Name}", microincrement.Id, name);

                return new PhaseItemServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Microincremento creado exitosamente.",
                    PhaseItem: microincrement
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear microincremento");
                return new PhaseItemServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear el microincremento."
                );
            }
        }

        public async Task<PhaseItemServiceResult> GetPhaseItemByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var item = await _repo.GetByIdAsync(id, ct);
                if (item == null)
                {
                    return new PhaseItemServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Item no encontrado."
                    );
                }

                return new PhaseItemServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Item encontrado.",
                    PhaseItem: item
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener item");
                return new PhaseItemServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al obtener el item."
                );
            }
        }

        public async Task<IEnumerable<PhaseItem>> GetPhaseItemsByPhaseAsync(Guid projectPhaseId, CancellationToken ct = default)
        {
            return await _repo.GetByPhaseIdAsync(projectPhaseId, ct);
        }

        public async Task<IEnumerable<PhaseItem>> GetIterationsByPhaseAsync(Guid projectPhaseId, CancellationToken ct = default)
        {
            return await _repo.GetIterationsByPhaseIdAsync(projectPhaseId, ct);
        }

        public async Task<IEnumerable<PhaseItem>> GetMicroincrementsByIterationAsync(Guid iterationId, CancellationToken ct = default)
        {
            return await _repo.GetMicroincrementsByIterationIdAsync(iterationId, ct);
        }

        public async Task<PhaseItemServiceResult> UpdatePhaseItemAsync(Guid id, string name, string? description, DateTime? startDate, DateTime? endDate, CancellationToken ct = default)
        {
            try
            {
                var item = await _repo.GetByIdAsync(id, ct);
                if (item == null)
                {
                    return new PhaseItemServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Item no encontrado."
                    );
                }

                item.UpdateDetails(name, description, startDate, endDate);
                await _repo.UpdateAsync(item, ct);
                await _repo.SaveChangesAsync(ct);

                return new PhaseItemServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Item actualizado exitosamente.",
                    PhaseItem: item
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar item");
                return new PhaseItemServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al actualizar el item."
                );
            }
        }

        public async Task<PhaseItemServiceResult> ChangePhaseItemStateAsync(Guid id, PhaseItemState newState, CancellationToken ct = default)
        {
            try
            {
                var item = await _repo.GetByIdAsync(id, ct);
                if (item == null)
                {
                    return new PhaseItemServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Item no encontrado."
                    );
                }

                item.SetState(newState);
                await _repo.UpdateAsync(item, ct);
                await _repo.SaveChangesAsync(ct);

                return new PhaseItemServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Estado del item actualizado.",
                    PhaseItem: item
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado del item");
                return new PhaseItemServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al cambiar el estado del item."
                );
            }
        }
    }
}

