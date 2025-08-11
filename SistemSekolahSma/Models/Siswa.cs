using System;
using System.ComponentModel.DataAnnotations;

namespace SistemSekolahSMA.Models
{
    public class Siswa
    {
        public int SiswaId { get; set; }

        [Required(ErrorMessage = "NISN harus diisi")]
        [StringLength(20, ErrorMessage = "NISN maksimal 20 karakter")]
        public string NISN { get; set; }

        [Required(ErrorMessage = "Nama siswa harus diisi")]
        [StringLength(100, ErrorMessage = "Nama maksimal 100 karakter")]
        public string NamaSiswa { get; set; }

        [Required(ErrorMessage = "Kelas harus dipilih")]
        public int KelasId { get; set; }

        public string TempatLahir { get; set; }

        [DataType(DataType.Date)]
        public DateTime? TanggalLahir { get; set; }

        public string JenisKelamin { get; set; }
        public string Alamat { get; set; }
        public string NoTelepon { get; set; }
        public string NamaOrtu { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public Kelas Kelas { get; set; }
    }
}