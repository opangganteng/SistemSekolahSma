using System;
using System.ComponentModel.DataAnnotations;

namespace SistemSekolahSMA.Models
{
    public class Guru
    {
        public int GuruId { get; set; }

        // TIDAK ADA VALIDATION ATTRIBUTE
        public int UserId { get; set; }

        [Required(ErrorMessage = "NIP harus diisi")]
        [StringLength(255, ErrorMessage = "NIP maksimal 255 karakter")]
        public string NIP { get; set; }

        [Required(ErrorMessage = "Nama guru harus diisi")]
        [StringLength(100, ErrorMessage = "Nama maksimal 100 karakter")]
        public string NamaGuru { get; set; }

        public string TempatLahir { get; set; }

        [DataType(DataType.Date)]
        public DateTime? TanggalLahir { get; set; }

        public string JenisKelamin { get; set; }
        public string Alamat { get; set; }
        public string NoTelepon { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // HAPUS NAVIGATION PROPERTY UNTUK SEMENTARA
        // public User User { get; set; }
    }
}