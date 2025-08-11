using SistemSekolahSMA.Data.Repositories;
using SistemSekolahSMA.Models;
using SistemSekolahSMA.ViewModels;
using System.Diagnostics;

namespace SistemSekolahSMA.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardStatistics> GetDashboardStatisticsAsync()
        {
            try
            {
                Debug.WriteLine("=== DashboardService.GetDashboardStatisticsAsync: Starting ===");

                var statistics = new DashboardStatistics
                {
                    TotalSiswa = await _dashboardRepository.GetTotalSiswaAsync(),
                    SiswaAktif = await _dashboardRepository.GetSiswaAktifAsync(),
                    TotalGuru = await _dashboardRepository.GetTotalGuruAsync(),
                    TotalKelas = await _dashboardRepository.GetTotalKelasAsync(),
                    TotalMataPelajaran = await _dashboardRepository.GetTotalMataPelajaranAsync()
                };

                Debug.WriteLine($"✅ Statistics loaded: Siswa={statistics.TotalSiswa}, Guru={statistics.TotalGuru}, Kelas={statistics.TotalKelas}");
                return statistics;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ DashboardService.GetDashboardStatisticsAsync error: {ex.Message}");
                throw;
            }
        }

        public async Task<DashboardChartData> GetDashboardChartDataAsync()
        {
            try
            {
                Debug.WriteLine("=== DashboardService.GetDashboardChartDataAsync: Starting ===");

                var chartData = new DashboardChartData
                {
                    DistribusiSiswaPerKelas = await _dashboardRepository.GetDistribusiSiswaPerKelasAsync(),
                    DistribusiSiswaPerTingkat = await _dashboardRepository.GetDistribusiSiswaPerTingkatAsync(),
                    DistribusiSiswaPerGender = await _dashboardRepository.GetDistribusiSiswaPerGenderAsync()
                };

                Debug.WriteLine($"✅ Chart data loaded: {chartData.DistribusiSiswaPerKelas.Count} kelas, {chartData.DistribusiSiswaPerTingkat.Count} tingkat");
                return chartData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ DashboardService.GetDashboardChartDataAsync error: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Siswa>> SearchSiswaAsync(string searchTerm)
        {
            try
            {
                Debug.WriteLine($"=== DashboardService.SearchSiswaAsync: Searching for '{searchTerm}' ===");

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return new List<Siswa>();
                }

                var results = await _dashboardRepository.SearchSiswaByNameOrNISNAsync(searchTerm.Trim());
                Debug.WriteLine($"✅ Found {results.Count()} siswa results");

                return results;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ DashboardService.SearchSiswaAsync error: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Jadwal>> SearchJadwalAsync(string kelasName)
        {
            try
            {
                Debug.WriteLine($"=== DashboardService.SearchJadwalAsync: Searching for '{kelasName}' ===");

                if (string.IsNullOrWhiteSpace(kelasName))
                {
                    return new List<Jadwal>();
                }

                var results = await _dashboardRepository.SearchJadwalByKelasAsync(kelasName.Trim());
                Debug.WriteLine($"✅ Found {results.Count()} jadwal results");

                return results;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ DashboardService.SearchJadwalAsync error: {ex.Message}");
                throw;
            }
        }
    }
}