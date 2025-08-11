using System;
using System.ComponentModel.DataAnnotations;

namespace SistemSekolahSMA.Models
{
    public class Jadwal
    {
        public int JadwalId { get; set; }

        [Required(ErrorMessage = "Kelas harus dipilih")]
        public int KelasId { get; set; }

        [Required(ErrorMessage = "Mata pelajaran harus dipilih")]
        public int MataPelajaranId { get; set; }

        [Required(ErrorMessage = "Guru harus dipilih")]
        public int GuruId { get; set; }

        [Required(ErrorMessage = "Hari harus dipilih")]
        public string Hari { get; set; }

        [Required(ErrorMessage = "Jam mulai harus diisi")]
        public TimeSpan JamMulai { get; set; }

        [Required(ErrorMessage = "Jam selesai harus diisi")]
        public TimeSpan JamSelesai { get; set; }

        public string Ruangan { get; set; }

        [Required(ErrorMessage = "Tahun ajaran harus diisi")]
        public string TahunAjaran { get; set; }

        [Required(ErrorMessage = "Semester harus dipilih")]
        public int Semester { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public Kelas Kelas { get; set; }
        public MataPelajaran MataPelajaran { get; set; }
        public Guru Guru { get; set; }
    }
}