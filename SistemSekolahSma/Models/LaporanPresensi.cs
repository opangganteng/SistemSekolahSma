using System;

namespace SistemSekolahSMA.Models
{
    public class LaporanPresensi
    {
        public int LaporanId { get; set; }
        public string JenisLaporan { get; set; }
        public int PeriodeBulan { get; set; }
        public int PeriodeTahun { get; set; }
        public int? GuruId { get; set; }
        public DateTime TanggalKirim { get; set; }
        public string StatusLaporan { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public Guru Guru { get; set; }
    }
}