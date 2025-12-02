using Microsoft.EntityFrameworkCore;
using OpenUpMan.Domain;

namespace OpenUpMan.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _ctx;

        public UserRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _ctx.Users.FindAsync(new object[] { id }, ct);
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
    }
}
