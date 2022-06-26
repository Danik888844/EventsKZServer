using EventsServer.Models;
using Microsoft.EntityFrameworkCore;

namespace EventsServer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUser(Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(e => e.Id == userId);

            if (user != null)
            {
                return user; // Ok
            }

            return null; // Don't finded
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if(user != null)
            {
                return user; // Ok
            }

            return null; // Don't finded

        }

        public async Task<User> AddUser(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Id = SequentialGuidUtils.CreateGuid();
            user.DateOfReg = DateTime.UtcNow;

            var result = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<User?> UpdateUser(User new_user)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(e => e.Id == new_user.Id);

            if (user != null)
            {
                user.FirstName = new_user.FirstName;
                user.MiddleName = new_user.MiddleName;
                user.LastName = new_user.LastName;
                user.Age = new_user.Age;
                user.Gender = new_user.Gender;
                user.Password = BCrypt.Net.BCrypt.HashPassword(new_user.Password);
                user.Roles = new_user.Roles;

                await _context.SaveChangesAsync();

                return user; // Ok
            }

            return null; // Don't finded
        }

        public async Task<User?> DeleteUser(Guid userId)
        {
            var result = await _context.Users
                .FirstOrDefaultAsync(e => e.Id == userId);
            if (result != null)
            {
                _context.Users.Remove(result);
                await _context.SaveChangesAsync();
                return result; // Ok
            }

            return null; // Don't finded
        }

        public async Task<User?> SetUserRole(Guid userId, Role[] roles)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(e => e.Id == userId);

            if (user != null)
            {
                user.Roles = roles;

                await _context.SaveChangesAsync();

                return user; // Ok
            }

            return null; // Don't finded
        }
    }
}
