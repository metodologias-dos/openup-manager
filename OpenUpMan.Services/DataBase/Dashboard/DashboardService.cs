using Microsoft.Extensions.Logging;
using OpenUpMan.Data;

namespace OpenUpMan.Services;

public class DashboardService : IDashboardService
{
    private readonly IPhaseRepository _phaseRepo;
    private readonly IIterationRepository _iterationRepo;
    private readonly IMicroincrementRepository _microincrementRepo;
    private readonly IArtifactRepository _artifactRepo;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        IPhaseRepository phaseRepo,
        IIterationRepository iterationRepo,
        IMicroincrementRepository microincrementRepo,
        IArtifactRepository artifactRepo,
        ILogger<DashboardService> logger)
    {
        _phaseRepo = phaseRepo;
        _iterationRepo = iterationRepo;
        _microincrementRepo = microincrementRepo;
        _artifactRepo = artifactRepo;
        _logger = logger;
    }

    public async Task<DashboardData?> GetDashboardDataAsync(int projectId, CancellationToken ct = default)
    {
        try
        {
            // Get all phases for the project
            var phases = (await _phaseRepo.GetByProjectIdAsync(projectId, ct)).ToList();
            if (!phases.Any())
            {
                _logger.LogWarning("No phases found for project {ProjectId}", projectId);
                return null;
            }

            // Determine current phase
            var currentPhase = phases.FirstOrDefault(p => p.Status == "IN_PROGRESS") 
                             ?? phases.FirstOrDefault(p => p.Status == "PENDING")
                             ?? phases.First();

            // Build phase status DTOs
            var phaseStatusDtos = phases.Select(p => new PhaseStatusDto(
                PhaseId: p.Id,
                Name: p.Name,
                IsCompleted: p.Status == "COMPLETED",
                IsCurrent: p.Id == currentPhase.Id,
                IsPending: p.Status == "PENDING",
                OrderIndex: p.OrderIndex ?? 0
            )).OrderBy(p => p.OrderIndex).ToList();

            var currentPhaseDto = phaseStatusDtos.First(p => p.IsCurrent);

            // Get active iterations
            var activeIterations = (await _iterationRepo.GetActiveIterationsByProjectIdAsync(projectId, ct)).ToList();
            
            var activeIterationDtos = new List<ActiveIterationDto>();
            foreach (var iteration in activeIterations)
            {
                var microincrementsCount = await _microincrementRepo.CountByIterationIdAsync(iteration.Id, ct);
                
                var durationDays = 0;
                if (iteration.StartDate.HasValue && iteration.EndDate.HasValue)
                {
                    durationDays = (iteration.EndDate.Value - iteration.StartDate.Value).Days;
                }
                
                activeIterationDtos.Add(new ActiveIterationDto(
                    IterationId: iteration.Id,
                    Name: iteration.Name ?? "Sin nombre",
                    Goal: iteration.Goal ?? "Sin objetivo definido",
                    StartDate: iteration.StartDate ?? DateTime.Now,
                    EndDate: iteration.EndDate ?? DateTime.Now,
                    MicroincrementsCount: microincrementsCount,
                    DurationDays: durationDays
                ));
            }

            // Get artifact progress
            var totalMandatory = await _artifactRepo.CountMandatoryByProjectIdAsync(projectId, ct);
            var mandatoryWithVersions = await _artifactRepo.CountMandatoryWithVersionsByProjectIdAsync(projectId, ct);
            var completionPercentage = totalMandatory > 0 
                ? Math.Round((double)mandatoryWithVersions / totalMandatory * 100, 1)
                : 0.0;

            var artifactProgress = new ArtifactProgressDto(
                MandatoryArtifactsRegistered: mandatoryWithVersions,
                TotalMandatoryArtifacts: totalMandatory,
                CompletionPercentage: completionPercentage
            );

            // Calculate statistics
            var allIterations = (await _iterationRepo.GetByProjectIdAsync(projectId, ct)).ToList();
            var allMicroincrements = (await _microincrementRepo.GetByProjectIdAsync(projectId, ct)).ToList();
            
            var avgMicroincrementsPerIteration = allIterations.Any() 
                ? Math.Round((double)allMicroincrements.Count / allIterations.Count, 1)
                : 0.0;

            var iterationsWithDates = allIterations
                .Where(i => i.StartDate.HasValue && i.EndDate.HasValue)
                .ToList();
            
            var avgIterationDuration = iterationsWithDates.Any()
                ? Math.Round(iterationsWithDates.Average(i => (i.EndDate!.Value - i.StartDate!.Value).TotalDays), 1)
                : 0.0;

            var statistics = new ProjectStatisticsDto(
                TotalIterations: allIterations.Count,
                TotalMicroincrements: allMicroincrements.Count,
                ActiveIterationsCount: activeIterations.Count,
                CompletedPhasesCount: phases.Count(p => p.Status == "COMPLETED"),
                AverageMicroincrementsPerIteration: avgMicroincrementsPerIteration,
                AverageIterationDurationDays: avgIterationDuration
            );

            return new DashboardData(
                CurrentPhase: currentPhaseDto,
                AllPhases: phaseStatusDtos,
                ActiveIterations: activeIterationDtos,
                ArtifactProgress: artifactProgress,
                Statistics: statistics
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data for project {ProjectId}", projectId);
            return null;
        }
    }
}

