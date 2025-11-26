using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public record ServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        User? User = null
    );
}

