using SistemSekolahSMA.Data.Repositories;
using SistemSekolahSMA.Models;

namespace SistemSekolahSMA.Services
{
    // INTERFACE DEFINITION
    public interface IGuruService
    {
        Task<Guru> GetGuruByIdAsync(int guruId);
        Task<Guru> GetGuruByUserIdAsync(int userId);
        Task<IEnumerable<Guru>> GetAllGuruAsync();
        Task<int> CreateGuruAsync(Guru guru);
        Task<bool> UpdateGuruAsync(Guru guru);
        Task<bool> DeleteGuruAsync(int guruId);
        Task<bool> NIPExistsAsync(string nip);
    }

    // CLASS IMPLEMENTATION
    public class GuruService : IGuruService
    {
        private readonly IGuruRepository _guruRepository;

        public GuruService(IGuruRepository guruRepository)
        {
            _guruRepository = guruRepository;
        }

        public async Task<Guru> GetGuruByIdAsync(int guruId)
        {
            return await _guruRepository.GetByIdAsync(guruId);
        }

        public async Task<Guru> GetGuruByUserIdAsync(int userId)
        {
            return await _guruRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Guru>> GetAllGuruAsync()
        {
            return await _guruRepository.GetAllAsync();
        }

        public async Task<int> CreateGuruAsync(Guru guru)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== GuruService.CreateGuruAsync ===");
                System.Diagnostics.Debug.WriteLine($"Input - UserId: {guru.UserId}, NIP: {guru.NIP}, Nama: {guru.NamaGuru}");

                var result = await _guruRepository.CreateAsync(guru);

                System.Diagnostics.Debug.WriteLine($"Repository result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GuruService.CreateGuruAsync error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateGuruAsync(Guru guru)
        {
            return await _guruRepository.UpdateAsync(guru);
        }

        public async Task<bool> DeleteGuruAsync(int guruId)
        {
            return await _guruRepository.DeleteAsync(guruId);
        }

        public async Task<bool> NIPExistsAsync(string nip)
        {
            return await _guruRepository.NIPExistsAsync(nip);
        }
    }
}