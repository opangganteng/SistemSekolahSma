using SistemSekolahSMA.Models;
using SistemSekolahSMA.Controllers;
using System.ComponentModel.DataAnnotations;

namespace SistemSekolahSMA.ViewModels
{
    public class DashboardSiswaViewModel
    {
        [Display(Name = "Kata Kunci Pencarian")]
        public string SearchTerm { get; set; }

        [Display(Name = "Jenis Pencarian")]
        public string SearchType { get; set; } = "siswa";

        public IEnumerable<Siswa> HasilPencarianSiswa { get; set; } = new List<Siswa>();
        public IEnumerable<JadwalKelas> HasilPencarianJadwal { get; set; } = new List<JadwalKelas>();

        // Analytics Data
        public DashboardAnalytics Analytics { get; set; } = new DashboardAnalytics();
    }
    public class DashboardAnalytics
    {
        public int TotalSiswa { get; set; }
        public int TotalGuru { get; set; }
        public int TotalKelas { get; set; }
        public decimal KehadiranHariIni { get; set; }

        public List<TrendKehadiranData> TrendKehadiranMingguan { get; set; } = new List<TrendKehadiranData>();
        public List<DistribusiSiswaData> DistribusiSiswa { get; set; } = new List<DistribusiSiswaData>();
        public List<KehadiranMapelData> KehadiranMataPelajaran { get; set; } = new List<KehadiranMapelData>();
        public List<PerbandinganBulananData> PerbandinganBulanan { get; set; } = new List<PerbandinganBulananData>();
        public List<TopKelasData> KelasTopPerformer { get; set; } = new List<TopKelasData>();
        public StatistikTerkiniData StatistikTerkini { get; set; } = new StatistikTerkiniData();
    }

    public class TrendKehadiranData
    {
        public string Day { get; set; }
        public decimal SiswaAttendance { get; set; }
        public decimal GuruAttendance { get; set; }
    }

    public class DistribusiSiswaData
    {
        public string Label { get; set; }
        public int Count { get; set; }
    }

    public class KehadiranMapelData
    {
        public string Subject { get; set; }
        public decimal Percentage { get; set; }
    }

    public class PerbandinganBulananData
    {
        public string Month { get; set; }
        public decimal SiswaAttendance { get; set; }
        public decimal GuruAttendance { get; set; }
    }

    public class TopKelasData
    {
        public string NamaKelas { get; set; }
        public decimal AttendancePercentage { get; set; }
    }

    public class StatistikTerkiniData
    {
        public int SiswaHadirHariIni { get; set; }
        public int SiswaTidakHadir { get; set; }
        public int GuruMengajarHariIni { get; set; }
        public int KelasAktif { get; set; }
        public decimal RataRataKehadiranMingguIni { get; set; }
    }
}