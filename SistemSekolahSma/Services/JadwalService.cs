using SistemSekolahSMA.Data.Repositories;
using SistemSekolahSMA.Models;
using SistemSekolahSMA.ViewModels;

namespace SistemSekolahSMA.Services
{
    public interface IJadwalService
    {
        Task<IEnumerable<Jadwal>> GetAllJadwalAsync();
        Task<Jadwal> GetJadwalByIdAsync(int jadwalId);
        Task<IEnumerable<Jadwal>> GetJadwalByGuruIdAsync(int guruId);
        Task<IEnumerable<Jadwal>> GetJadwalByKelasIdAsync(int kelasId);
        Task<IEnumerable<JadwalKelas>> SearchJadwalByKelasAsync(string namaKelas);
        Task<int> CreateJadwalAsync(Jadwal jadwal);
        Task<bool> UpdateJadwalAsync(Jadwal jadwal);
        Task<bool> DeleteJadwalAsync(int jadwalId);
    }

    public class JadwalService : IJadwalService
    {
        private readonly IJadwalRepository _jadwalRepository;

        public JadwalService(IJadwalRepository jadwalRepository)
        {
            _jadwalRepository = jadwalRepository;
        }

        public async Task<IEnumerable<Jadwal>> GetAllJadwalAsync()
        {
            try
            {
                return await _jadwalRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"JadwalService.GetAllJadwalAsync error: {ex.Message}");
                return new List<Jadwal>();
            }
        }

        public async Task<Jadwal> GetJadwalByIdAsync(int jadwalId)
        {
            try
            {
                return await _jadwalRepository.GetByIdAsync(jadwalId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"JadwalService.GetJadwalByIdAsync error: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<Jadwal>> GetJadwalByGuruIdAsync(int guruId)
        {
            try
            {
                return await _jadwalRepository.GetByGuruIdAsync(guruId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"JadwalService.GetJadwalByGuruIdAsync error: {ex.Message}");
                return new List<Jadwal>();
            }
        }

        public async Task<IEnumerable<Jadwal>> GetJadwalByKelasIdAsync(int kelasId)
        {
            try
            {
                return await _jadwalRepository.GetByKelasIdAsync(kelasId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"JadwalService.GetJadwalByKelasIdAsync error: {ex.Message}");
                return new List<Jadwal>();
            }
        }

        // Method yang menggunakan method repository yang sudah ada
        public async Task<IEnumerable<JadwalKelas>> SearchJadwalByKelasAsync(string namaKelas)
        {
            try
            {
                // Langsung return dari repository karena sudah mengembalikan JadwalKelas
                return await _jadwalRepository.SearchJadwalByKelasAsync(namaKelas);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"JadwalService.SearchJadwalByKelasAsync error: {ex.Message}");
                return new List<JadwalKelas>();
            }
        }

        public async Task<int> CreateJadwalAsync(Jadwal jadwal)
        {
            try
            {
                jadwal.IsActive = true;
                jadwal.CreatedDate = DateTime.Now;
                return await _jadwalRepository.CreateAsync(jadwal);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"JadwalService.CreateJadwalAsync error: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> UpdateJadwalAsync(Jadwal jadwal)
        {
            try
            {
                return await _jadwalRepository.UpdateAsync(jadwal);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"JadwalService.UpdateJadwalAsync error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteJadwalAsync(int jadwalId)
        {
            try
            {
                return await _jadwalRepository.DeleteAsync(jadwalId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"JadwalService.DeleteJadwalAsync error: {ex.Message}");
                return false;
            }
        }
    }
}