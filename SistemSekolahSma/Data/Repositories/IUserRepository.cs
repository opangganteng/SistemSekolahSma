using SistemSekolahSMA.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemSekolahSMA.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<int> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int userId);
        Task<bool> UsernameExistsAsync(string username);
    }
}