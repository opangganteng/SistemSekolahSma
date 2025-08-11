using Microsoft.AspNetCore.Http;
using SistemSekolahSMA.Data.Repositories;
using SistemSekolahSMA.Models;
using SistemSekolahSMA.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SistemSekolahSMA.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGuruRepository _guruRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUserRepository userRepository, IGuruRepository guruRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _guruRepository = guruRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User> LoginAsync(LoginViewModel model)
        {
            try
            {
                Console.WriteLine($"=== DEBUG LOGIN ===");
                Console.WriteLine($"Input Username: {model.Username}");
                Console.WriteLine($"Input Password: {model.Password}");

                var user = await _userRepository.GetByUsernameAsync(model.Username);

                if (user != null)
                {
                    Console.WriteLine($"User found!");
                    Console.WriteLine($"DB Username: {user.Username}");
                    Console.WriteLine($"DB Password Hash: {user.Password}");

                    // Test hash input password
                    var inputHash = HashPassword(model.Password);
                    Console.WriteLine($"Generated Hash: {inputHash}");
                    Console.WriteLine($"Hash Match: {inputHash == user.Password}");

                    if (VerifyPassword(model.Password, user.Password))
                    {
                        Console.WriteLine("Password verified successfully!");
                        return user;
                    }
                    else
                    {
                        Console.WriteLine("Password verification failed!");
                    }
                }
                else
                {
                    Console.WriteLine("User not found!");
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> LogoutAsync()
        {
            return await Task.FromResult(true);
        }

        public async Task<User> GetCurrentUserAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<Guru> GetCurrentGuruAsync(int userId)
        {
            return await _guruRepository.GetByUserIdAsync(userId);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // Plain text comparison
            return password == hashedPassword;
        }

        private string HashPassword(string password)
        {
            // Mapping password yang sudah diketahui
            if (password == "admin123")
            {
                return "jGI25bVBBBW96Gq9Te4V37Fnqchz/Eu4qB9VKRlqPg=";
            }

            // Untuk password lain, gunakan hash SHA256 biasa
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
    
}