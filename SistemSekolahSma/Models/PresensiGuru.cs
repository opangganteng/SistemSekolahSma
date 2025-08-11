using System;
using System.ComponentModel.DataAnnotations;

namespace SistemSekolahSMA.Models
{
    public class PresensiGuru
    {
        public int PresensiGuruId { get; set; }

        [Required(ErrorMessage = "Jadwal harus dipilih")]
        public int JadwalId { get; set; }

        public int GuruId { get; set; }

        [Required(ErrorMessage = "Tanggal presensi harus diisi")]
        [DataType(DataType.Date)]
        public DateTime TanggalPresensi { get; set; }

        [Required(ErrorMessage = "Status kehadiran harus dipilih")]
        public string StatusKehadiran { get; set; }

        public string MateriPelajaran { get; set; }

        public string Keterangan { get; set; }

        public DateTime CreatedDate { get; set; }

        // Navigation properties - TIDAK REQUIRED untuk form binding
        public Jadwal Jadwal { get; set; }
        public Guru Guru { get; set; }
    }
}