﻿using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public interface IIterationRepository
    {
        Task<Iteration?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Iteration>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Iteration>> GetByPhaseIdAsync(int phaseId, CancellationToken ct = default);
        Task<IEnumerable<Iteration>> GetByProjectIdAsync(int projectId, CancellationToken ct = default);
        Task AddAsync(Iteration iteration, CancellationToken ct = default);
        Task UpdateAsync(Iteration iteration, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<Iteration?> GetActiveIterationByPhaseIdAsync(int phaseId, CancellationToken ct = default);
        Task<Iteration?> GetActiveIterationByProjectIdAsync(int projectId, CancellationToken ct = default);
        
        // Dashboard queries
        Task<IEnumerable<Iteration>> GetActiveIterationsByProjectIdAsync(int projectId, CancellationToken ct = default);
    }
}

