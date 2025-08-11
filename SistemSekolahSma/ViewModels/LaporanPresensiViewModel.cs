using SistemSekolahSMA.Models;
using System.Collections.Generic;

namespace SistemSekolahSMA.ViewModels
{
    public class LaporanPresensiViewModel
    {
        public int Bulan { get; set; }
        public int Tahun { get; set; }
        public List<PresensiSiswa> DataPresensiSiswa { get; set; } = new List<PresensiSiswa>();
        public List<PresensiGuru> DataPresensiGuru { get; set; } = new List<PresensiGuru>();
        public StatistikPresensi Statistik { get; set; } = new StatistikPresensi();
    }
}