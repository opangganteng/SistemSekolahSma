using Microsoft.AspNetCore.Mvc;
using SistemSekolahSMA.Services;
using SistemSekolahSMA.ViewModels;
using SistemSekolahSMA.Models;
using System.Diagnostics;

namespace SistemSekolahSMA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISiswaService _siswaService;
        private readonly IJadwalService _jadwalService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IDashboardService _dashboardService;

        public HomeController(
            ISiswaService siswaService,
            IJadwalService jadwalService,
            IAnalyticsService analyticsService,
            IDashboardService dashboardService)
        {
            _siswaService = siswaService;
            _jadwalService = jadwalService;
            _analyticsService = analyticsService;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                Debug.WriteLine("=== HomeController.Index: Starting ===");

                // Get dashboard statistics and chart data
                var statistics = await _dashboardService.GetDashboardStatisticsAsync();
                var chartData = await _dashboardService.GetDashboardChartDataAsync();

                // Create analytics object for compatibility
                var analytics = new DashboardAnalyticsCompat
                {
                    TotalSiswa = statistics.TotalSiswa,
                    TotalGuru = statistics.TotalGuru,
                    TotalKelas = statistics.TotalKelas,
                    KehadiranHariIni = 94.7m, // Mock data
                    DistribusiSiswa = chartData.DistribusiSiswaPerKelas
                };

                var viewModel = new DashboardViewModel
                {
                    Statistics = statistics,
                    ChartData = chartData,
                    Analytics = analytics,
                    HasilPencarianSiswa = new List<Siswa>(),
                    HasilPencarianJadwal = new List<Jadwal>()
                };

                Debug.WriteLine($"✅ Dashboard loaded: Siswa={statistics.TotalSiswa}, Guru={statistics.TotalGuru}, Kelas={statistics.TotalKelas}");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ HomeController.Index error: {ex.Message}");
                Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");

                // Return empty dashboard as fallback
                var fallbackViewModel = new DashboardViewModel();
                return View(fallbackViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Search(DashboardViewModel model)
        {
            Debug.WriteLine($"=== SEARCH DEBUG ===");
            Debug.WriteLine($"SearchTerm: '{model.SearchTerm}'");
            Debug.WriteLine($"SearchType: '{model.SearchType}'");

            if (string.IsNullOrEmpty(model.SearchTerm))
            {
                ModelState.AddModelError("SearchTerm", "Kata kunci pencarian harus diisi");

                // Reload dashboard data
                try
                {
                    model.Statistics = await _dashboardService.GetDashboardStatisticsAsync();
                    model.ChartData = await _dashboardService.GetDashboardChartDataAsync();
                    model.Analytics = new DashboardAnalyticsCompat
                    {
                        TotalSiswa = model.Statistics.TotalSiswa,
                        TotalGuru = model.Statistics.TotalGuru,
                        TotalKelas = model.Statistics.TotalKelas,
                        DistribusiSiswa = model.ChartData.DistribusiSiswaPerKelas
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error reloading dashboard: {ex.Message}");
                }

                model.HasilPencarianSiswa = new List<Siswa>();
                model.HasilPencarianJadwal = new List<Jadwal>();
                return View("Index", model);
            }

            try
            {
                // Reload dashboard data first
                model.Statistics = await _dashboardService.GetDashboardStatisticsAsync();
                model.ChartData = await _dashboardService.GetDashboardChartDataAsync();
                model.Analytics = new DashboardAnalyticsCompat
                {
                    TotalSiswa = model.Statistics.TotalSiswa,
                    TotalGuru = model.Statistics.TotalGuru,
                    TotalKelas = model.Statistics.TotalKelas,
                    DistribusiSiswa = model.ChartData.DistribusiSiswaPerKelas
                };

                if (model.SearchType == "siswa")
                {
                    Debug.WriteLine("Processing siswa search...");
                    model.HasilPencarianSiswa = await _dashboardService.SearchSiswaAsync(model.SearchTerm);
                    model.HasilPencarianJadwal = new List<Jadwal>();
                    Debug.WriteLine($"Found {model.HasilPencarianSiswa.Count()} siswa results");
                }
                else if (model.SearchType == "jadwal")
                {
                    Debug.WriteLine("Processing jadwal search...");
                    model.HasilPencarianJadwal = await _dashboardService.SearchJadwalAsync(model.SearchTerm);
                    model.HasilPencarianSiswa = new List<Siswa>();
                    Debug.WriteLine($"Found {model.HasilPencarianJadwal.Count()} jadwal results");
                }
                else
                {
                    Debug.WriteLine($"Unknown search type: {model.SearchType}");
                    model.HasilPencarianSiswa = new List<Siswa>();
                    model.HasilPencarianJadwal = new List<Jadwal>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Search error: {ex.Message}");
                TempData["Error"] = "Terjadi kesalahan saat melakukan pencarian: " + ex.Message;

                model.HasilPencarianSiswa = new List<Siswa>();
                model.HasilPencarianJadwal = new List<Jadwal>();
            }

            Debug.WriteLine($"=== END SEARCH DEBUG ===");
            return View("Index", model);
        }

        // API endpoint untuk data chart real-time
        [HttpGet]
        public async Task<IActionResult> GetChartData()
        {
            try
            {
                var chartData = await _dashboardService.GetDashboardChartDataAsync();

                return Json(new
                {
                    success = true,
                    distribusiKelas = chartData.DistribusiSiswaPerKelas,
                    distribusiTingkat = chartData.DistribusiSiswaPerTingkat,
                    distribusiGender = chartData.DistribusiSiswaPerGender
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ GetChartData error: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        // API endpoint untuk statistik dashboard
        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var statistics = await _dashboardService.GetDashboardStatisticsAsync();

                return Json(new
                {
                    success = true,
                    totalSiswa = statistics.TotalSiswa,
                    siswaAktif = statistics.SiswaAktif,
                    totalGuru = statistics.TotalGuru,
                    totalKelas = statistics.TotalKelas,
                    totalMataPelajaran = statistics.TotalMataPelajaran
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ GetDashboardStats error: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        // TEST ENDPOINT untuk debugging (keep existing)
        [HttpGet]
        public async Task<IActionResult> TestAnalytics()
        {
            try
            {
                Debug.WriteLine("=== TESTING ANALYTICS DATA ===");

                var analytics = await _analyticsService.GetDashboardAnalyticsAsync();

                Debug.WriteLine($"✅ Total Siswa: {analytics.TotalSiswa}");
                Debug.WriteLine($"✅ Total Guru: {analytics.TotalGuru}");
                Debug.WriteLine($"✅ Total Kelas: {analytics.TotalKelas}");

                return Json(new
                {
                    TotalSiswa = analytics.TotalSiswa,
                    TotalGuru = analytics.TotalGuru,
                    TotalKelas = analytics.TotalKelas,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ANALYTICS ERROR: {ex.Message}");
                return Json(new { Error = ex.Message, Success = false });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}