namespace SistemSekolahSMA.ViewModels
{
    public class StatistikPresensi
    {
        public int TotalHadirSiswa { get; set; }
        public int TotalIzinSiswa { get; set; }
        public int TotalSakitSiswa { get; set; }
        public int TotalAlphaSiswa { get; set; }
        public int TotalHadirGuru { get; set; }
        public int TotalIzinGuru { get; set; }
        public int TotalSakitGuru { get; set; }
        public int TotalAlphaGuru { get; set; }

        public double PersentaseKehadiranSiswa
        {
            get
            {
                var total = TotalHadirSiswa + TotalIzinSiswa + TotalSakitSiswa + TotalAlphaSiswa;
                return total > 0 ? (double)TotalHadirSiswa / total * 100 : 0;
            }
        }

        public double PersentaseKehadiranGuru
        {
            get
            {
                var total = TotalHadirGuru + TotalIzinGuru + TotalSakitGuru + TotalAlphaGuru;
                return total > 0 ? (double)TotalHadirGuru / total * 100 : 0;
            }
        }
    }
}