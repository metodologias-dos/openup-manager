﻿using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IPhaseService
    {
        Task<PhaseServiceResult> CreatePhaseAsync(int projectId, string name, int orderIndex, string status = "PENDING", 
            string? objective = null, string? scope = null, string? observations = null, CancellationToken ct = default);
        Task<PhaseServiceResult> GetPhaseByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Phase>> GetPhasesByProjectIdAsync(int projectId, CancellationToken ct = default);
        Task<PhaseServiceResult> UpdatePhaseAsync(int id, string name, DateTime? startDate, DateTime? endDate, string status, 
            int? orderIndex = null, string? objective = null, string? scope = null, string? observations = null, CancellationToken ct = default);
        Task<PhaseServiceResult> SetPhaseStatusAsync(int id, string status, CancellationToken ct = default);
        Task<PhaseServiceResult> UpdatePhaseObjectiveAsync(int id, string? objective, CancellationToken ct = default);
        Task<PhaseServiceResult> UpdatePhaseScopeAsync(int id, string? scope, CancellationToken ct = default);
        Task<PhaseServiceResult> UpdatePhaseObservationsAsync(int id, string? observations, CancellationToken ct = default);
        Task<PhaseServiceResult> DeletePhaseAsync(int id, CancellationToken ct = default);
        Task<PhaseServiceResult> StartPhaseAsync(int phaseId, int projectId, CancellationToken ct = default);
        Task<PhaseServiceResult> EndPhaseAsync(int phaseId, CancellationToken ct = default);
    }

    public record PhaseServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        Phase? Phase = null
    );
}

