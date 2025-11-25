using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public record PhaseItemServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        PhaseItem? PhaseItem = null
    );
}

