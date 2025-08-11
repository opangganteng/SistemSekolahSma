using SistemSekolahSMA.Data.Repositories;
using SistemSekolahSMA.Models;

namespace SistemSekolahSMA.Services
{
    public interface IKelasService
    {
        Task<IEnumerable<Kelas>> GetAllKelasAsync();
        Task<Kelas> GetKelasByIdAsync(int kelasId);
        Task<int> CreateKelasAsync(Kelas kelas);
        Task<bool> UpdateKelasAsync(Kelas kelas);
        Task<bool> DeleteKelasAsync(int kelasId);
        Task<bool> NamaKelasExistsAsync(string namaKelas);
    }

    public class KelasService : IKelasService
    {
        private readonly IKelasRepository _kelasRepository;

        public KelasService(IKelasRepository kelasRepository)
        {
            _kelasRepository = kelasRepository;
        }

        public async Task<IEnumerable<Kelas>> GetAllKelasAsync()
        {
            try
            {
                return await _kelasRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"KelasService.GetAllKelasAsync error: {ex.Message}");
                return new List<Kelas>();
            }
        }

        public async Task<Kelas> GetKelasByIdAsync(int kelasId)
        {
            try
            {
                return await _kelasRepository.GetByIdAsync(kelasId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"KelasService.GetKelasByIdAsync error: {ex.Message}");
                return null;
            }
        }

        public async Task<int> CreateKelasAsync(Kelas kelas)
        {
            try
            {
                kelas.IsActive = true;
                kelas.CreatedDate = DateTime.Now;
                return await _kelasRepository.CreateAsync(kelas);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"KelasService.CreateKelasAsync error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateKelasAsync(Kelas kelas)
        {
            try
            {
                return await _kelasRepository.UpdateAsync(kelas);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"KelasService.UpdateKelasAsync error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteKelasAsync(int kelasId)
        {
            try
            {
                return await _kelasRepository.DeleteAsync(kelasId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"KelasService.DeleteKelasAsync error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> NamaKelasExistsAsync(string namaKelas)
        {
            try
            {
                return await _kelasRepository.NamaKelasExistsAsync(namaKelas);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"KelasService.NamaKelasExistsAsync error: {ex.Message}");
                return false;
            }
        }
    }
}