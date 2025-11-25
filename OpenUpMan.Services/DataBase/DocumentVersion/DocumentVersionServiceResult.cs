using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public record DocumentVersionServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        DocumentVersion? DocumentVersion = null
    );
}

