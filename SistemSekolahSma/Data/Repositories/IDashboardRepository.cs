using SistemSekolahSMA.Models;

namespace SistemSekolahSMA.Data.Repositories
{
    public interface IDashboardRepository
    {
        // Dashboard Statistics
        Task<int> GetTotalSiswaAsync();
        Task<int> GetSiswaAktifAsync();
        Task<int> GetTotalGuruAsync();
        Task<int> GetTotalKelasAsync();
        Task<int> GetTotalMataPelajaranAsync();

        // Chart Data
        Task<Dictionary<string, int>> GetDistribusiSiswaPerKelasAsync();
        Task<Dictionary<string, int>> GetDistribusiSiswaPerTingkatAsync();
        Task<Dictionary<string, int>> GetDistribusiSiswaPerGenderAsync();

        // Search Functions
        Task<IEnumerable<Siswa>> SearchSiswaByNameOrNISNAsync(string searchTerm);
        Task<IEnumerable<Jadwal>> SearchJadwalByKelasAsync(string kelasName);
    }
}