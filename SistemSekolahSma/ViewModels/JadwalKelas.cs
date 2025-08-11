using System;

namespace SistemSekolahSMA.ViewModels
{
    public class JadwalKelas
    {
        public int JadwalId { get; set; }
        public string NamaKelas { get; set; }
        public string Hari { get; set; }
        public TimeSpan JamMulai { get; set; }
        public TimeSpan JamSelesai { get; set; }
        public string NamaMapel { get; set; }
        public string NamaGuru { get; set; }
        public string Ruangan { get; set; }

        // Property untuk display format yang user-friendly
        public string JamMulaiFormatted => JamMulai.ToString(@"hh\:mm");
        public string JamSelesaiFormatted => JamSelesai.ToString(@"hh\:mm");
        public string JadwalLengkap => $"{Hari}, {JamMulaiFormatted}-{JamSelesaiFormatted}";
    }
}