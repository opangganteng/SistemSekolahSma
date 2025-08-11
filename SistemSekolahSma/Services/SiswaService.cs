using SistemSekolahSMA.Data.Repositories;
using SistemSekolahSMA.Models;

namespace SistemSekolahSMA.Services
{
    public interface ISiswaService
    {
        Task<IEnumerable<Siswa>> GetAllSiswaAsync();
        Task<Siswa> GetSiswaByIdAsync(int siswaId);
        Task<IEnumerable<Siswa>> GetSiswaByKelasIdAsync(int kelasId);
        Task<IEnumerable<Siswa>> SearchSiswaByNameAsync(string name);
        Task<int> CreateSiswaAsync(Siswa siswa);
        Task<bool> UpdateSiswaAsync(Siswa siswa);
        Task<bool> DeleteSiswaAsync(int siswaId);
        Task<bool> NISNExistsAsync(string nisn);
    }

    public class SiswaService : ISiswaService
    {
        private readonly SiswaRepository _siswaRepository;

        public SiswaService(SiswaRepository siswaRepository)
        {
            _siswaRepository = siswaRepository;
        }

        public async Task<IEnumerable<Siswa>> GetAllSiswaAsync()
        {
            return await _siswaRepository.GetAllAsync();
        }

        public async Task<Siswa> GetSiswaByIdAsync(int siswaId)
        {
            return await _siswaRepository.GetByIdAsync(siswaId);
        }

        public async Task<IEnumerable<Siswa>> GetSiswaByKelasIdAsync(int kelasId)
        {
            return await _siswaRepository.GetByKelasIdAsync(kelasId);
        }

        public async Task<IEnumerable<Siswa>> SearchSiswaByNameAsync(string name)
        {
            return await _siswaRepository.SearchByNameAsync(name);
        }

        public async Task<int> CreateSiswaAsync(Siswa siswa)
        {
            try
            {
                siswa.IsActive = true;
                siswa.CreatedDate = DateTime.Now;
                return await _siswaRepository.CreateAsync(siswa);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SiswaService.CreateSiswaAsync error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateSiswaAsync(Siswa siswa)
        {
            return await _siswaRepository.UpdateAsync(siswa);
        }

        public async Task<bool> DeleteSiswaAsync(int siswaId)
        {
            return await _siswaRepository.DeleteAsync(siswaId);
        }

        public async Task<bool> NISNExistsAsync(string nisn)
        {
            return await _siswaRepository.NISNExistsAsync(nisn);
        }
    }
}