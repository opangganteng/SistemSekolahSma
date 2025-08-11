using System;
using System.ComponentModel.DataAnnotations;

namespace SistemSekolahSMA.Models
{
    public class MataPelajaran
    {
        public int MataPelajaranId { get; set; }

        [Required(ErrorMessage = "Kode mata pelajaran harus diisi")]
        [StringLength(10, ErrorMessage = "Kode maksimal 10 karakter")]
        public string KodeMapel { get; set; }

        [Required(ErrorMessage = "Nama mata pelajaran harus diisi")]
        [StringLength(100, ErrorMessage = "Nama maksimal 100 karakter")]
        public string NamaMapel { get; set; }

        public string Kategori { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}