using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public record ProjectUserServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        ProjectUser? ProjectUser = null
    );
}

