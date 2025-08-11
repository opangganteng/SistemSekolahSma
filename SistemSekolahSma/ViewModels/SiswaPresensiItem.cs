using System.ComponentModel.DataAnnotations;

namespace SistemSekolahSMA.ViewModels
{
    public class SiswaPresensiItem
    {
        public int SiswaId { get; set; }
        public string NamaSiswa { get; set; }
        public string NISN { get; set; }

        [Required(ErrorMessage = "Status kehadiran harus dipilih")]
        public string StatusKehadiran { get; set; } = "Hadir";

        public string Keterangan { get; set; }

        // Helper property untuk checkbox (jika diperlukan)
        public bool IsHadir { get; set; } = true;
        public bool IsSakit { get; set; } = false;
        public bool IsIzin { get; set; } = false;
        public bool IsAlpha { get; set; } = false;
    }
}