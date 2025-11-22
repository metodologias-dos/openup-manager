using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private readonly AppDbContext _ctx;

        public UserRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _ctx.Users.AddAsync(user, ct);
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            return await _ctx.Users.FirstOrDefaultAsync(u => u.Username == username, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _ctx.SaveChangesAsync(ct);
        }

        public void Dispose()
        {
            _ctx.Dispose();
        }
    }
}
