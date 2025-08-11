using SistemSekolahSMA.Models;
using SistemSekolahSMA.ViewModels;
using System.Threading.Tasks;

namespace SistemSekolahSMA.Services
{
    public interface IAuthService
    {
        Task<User> LoginAsync(LoginViewModel model);
        Task<bool> LogoutAsync();
        Task<User> GetCurrentUserAsync(string username);
        Task<Guru> GetCurrentGuruAsync(int userId);
    }
}