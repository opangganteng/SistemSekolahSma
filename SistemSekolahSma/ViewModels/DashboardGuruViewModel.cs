using SistemSekolahSMA.Models;
using System.Collections.Generic;

namespace SistemSekolahSMA.ViewModels
{
    public class DashboardGuruViewModel
    {
        public Guru DataGuru { get; set; }
        public List<JadwalMengajar> JadwalMengajar { get; set; } = new List<JadwalMengajar>();
        public List<PresensiHariIni> PresensiHariIni { get; set; } = new List<PresensiHariIni>();
    }
}