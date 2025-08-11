using System;
using System.ComponentModel.DataAnnotations;

namespace SistemSekolahSMA.Models
{
    public class PresensiSiswa
    {
        public int PresensiSiswaId { get; set; }
        public int JadwalId { get; set; }
        public int SiswaId { get; set; }

        [Required(ErrorMessage = "Tanggal presensi harus diisi")]
        [DataType(DataType.Date)]
        public DateTime TanggalPresensi { get; set; }

        [Required(ErrorMessage = "Status kehadiran harus dipilih")]
        public string StatusKehadiran { get; set; }

        public string Keterangan { get; set; }
        public int DibuatOleh { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public Jadwal Jadwal { get; set; }
        public Siswa Siswa { get; set; }
        public Guru DibuatOlehNavigation { get; set; }
    }
}