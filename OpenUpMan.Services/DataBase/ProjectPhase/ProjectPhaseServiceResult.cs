using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public record ProjectPhaseServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        ProjectPhase? ProjectPhase = null
    );
}

