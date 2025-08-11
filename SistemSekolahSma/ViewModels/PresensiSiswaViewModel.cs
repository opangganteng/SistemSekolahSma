using System;
using System.Collections.Generic;

namespace SistemSekolahSMA.ViewModels
{
    public class PresensiSiswaViewModel
    {
        public int JadwalId { get; set; }
        public string NamaKelas { get; set; }
        public string NamaMapel { get; set; }
        public string NamaGuru { get; set; }
        public DateTime TanggalPresensi { get; set; }
        public List<SiswaPresensiItem> DaftarSiswa { get; set; } = new List<SiswaPresensiItem>();
    }
}