using SistemSekolahSMA.Models;

namespace SistemSekolahSMA.Data.Repositories
{
    public interface ISiswaRepository
    {
        Task<Siswa> GetByIdAsync(int siswaId);
        Task<IEnumerable<Siswa>> GetAllAsync();
        Task<IEnumerable<Siswa>> GetByKelasIdAsync(int kelasId);
        Task<IEnumerable<Siswa>> SearchByNameAsync(string name);
        Task<int> CreateAsync(Siswa siswa);
        Task<bool> UpdateAsync(Siswa siswa);
        Task<bool> DeleteAsync(int siswaId);
        Task<bool> NISNExistsAsync(string nisn);
    }
}