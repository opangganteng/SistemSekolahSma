using SistemSekolahSMA.Models;

namespace SistemSekolahSMA.Data.Repositories
{
    public interface IPresensiGuruRepository
    {
        Task<IEnumerable<PresensiGuru>> GetByGuruIdAndDateAsync(int guruId, DateTime tanggal);
        Task<int> CreateAsync(PresensiGuru presensi);
        Task<bool> UpdateAsync(PresensiGuru presensi);
        Task<bool> CreateOrUpdateAsync(PresensiGuru presensi);
        Task<IEnumerable<dynamic>> GetLaporanByMonthAsync(int bulan, int tahun);
    }
}