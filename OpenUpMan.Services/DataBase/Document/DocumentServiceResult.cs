using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public record DocumentServiceResult(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        Document? Document = null
    );
}

