using SistemSekolahSMA.Models;
using SistemSekolahSMA.ViewModels;

namespace SistemSekolahSMA.Data.Repositories
{
    public interface IJadwalRepository
    {
        Task<Jadwal> GetByIdAsync(int jadwalId);
        Task<IEnumerable<Jadwal>> GetAllAsync();
        Task<IEnumerable<Jadwal>> GetByGuruIdAsync(int guruId);
        Task<IEnumerable<Jadwal>> GetByKelasIdAsync(int kelasId);
        Task<IEnumerable<JadwalKelas>> SearchJadwalByKelasAsync(string namaKelas);
        Task<int> CreateAsync(Jadwal jadwal);
        Task<bool> UpdateAsync(Jadwal jadwal);
        Task<bool> DeleteAsync(int jadwalId);
    }
}