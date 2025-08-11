using SistemSekolahSMA.Repositories;
using SistemSekolahSMA.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SistemSekolahSMA.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        public AnalyticsService(IAnalyticsRepository analyticsRepository)
        {
            _analyticsRepository = analyticsRepository;
        }

        public async Task<ViewModels.DashboardAnalytics> GetDashboardAnalyticsAsync()
        {
            var analytics = new ViewModels.DashboardAnalytics();

            try
            {
                Debug.WriteLine("=== AnalyticsService: Starting GetDashboardAnalyticsAsync ===");

                // Get basic counts from database
                analytics.TotalSiswa = await _analyticsRepository.GetTotalSiswaAsync();
                analytics.TotalGuru = await _analyticsRepository.GetTotalGuruAsync();
                analytics.TotalKelas = await _analyticsRepository.GetTotalKelasAsync();
                analytics.KehadiranHariIni = await _analyticsRepository.GetKehadiranHariIniAsync();

                Debug.WriteLine($"✅ Basic counts - Siswa: {analytics.TotalSiswa}, Guru: {analytics.TotalGuru}, Kelas: {analytics.TotalKelas}");
                Debug.WriteLine($"✅ Kehadiran hari ini: {analytics.KehadiranHariIni}%");

                // Get trend data
                var trendData = await _analyticsRepository.GetTrendKehadiranMingguan();
                analytics.TrendKehadiranMingguan = trendData.Select(t => new ViewModels.TrendKehadiranData
                {
                    Day = GetSafeString(t.Day, "N/A"),
                    SiswaAttendance = GetSafeDecimal(t.SiswaAttendance, 0),
                    GuruAttendance = GetSafeDecimal(t.GuruAttendance, 0)
                }).ToList();

                Debug.WriteLine($"✅ Trend kehadiran mingguan: {analytics.TrendKehadiranMingguan.Count} items");

                // Get student distribution
                var distribusiData = await _analyticsRepository.GetDistribusiSiswaPerTingkat();
                analytics.DistribusiSiswa = distribusiData.Select(d => new ViewModels.DistribusiSiswaData
                {
                    Label = GetSafeString(d.Label, "N/A"),
                    Count = GetSafeInt(d.Count, 0)
                }).ToList();

                Debug.WriteLine($"✅ Distribusi siswa: {analytics.DistribusiSiswa.Count} items");

                // Get subject attendance
                var subjectData = await _analyticsRepository.GetKehadiranPerMataPelajaran();
                analytics.KehadiranMataPelajaran = subjectData.Select(s => new ViewModels.KehadiranMapelData
                {
                    Subject = GetSafeString(s.Subject, "N/A"),
                    Percentage = GetSafeDecimal(s.Percentage, 0)
                }).ToList();

                Debug.WriteLine($"✅ Kehadiran mata pelajaran: {analytics.KehadiranMataPelajaran.Count} items");

                // Get monthly comparison
                var monthlyData = await _analyticsRepository.GetPerbandinganBulanan();
                analytics.PerbandinganBulanan = monthlyData.Select(m => new ViewModels.PerbandinganBulananData
                {
                    Month = GetSafeString(m.Month, "N/A"),
                    SiswaAttendance = GetSafeDecimal(m.SiswaAttendance, 0),
                    GuruAttendance = GetSafeDecimal(m.GuruAttendance, 0)
                }).ToList();

                Debug.WriteLine($"✅ Perbandingan bulanan: {analytics.PerbandinganBulanan.Count} items");

                // Get top performing classes (using ranking data)
                var rankingData = await _analyticsRepository.GetRankingMataPelajaran();
                analytics.KelasTopPerformer = rankingData.Select(r => new ViewModels.TopKelasData
                {
                    NamaKelas = GetSafeString(r.NamaMapel, "N/A"), // Use subject name as class representation
                    AttendancePercentage = GetSafeDecimal(r.AttendancePercentage, 0)
                }).ToList();

                Debug.WriteLine($"✅ Kelas top performer: {analytics.KelasTopPerformer.Count} items");

                // Get recent statistics
                var statsData = await _analyticsRepository.GetStatistikKehadiranGuru();
                var statsItem = statsData.FirstOrDefault();

                analytics.StatistikTerkini = new ViewModels.StatistikTerkiniData
                {
                    SiswaHadirHariIni = (int)(analytics.TotalSiswa * (double)(analytics.KehadiranHariIni / 100)),
                    SiswaTidakHadir = analytics.TotalSiswa - (int)(analytics.TotalSiswa * (double)(analytics.KehadiranHariIni / 100)),
                    GuruMengajarHariIni = statsItem != null ? GetSafeInt(statsItem.TotalHadirGuru, 0) : 0,
                    KelasAktif = analytics.TotalKelas,
                    RataRataKehadiranMingguIni = analytics.TrendKehadiranMingguan.Any() ?
                        analytics.TrendKehadiranMingguan.Average(t => t.SiswaAttendance) : 0
                };

                Debug.WriteLine($"✅ Statistik terkini - Siswa hadir: {analytics.StatistikTerkini.SiswaHadirHariIni}");

                Debug.WriteLine("=== AnalyticsService: GetDashboardAnalyticsAsync completed successfully ===");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ AnalyticsService ERROR: {ex.Message}");
                Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");

                // Return empty analytics object with default values to prevent crashes
                analytics = new ViewModels.DashboardAnalytics
                {
                    TotalSiswa = 0,
                    TotalGuru = 0,
                    TotalKelas = 0,
                    KehadiranHariIni = 0,
                    DistribusiSiswa = new List<ViewModels.DistribusiSiswaData>(),
                    TrendKehadiranMingguan = new List<ViewModels.TrendKehadiranData>(),
                    KelasTopPerformer = new List<ViewModels.TopKelasData>(),
                    KehadiranMataPelajaran = new List<ViewModels.KehadiranMapelData>(),
                    PerbandinganBulanan = new List<ViewModels.PerbandinganBulananData>(),
                    StatistikTerkini = new ViewModels.StatistikTerkiniData()
                };
            }

            return analytics;
        }

        // Helper methods to safely convert dynamic properties
        private string GetSafeString(dynamic value, string defaultValue)
        {
            try
            {
                return value?.ToString() ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private int GetSafeInt(dynamic value, int defaultValue)
        {
            try
            {
                if (value == null) return defaultValue;
                if (int.TryParse(value.ToString(), out int result))
                    return result;
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private decimal GetSafeDecimal(dynamic value, decimal defaultValue)
        {
            try
            {
                if (value == null) return defaultValue;
                if (decimal.TryParse(value.ToString(), out decimal result))
                    return result;
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}