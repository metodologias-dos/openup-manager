using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public record ProjectServiceResult(
        bool Success,
        ProjectServiceResultType ResultType,
        string Message,
        Project? Project = null,
        IEnumerable<Project>? Projects = null
    );
}

