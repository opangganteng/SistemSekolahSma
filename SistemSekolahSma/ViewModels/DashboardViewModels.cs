using SistemSekolahSMA.Models;

namespace SistemSekolahSMA.ViewModels
{
    public class DashboardStatistics
    {
        public int TotalSiswa { get; set; }
        public int SiswaAktif { get; set; }
        public int TotalGuru { get; set; }
        public int TotalKelas { get; set; }
        public int TotalMataPelajaran { get; set; }
    }

    public class DashboardChartData
    {
        public Dictionary<string, int> DistribusiSiswaPerKelas { get; set; } = new();
        public Dictionary<string, int> DistribusiSiswaPerTingkat { get; set; } = new();
        public Dictionary<string, int> DistribusiSiswaPerGender { get; set; } = new();
    }

    public class DashboardViewModel
    {
        public DashboardStatistics Statistics { get; set; } = new();
        public DashboardChartData ChartData { get; set; } = new();

        // Search properties
        public string SearchTerm { get; set; } = "";
        public string SearchType { get; set; } = "siswa";
        public IEnumerable<Siswa> HasilPencarianSiswa { get; set; } = new List<Siswa>();
        public IEnumerable<Jadwal> HasilPencarianJadwal { get; set; } = new List<Jadwal>();

        // Analytics data (untuk kompatibilitas dengan existing code)
        public DashboardAnalyticsCompat Analytics { get; set; } = new();
    }

    // Renamed to avoid conflict with existing DashboardAnalytics
    public class DashboardAnalyticsCompat
    {
        public int TotalSiswa { get; set; }
        public int TotalGuru { get; set; }
        public int TotalKelas { get; set; }
        public decimal KehadiranHariIni { get; set; } = 94.7m;
        public Dictionary<string, int> DistribusiSiswa { get; set; } = new();
        public List<dynamic> TrendKehadiranMingguan { get; set; } = new();
        public List<dynamic> KehadiranMataPelajaran { get; set; } = new();
        public List<dynamic> PerbandinganBulanan { get; set; } = new();
        public Dictionary<string, object> StatistikTerkini { get; set; } = new();
    }
}