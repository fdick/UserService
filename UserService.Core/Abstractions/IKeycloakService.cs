using UserService.Core.Models;

namespace UserService.Core.Abstractions
{
    public interface IKeycloakService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(Guid id);
    }
}