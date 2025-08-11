using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemSekolahSMA.Repositories
{
    public interface IAnalyticsRepository
    {
        // Basic Analytics Methods (Match dengan existing interface)
        Task<dynamic> GetDashboardAnalyticsAsync();
        Task<List<dynamic>> GetTrendKehadiranMingguanAsync();
        Task<List<dynamic>> GetDistribusiSiswaAsync();
        Task<List<dynamic>> GetKehadiranMataPelajaranAsync();
        Task<List<dynamic>> GetPerbandinganBulananAsync();
        Task<List<dynamic>> GetKelasTopPerformerAsync();
        Task<List<dynamic>> SearchJadwalByKelasAsync(string kelas);

        // Additional Methods (NEW - for real data)
        Task<int> GetTotalSiswaAsync();
        Task<int> GetTotalGuruAsync();
        Task<int> GetTotalKelasAsync();
        Task<decimal> GetKehadiranHariIniAsync();
        Task<List<dynamic>> GetTrendKehadiranMingguan();
        Task<List<dynamic>> GetDistribusiSiswaPerTingkat();
        Task<List<dynamic>> GetKehadiranPerMataPelajaran();
        Task<List<dynamic>> GetPerbandinganBulanan();
        Task<List<dynamic>> GetStatistikKehadiranGuru();
        Task<List<dynamic>> GetRankingMataPelajaran();
        Task<List<dynamic>> GetTrendBulananSiswa();
    }
}