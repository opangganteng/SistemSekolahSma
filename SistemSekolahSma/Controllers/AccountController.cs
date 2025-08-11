using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemSekolahSMA.Services;
using SistemSekolahSMA.ViewModels;
using System.Security.Claims;

namespace SistemSekolahSMA.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string role = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel();
            if (!string.IsNullOrEmpty(role))
            {
                model.RoleType = role;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _authService.LoginAsync(model);
                if (user == null)
                {
                    ModelState.AddModelError("", "Username atau password salah!");
                    return View(model);
                }

                // Validasi role yang dipilih dengan role di database
                if (user.Role != model.RoleType)
                {
                    ModelState.AddModelError("RoleType", $"User ini bukan {model.RoleType}. Role sebenarnya: {user.Role}");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.UserId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirect berdasarkan role
                if (user.Role == "Admin")
                {
                    TempData["Success"] = "Login berhasil sebagai Admin";
                    return RedirectToAction("Index", "Admin");
                }
                else if (user.Role == "Guru")
                {
                    TempData["Success"] = "Login berhasil sebagai Guru";
                    return RedirectToAction("Index", "Guru");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Terjadi kesalahan sistem: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Logout berhasil";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Quick login methods untuk testing
        [HttpGet]
        public IActionResult QuickLoginAdmin()
        {
            return RedirectToAction("Login", new { role = "Admin" });
        }

        [HttpGet]
        public IActionResult QuickLoginGuru()
        {
            return RedirectToAction("Login", new { role = "Guru" });
        }

        [HttpPost]
        public async Task<IActionResult> TestLogin()
        {
            var user = await _authService.LoginAsync(new LoginViewModel
            {
                Username = "admin1",
                Password = "admin123",
                RoleType = "Admin"
            });

            return Json(new
            {
                Success = user != null,
                UserFound = user?.Username,
                Role = user?.Role
            });
        }

        [HttpGet]
        public async Task<IActionResult> DebugLogin(string username = "admin1", string password = "admin123")
        {
            try
            {
                // 1. Test pencarian user berdasarkan username melalui authService
                var userByUsername = await _authService.GetCurrentUserAsync(username);

                // 2. Test hash password
                string hashedPassword = "";
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    hashedPassword = Convert.ToBase64String(hashedBytes);
                }

                // 3. Test manual login
                var loginResult = await _authService.LoginAsync(new LoginViewModel
                {
                    Username = username,
                    Password = password,
                    RoleType = "Admin" // Default untuk testing
                });

                return Json(new
                {
                    Debug = new
                    {
                        InputUsername = username,
                        InputPassword = password,
                        GeneratedHash = hashedPassword
                    },

                    UserSearch = new
                    {
                        Found = userByUsername != null,
                        UserId = userByUsername?.UserId,
                        Username = userByUsername?.Username,
                        Email = userByUsername?.Email,
                        Role = userByUsername?.Role,
                        IsActive = userByUsername?.IsActive,
                        DatabasePassword = userByUsername?.Password,
                        PasswordMatch = userByUsername?.Password == hashedPassword
                    },

                    LoginTest = new
                    {
                        Success = loginResult != null,
                        ResultUsername = loginResult?.Username,
                        ResultRole = loginResult?.Role,
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }
    }
}