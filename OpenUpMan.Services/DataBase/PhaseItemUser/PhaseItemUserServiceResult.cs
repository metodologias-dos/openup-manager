using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public record PhaseItemUserServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        PhaseItemUser? PhaseItemUser = null
    );
}

