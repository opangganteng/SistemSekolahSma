using SistemSekolahSMA.Data.Repositories;
using SistemSekolahSMA.Models;
using System.Security.Cryptography;
using System.Text;

namespace SistemSekolahSMA.Services
{
    // ===== INTERFACE =====
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<int> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> UsernameExistsAsync(string username);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }

    // ===== IMPLEMENTASI =====
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                var users = await GetAllUsersAsync();
                return users.FirstOrDefault(u => u.Username == username);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetUserByUsernameAsync error: {ex.Message}");
                return null;
            }
        }

        public async Task<int> CreateUserAsync(User user)
        {
            user.Password = HashPassword(user.Password);
            user.CreatedDate = DateTime.Now;
            user.IsActive = true;
            return await _userRepository.CreateAsync(user);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            if (!string.IsNullOrEmpty(user.Password))
            {
                user.Password = HashPassword(user.Password);
            }
            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            return await _userRepository.DeleteAsync(userId);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _userRepository.UsernameExistsAsync(username);
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
    }
}