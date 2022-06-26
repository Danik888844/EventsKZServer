using EventsServer.Models;

namespace EventsServer.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User?> GetUser(Guid userId);
        Task<User> AddUser(User user);
        Task<User?> UpdateUser(User user);
        Task<User?> DeleteUser(Guid userId);
        Task<User?> GetUserByEmail(string email);
        Task<User?> SetUserRole(Guid userId, Role[] roles);
    }
}
