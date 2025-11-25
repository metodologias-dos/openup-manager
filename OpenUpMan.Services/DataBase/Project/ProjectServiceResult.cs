using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public record ProjectServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        Project? Project = null
    );
}

