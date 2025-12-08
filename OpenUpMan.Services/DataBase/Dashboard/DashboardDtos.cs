namespace OpenUpMan.Services;

public record DashboardData(
    PhaseStatusDto CurrentPhase,
    List<PhaseStatusDto> AllPhases,
    List<ActiveIterationDto> ActiveIterations,
    ArtifactProgressDto ArtifactProgress,
    ProjectStatisticsDto Statistics
);

public record PhaseStatusDto(
    int PhaseId,
    string Name,
    bool IsCompleted,
    bool IsCurrent,
    bool IsPending,
    int OrderIndex
);

public record ActiveIterationDto(
    int IterationId,
    string Name,
    string Goal,
    DateTime StartDate,
    DateTime EndDate,
    int MicroincrementsCount,
    int DurationDays
);

public record ArtifactProgressDto(
    int MandatoryArtifactsRegistered,
    int TotalMandatoryArtifacts,
    double CompletionPercentage
);

public record ProjectStatisticsDto(
    int TotalIterations,
    int TotalMicroincrements,
    int ActiveIterationsCount,
    int CompletedPhasesCount,
    double AverageMicroincrementsPerIteration,
    double AverageIterationDurationDays
);

