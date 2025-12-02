using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public interface IUserService
    {
        Task<ServiceResult> CreateUserAsync(string username, string password, CancellationToken ct = default);
        Task<ServiceResult> AuthenticateAsync(string username, string password, CancellationToken ct = default);
        Task<ServiceResult> GetUserByIdAsync(int userId, CancellationToken ct = default);
    }
}

