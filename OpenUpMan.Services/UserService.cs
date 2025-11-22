// User service to handle creation and authentication with obscured errors
using BCrypt.Net;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<(bool Success, string Message, User? User)> CreateUserAsync(string username, string password, CancellationToken ct = default)
        {
            // simple validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Nombre de usuario o contraseña inválidos.", null);

            var existing = await _repo.GetByUsernameAsync(username, ct);
            if (existing != null)
            {
                // Obscure the reason: don't say "username already exists" specifically
                return (false, "No se pudo crear el usuario. Por favor, revise los datos.", null);
            }

            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User(username, hash);
            await _repo.AddAsync(user, ct);
            await _repo.SaveChangesAsync(ct);
            return (true, "Usuario creado", user);
        }

        public async Task<(bool Success, string Message, User? User)> AuthenticateAsync(string username, string password, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Credenciales inválidas.", null);

            var user = await _repo.GetByUsernameAsync(username, ct);
            if (user == null)
                return (false, "Credenciales inválidas.", null); // generic message

            var ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!ok)
                return (false, "Credenciales inválidas.", null);

            return (true, "Autenticado", user);
        }
    }
}
