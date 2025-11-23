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

        public async Task<ServiceResult> CreateUserAsync(string username, string password, CancellationToken ct = default)
        {
            try
            {
                // simple validation
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return new ServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Nombre de usuario o contraseña inválidos.",
                        User: null
                    );
                }

                var existing = await _repo.GetByUsernameAsync(username, ct);
                if (existing != null)
                {
                    // Obscure the reason: don't say "username already exists" specifically
                    return new ServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "No se pudo crear el usuario. Por favor, revise los datos.",
                        User: null
                    );
                }

                var hash = BCrypt.Net.BCrypt.HashPassword(password);
                var user = new User(username, hash);
                await _repo.AddAsync(user, ct);
                await _repo.SaveChangesAsync(ct);
                
                return new ServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Usuario creado exitosamente",
                    User: user
                );
            }
            catch (Exception)
            {
                // Log exception here if you have logging
                return new ServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al crear el usuario. Por favor, inténtelo de nuevo.",
                    User: null
                );
            }
        }

        public async Task<ServiceResult> AuthenticateAsync(string username, string password, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return new ServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Credenciales inválidas.",
                        User: null
                    );
                }

                var user = await _repo.GetByUsernameAsync(username, ct);
                if (user == null)
                {
                    return new ServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Credenciales inválidas.",
                        User: null
                    );
                }

                var ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                if (!ok)
                {
                    return new ServiceResult(
                        Success: false,
                        ResultType: ServiceResultType.Error,
                        Message: "Credenciales inválidas.",
                        User: null
                    );
                }

                return new ServiceResult(
                    Success: true,
                    ResultType: ServiceResultType.Success,
                    Message: "Autenticación exitosa",
                    User: user
                );
            }
            catch (Exception)
            {
                // Log exception here if you have logging
                return new ServiceResult(
                    Success: false,
                    ResultType: ServiceResultType.Error,
                    Message: "Error al autenticar. Por favor, inténtelo de nuevo.",
                    User: null
                );
            }
        }
    }
}
