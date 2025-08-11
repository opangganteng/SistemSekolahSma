using SistemSekolahSMA.Data.Repositories;
using SistemSekolahSMA.Models;

namespace SistemSekolahSMA.Services
{
    public interface IMataPelajaranService
    {
        Task<IEnumerable<MataPelajaran>> GetAllMataPelajaranAsync();
        Task<MataPelajaran> GetMataPelajaranByIdAsync(int mataPelajaranId);
        Task<int> CreateMataPelajaranAsync(MataPelajaran mataPelajaran);
        Task<bool> UpdateMataPelajaranAsync(MataPelajaran mataPelajaran);
        Task<bool> DeleteMataPelajaranAsync(int mataPelajaranId);
        Task<bool> KodeMapelExistsAsync(string kodeMapel);
    }

    public class MataPelajaranService : IMataPelajaranService
    {
        private readonly IMataPelajaranRepository _mataPelajaranRepository;

        public MataPelajaranService(IMataPelajaranRepository mataPelajaranRepository)
        {
            _mataPelajaranRepository = mataPelajaranRepository;
        }

        public async Task<IEnumerable<MataPelajaran>> GetAllMataPelajaranAsync()
        {
            try
            {
                return await _mataPelajaranRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MataPelajaranService.GetAllMataPelajaranAsync error: {ex.Message}");
                return new List<MataPelajaran>();
            }
        }

        public async Task<MataPelajaran> GetMataPelajaranByIdAsync(int mataPelajaranId)
        {
            try
            {
                return await _mataPelajaranRepository.GetByIdAsync(mataPelajaranId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MataPelajaranService.GetMataPelajaranByIdAsync error: {ex.Message}");
                return null;
            }
        }

        public async Task<int> CreateMataPelajaranAsync(MataPelajaran mataPelajaran)
        {
            try
            {
                mataPelajaran.IsActive = true;
                mataPelajaran.CreatedDate = DateTime.Now;
                return await _mataPelajaranRepository.CreateAsync(mataPelajaran);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MataPelajaranService.CreateMataPelajaranAsync error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateMataPelajaranAsync(MataPelajaran mataPelajaran)
        {
            try
            {
                return await _mataPelajaranRepository.UpdateAsync(mataPelajaran);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MataPelajaranService.UpdateMataPelajaranAsync error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteMataPelajaranAsync(int mataPelajaranId)
        {
            try
            {
                return await _mataPelajaranRepository.DeleteAsync(mataPelajaranId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MataPelajaranService.DeleteMataPelajaranAsync error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> KodeMapelExistsAsync(string kodeMapel)
        {
            try
            {
                return await _mataPelajaranRepository.KodeMapelExistsAsync(kodeMapel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MataPelajaranService.KodeMapelExistsAsync error: {ex.Message}");
                return false;
            }
        }
    }
}