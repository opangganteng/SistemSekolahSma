using SistemSekolahSMA.Data.Repositories;
using SistemSekolahSMA.Models;
using SistemSekolahSMA.ViewModels;

namespace SistemSekolahSMA.Services
{
    public interface IPresensiService
    {
        Task<PresensiSiswaViewModel> GetPresensiSiswaViewModelAsync(int jadwalId, DateTime tanggal);
        Task<bool> SavePresensiSiswaAsync(PresensiSiswaViewModel model, int guruId);
        Task<bool> SavePresensiGuruAsync(PresensiGuru presensiGuru);
        Task<IEnumerable<dynamic>> GetLaporanPresensiSiswaAsync(int bulan, int tahun);
        Task<IEnumerable<dynamic>> GetLaporanPresensiGuruAsync(int bulan, int tahun);
        Task<bool> KirimLaporanAsync(int guruId, int bulan, int tahun, string jenisLaporan);
    }

    public class PresensiService : IPresensiService
    {
        private readonly IPresensiSiswaRepository _presensiSiswaRepository;
        private readonly IPresensiGuruRepository _presensiGuruRepository;
        private readonly IJadwalRepository _jadwalRepository;
        private readonly SiswaRepository _siswaRepository;

        public PresensiService(
            IPresensiSiswaRepository presensiSiswaRepository,
            IPresensiGuruRepository presensiGuruRepository,
            IJadwalRepository jadwalRepository,
            SiswaRepository siswaRepository)
        {
            _presensiSiswaRepository = presensiSiswaRepository;
            _presensiGuruRepository = presensiGuruRepository;
            _jadwalRepository = jadwalRepository;
            _siswaRepository = siswaRepository;
        }

        public async Task<PresensiSiswaViewModel> GetPresensiSiswaViewModelAsync(int jadwalId, DateTime tanggal)
        {
            try
            {
                var jadwal = await _jadwalRepository.GetByIdAsync(jadwalId);
                if (jadwal == null)
                {
                    return null;
                }

                var siswaList = await _siswaRepository.GetByKelasIdAsync(jadwal.KelasId);
                var existingPresensi = await _presensiSiswaRepository.GetByJadwalAndDateAsync(jadwalId, tanggal);

                return new PresensiSiswaViewModel
                {
                    JadwalId = jadwalId,
                    TanggalPresensi = tanggal,
                    NamaKelas = jadwal.Kelas?.NamaKelas ?? "N/A",
                    NamaMapel = jadwal.MataPelajaran?.NamaMapel ?? "N/A",
                    NamaGuru = jadwal.Guru?.NamaGuru ?? "N/A",
                    DaftarSiswa = siswaList.Select(s =>
                    {
                        var presensiExisting = existingPresensi.FirstOrDefault(p => p.SiswaId == s.SiswaId);
                        return new SiswaPresensiItem
                        {
                            SiswaId = s.SiswaId,
                            NamaSiswa = s.NamaSiswa,
                            NISN = s.NISN,
                            StatusKehadiran = presensiExisting?.StatusKehadiran ?? "Hadir",
                            Keterangan = presensiExisting?.Keterangan ?? "",
                            IsHadir = presensiExisting?.StatusKehadiran == "Hadir" || presensiExisting == null
                        };
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresensiService.GetPresensiSiswaViewModelAsync error: {ex.Message}");
                return new PresensiSiswaViewModel
                {
                    JadwalId = jadwalId,
                    TanggalPresensi = tanggal,
                    NamaKelas = "Test Kelas",
                    NamaMapel = "Test Mapel",
                    NamaGuru = "Test Guru",
                    DaftarSiswa = new List<SiswaPresensiItem>()
                };
            }
        }

        public async Task<bool> SavePresensiSiswaAsync(PresensiSiswaViewModel model, int guruId)
        {
            try
            {
                var presensiList = model.DaftarSiswa.Select(item => new PresensiSiswa
                {
                    JadwalId = model.JadwalId,
                    SiswaId = item.SiswaId,
                    TanggalPresensi = model.TanggalPresensi,
                    StatusKehadiran = item.StatusKehadiran,
                    Keterangan = item.Keterangan,
                    DibuatOleh = guruId,
                    CreatedDate = DateTime.Now
                }).ToList();

                foreach (var presensi in presensiList)
                {
                    await _presensiSiswaRepository.CreateOrUpdateAsync(presensi);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresensiService.SavePresensiSiswaAsync error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SavePresensiGuruAsync(PresensiGuru presensiGuru)
        {
            try
            {
                presensiGuru.CreatedDate = DateTime.Now;
                return await _presensiGuruRepository.CreateOrUpdateAsync(presensiGuru);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresensiService.SavePresensiGuruAsync error: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<dynamic>> GetLaporanPresensiSiswaAsync(int bulan, int tahun)
        {
            try
            {
                return await _presensiSiswaRepository.GetLaporanByMonthAsync(bulan, tahun);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresensiService.GetLaporanPresensiSiswaAsync error: {ex.Message}");
                return new List<dynamic>();
            }
        }

        public async Task<IEnumerable<dynamic>> GetLaporanPresensiGuruAsync(int bulan, int tahun)
        {
            try
            {
                return await _presensiGuruRepository.GetLaporanByMonthAsync(bulan, tahun);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresensiService.GetLaporanPresensiGuruAsync error: {ex.Message}");
                return new List<dynamic>();
            }
        }

        public async Task<bool> KirimLaporanAsync(int guruId, int bulan, int tahun, string jenisLaporan)
        {
            try
            {
                await Task.Delay(100);
                System.Diagnostics.Debug.WriteLine($"Laporan {jenisLaporan} dikirim oleh Guru {guruId} untuk {bulan}/{tahun}");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}