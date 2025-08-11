using SistemSekolahSMA.Models;
using SistemSekolahSMA.ViewModels;

namespace SistemSekolahSMA.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatistics> GetDashboardStatisticsAsync();
        Task<DashboardChartData> GetDashboardChartDataAsync();
        Task<IEnumerable<Siswa>> SearchSiswaAsync(string searchTerm);
        Task<IEnumerable<Jadwal>> SearchJadwalAsync(string kelasName);
    }
}