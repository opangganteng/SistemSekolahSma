using System;
using System.ComponentModel.DataAnnotations;

namespace SistemSekolahSMA.Models
{
    public class Kelas
    {
        public int KelasId { get; set; }

        [Required(ErrorMessage = "Nama kelas harus diisi")]
        [StringLength(20, ErrorMessage = "Nama kelas maksimal 20 karakter")]
        public string NamaKelas { get; set; }

        [Required(ErrorMessage = "Tingkat harus dipilih")]
        public int Tingkat { get; set; }

        public string Jurusan { get; set; }
        public int? WaliKelas { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public Guru WaliKelasNavigation { get; set; }
    }
}