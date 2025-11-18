using medical_record_system_backend.Models;

namespace medical_record_system_backend.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(int id);
        Task<User?> CreateAsync(User user);
        Task UpdateAsync(User user);
    }
}
