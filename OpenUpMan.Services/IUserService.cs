using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IUserService
    {
        Task<(bool Success, string Message, User? User)> CreateUserAsync(string username, string password, CancellationToken ct = default);
        Task<(bool Success, string Message, User? User)> AuthenticateAsync(string username, string password, CancellationToken ct = default);
    }
}

