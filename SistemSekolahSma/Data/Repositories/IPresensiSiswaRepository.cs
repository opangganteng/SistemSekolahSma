using SistemSekolahSMA.Models;

namespace SistemSekolahSMA.Data.Repositories
{
    public interface IPresensiSiswaRepository
    {
        Task<IEnumerable<PresensiSiswa>> GetByJadwalAndDateAsync(int jadwalId, DateTime tanggal);
        Task<int> CreateAsync(PresensiSiswa presensi);
        Task<bool> UpdateAsync(PresensiSiswa presensi);
        Task<bool> CreateOrUpdateAsync(PresensiSiswa presensi);
        Task<IEnumerable<dynamic>> GetLaporanByMonthAsync(int bulan, int tahun);
    }
}