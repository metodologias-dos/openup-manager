namespace OpenUpMan.Services
{
    public record ServiceResult<T>(
        bool Success,
        ServiceResultType ResultType,
        string Message,
        T? Data = default
    );
}

