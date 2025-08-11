using SistemSekolahSMA.Models;
using System.Collections.Generic;

namespace SistemSekolahSMA.ViewModels
{
    public class UserManagementViewModel
    {
        public List<User> DaftarUser { get; set; } = new List<User>();
        public User UserBaru { get; set; } = new User();
    }
}