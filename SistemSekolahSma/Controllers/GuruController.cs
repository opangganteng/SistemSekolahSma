using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemSekolahSMA.Models;
using SistemSekolahSMA.Services;
using SistemSekolahSMA.ViewModels;

namespace SistemSekolahSMA.Controllers
{
    [Authorize(Roles = "Guru")]
    public class GuruController : Controller
    {
        private readonly IGuruService _guruService;
        private readonly IJadwalService _jadwalService;
        private readonly IPresensiService _presensiService;

        public GuruController(IGuruService guruService, IJadwalService jadwalService,
            IPresensiService presensiService)
        {
            _guruService = guruService;
            _jadwalService = jadwalService;
            _presensiService = presensiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var guru = await _guruService.GetGuruByUserIdAsync(userId);

                if (guru == null)
                {
                    TempData["Error"] = "Data guru tidak ditemukan";
                    return RedirectToAction("Login", "Account");
                }

                var jadwalMengajar = await _jadwalService.GetJadwalByGuruIdAsync(guru.GuruId);

                var viewModel = new DashboardGuruViewModel
                {
                    DataGuru = guru,
                    JadwalMengajar = jadwalMengajar.Select(j => new JadwalMengajar
                    {
                        JadwalId = j.JadwalId,
                        Hari = j.Hari,
                        JamMulai = j.JamMulai.ToString(@"hh\:mm"),
                        JamSelesai = j.JamSelesai.ToString(@"hh\:mm"),
                        NamaKelas = j.Kelas?.NamaKelas,
                        NamaMapel = j.MataPelajaran?.NamaMapel,
                        Ruangan = j.Ruangan
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dashboard Error: {ex.Message}");
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return View(new DashboardGuruViewModel());
            }
        }

        public async Task<IActionResult> PresensiSiswa(int jadwalId, DateTime? tanggal)
        {
            if (tanggal == null)
                tanggal = DateTime.Today;

            try
            {
                System.Diagnostics.Debug.WriteLine($"=== PresensiSiswa GET ===");
                System.Diagnostics.Debug.WriteLine($"JadwalId: {jadwalId}, Tanggal: {tanggal}");

                var viewModel = await _presensiService.GetPresensiSiswaViewModelAsync(jadwalId, tanggal.Value);
                if (viewModel == null)
                {
                    System.Diagnostics.Debug.WriteLine("ViewModel null dari service");
                    TempData["Error"] = "Jadwal tidak ditemukan";
                    return RedirectToAction("Index");
                }

                System.Diagnostics.Debug.WriteLine($"ViewModel: {viewModel.NamaKelas}, Total Siswa: {viewModel.DaftarSiswa?.Count}");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresensiSiswa GET Error: {ex.Message}");
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SimpanPresensiSiswa(PresensiSiswaViewModel model)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== SimpanPresensiSiswa Called ===");
                System.Diagnostics.Debug.WriteLine($"JadwalId: {model.JadwalId}");
                System.Diagnostics.Debug.WriteLine($"TanggalPresensi: {model.TanggalPresensi}");
                System.Diagnostics.Debug.WriteLine($"Total Siswa: {model.DaftarSiswa?.Count}");

                // Remove validation errors for optional fields
                var keysToRemove = ModelState.Keys.Where(k => k.Contains(".Keterangan")).ToList();
                foreach (var key in keysToRemove)
                {
                    ModelState.Remove(key);
                }

                // Set default values
                if (model.DaftarSiswa != null)
                {
                    foreach (var siswa in model.DaftarSiswa)
                    {
                        if (string.IsNullOrEmpty(siswa.Keterangan))
                        {
                            siswa.Keterangan = "";
                        }
                        if (string.IsNullOrEmpty(siswa.StatusKehadiran))
                        {
                            siswa.StatusKehadiran = "Hadir";
                        }
                        System.Diagnostics.Debug.WriteLine($"Siswa: {siswa.NamaSiswa}, Status: {siswa.StatusKehadiran}");
                    }
                }

                if (!ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("ModelState tidak valid!");
                    foreach (var error in ModelState)
                    {
                        System.Diagnostics.Debug.WriteLine($"Field: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                    TempData["Error"] = "Data presensi tidak valid";
                    return View("PresensiSiswa", model);
                }

                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var guru = await _guruService.GetGuruByUserIdAsync(userId);

                if (guru == null)
                {
                    TempData["Error"] = "Data guru tidak ditemukan";
                    return RedirectToAction("Index");
                }

                var result = await _presensiService.SavePresensiSiswaAsync(model, guru.GuruId);

                if (result)
                {
                    TempData["Success"] = "Presensi siswa berhasil disimpan";
                }
                else
                {
                    TempData["Error"] = "Gagal menyimpan presensi siswa";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SimpanPresensiSiswa Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            return RedirectToAction("PresensiSiswa", new { jadwalId = model.JadwalId, tanggal = model.TanggalPresensi });
        }

        public async Task<IActionResult> PresensiGuru()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                System.Diagnostics.Debug.WriteLine($"=== PresensiGuru GET ===");
                System.Diagnostics.Debug.WriteLine($"UserId: {userId}");

                var guru = await _guruService.GetGuruByUserIdAsync(userId);
                if (guru == null)
                {
                    System.Diagnostics.Debug.WriteLine("Guru tidak ditemukan!");
                    TempData["Error"] = "Data guru tidak ditemukan";
                    ViewBag.JadwalOptions = new List<SelectListItem>();
                    return View(new PresensiGuru { TanggalPresensi = DateTime.Today });
                }

                System.Diagnostics.Debug.WriteLine($"GuruId: {guru.GuruId}, Nama: {guru.NamaGuru}");

                var jadwalGuru = await _jadwalService.GetJadwalByGuruIdAsync(guru.GuruId);
                System.Diagnostics.Debug.WriteLine($"Total jadwal ditemukan: {jadwalGuru?.Count() ?? 0}");

                var jadwalOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "-- Pilih Jadwal Mengajar --" }
                };

                if (jadwalGuru?.Any() == true)
                {
                    foreach (var jadwal in jadwalGuru)
                    {
                        var kelasName = jadwal.Kelas?.NamaKelas ?? "Kelas N/A";
                        var mapelName = jadwal.MataPelajaran?.NamaMapel ?? "Mapel N/A";
                        var ruangan = !string.IsNullOrEmpty(jadwal.Ruangan) ? $" | {jadwal.Ruangan}" : "";
                        var jadwalText = $"{jadwal.Hari} - {jadwal.JamMulai:hh\\:mm}-{jadwal.JamSelesai:hh\\:mm} | {kelasName} - {mapelName}{ruangan}";

                        jadwalOptions.Add(new SelectListItem
                        {
                            Value = jadwal.JadwalId.ToString(),
                            Text = jadwalText
                        });

                        System.Diagnostics.Debug.WriteLine($"Jadwal: {jadwal.JadwalId} - {jadwalText}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Tidak ada jadwal ditemukan!");
                    TempData["Warning"] = "Tidak ada jadwal mengajar ditemukan. Hubungi admin untuk menambahkan jadwal.";
                }

                ViewBag.JadwalOptions = jadwalOptions;
                ViewBag.GuruId = guru.GuruId;
                ViewBag.TotalJadwal = jadwalOptions.Count - 1;

                System.Diagnostics.Debug.WriteLine($"ViewBag.JadwalOptions count: {jadwalOptions.Count}");

                return View(new PresensiGuru { TanggalPresensi = DateTime.Today });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresensiGuru GET Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                ViewBag.JadwalOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "-- Error loading jadwal --" }
                };
                return View(new PresensiGuru { TanggalPresensi = DateTime.Today });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SimpanPresensiGuru(PresensiGuru model)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=====================================");
                System.Diagnostics.Debug.WriteLine("=== SimpanPresensiGuru Called ===");
                System.Diagnostics.Debug.WriteLine("=====================================");

                // Log semua data yang diterima
                System.Diagnostics.Debug.WriteLine($"Model received:");
                System.Diagnostics.Debug.WriteLine($"  - JadwalId: {model.JadwalId}");
                System.Diagnostics.Debug.WriteLine($"  - TanggalPresensi: {model.TanggalPresensi}");
                System.Diagnostics.Debug.WriteLine($"  - StatusKehadiran: '{model.StatusKehadiran}'");
                System.Diagnostics.Debug.WriteLine($"  - MateriPelajaran: '{model.MateriPelajaran}'");
                System.Diagnostics.Debug.WriteLine($"  - Keterangan: '{model.Keterangan}'");
                System.Diagnostics.Debug.WriteLine($"  - GuruId: {model.GuruId}");

                // ✅ HAPUS VALIDATION ERROR UNTUK NAVIGATION PROPERTIES
                var keysToRemove = ModelState.Keys.Where(k =>
                    k.Equals("Guru") ||
                    k.Equals("Jadwal") ||
                    k.Contains("Guru.") ||
                    k.Contains("Jadwal.")).ToList();

                foreach (var key in keysToRemove)
                {
                    ModelState.Remove(key);
                    System.Diagnostics.Debug.WriteLine($"Removed ModelState key: {key}");
                }

                // Cek ModelState setelah cleanup
                System.Diagnostics.Debug.WriteLine($"ModelState.IsValid after cleanup: {ModelState.IsValid}");

                if (!ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("❌ MODELSTATE ERRORS REMAINING:");
                    foreach (var error in ModelState)
                    {
                        if (error.Value.Errors.Any())
                        {
                            System.Diagnostics.Debug.WriteLine($"  Field: {error.Key}");
                            foreach (var err in error.Value.Errors)
                            {
                                System.Diagnostics.Debug.WriteLine($"    Error: {err.ErrorMessage}");
                            }
                        }
                    }

                    TempData["Error"] = "Data presensi tidak valid. Pastikan semua field wajib sudah diisi.";
                    await LoadJadwalOptions();
                    return View("PresensiGuru", model);
                }

                // Get user info
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                System.Diagnostics.Debug.WriteLine($"Current UserId: {userId}");

                var guru = await _guruService.GetGuruByUserIdAsync(userId);

                if (guru == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ GURU TIDAK DITEMUKAN!");
                    TempData["Error"] = "Data guru tidak ditemukan";
                    return RedirectToAction("Index");
                }

                System.Diagnostics.Debug.WriteLine($"✅ Guru found: {guru.NamaGuru} (GuruId: {guru.GuruId})");

                // Set GuruId ke model
                model.GuruId = guru.GuruId;
                System.Diagnostics.Debug.WriteLine($"Model.GuruId set to: {model.GuruId}");

                // Validate required fields manually
                if (model.JadwalId <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("❌ JadwalId tidak valid!");
                    TempData["Error"] = "Pilih jadwal mengajar terlebih dahulu";
                    await LoadJadwalOptions();
                    return View("PresensiGuru", model);
                }

                if (string.IsNullOrEmpty(model.StatusKehadiran))
                {
                    System.Diagnostics.Debug.WriteLine("❌ StatusKehadiran kosong!");
                    TempData["Error"] = "Pilih status kehadiran terlebih dahulu";
                    await LoadJadwalOptions();
                    return View("PresensiGuru", model);
                }

                // Call service to save
                System.Diagnostics.Debug.WriteLine("Calling PresensiService.SavePresensiGuruAsync...");
                var result = await _presensiService.SavePresensiGuruAsync(model);
                System.Diagnostics.Debug.WriteLine($"Service result: {result}");

                if (result)
                {
                    System.Diagnostics.Debug.WriteLine("✅ SUCCESS: Presensi guru berhasil disimpan");
                    TempData["Success"] = "Presensi guru berhasil disimpan";
                    return RedirectToAction("PresensiGuru");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("❌ FAILED: Service returned false");
                    TempData["Error"] = "Gagal menyimpan presensi guru - service returned false";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌❌❌ EXCEPTION OCCURRED ❌❌❌");
                System.Diagnostics.Debug.WriteLine($"Exception Type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                TempData["Error"] = $"Terjadi kesalahan: {ex.Message}";
            }

            System.Diagnostics.Debug.WriteLine("Returning to PresensiGuru view with error...");
            await LoadJadwalOptions();
            return View("PresensiGuru", model);
        }

        public async Task<IActionResult> LaporanPresensi(int? bulan, int? tahun)
        {
            var targetBulan = bulan ?? DateTime.Now.Month;
            var targetTahun = tahun ?? DateTime.Now.Year;

            try
            {
                var laporanSiswa = await _presensiService.GetLaporanPresensiSiswaAsync(targetBulan, targetTahun);
                var laporanGuru = await _presensiService.GetLaporanPresensiGuruAsync(targetBulan, targetTahun);

                ViewBag.LaporanSiswa = laporanSiswa;
                ViewBag.LaporanGuru = laporanGuru;
                ViewBag.Bulan = targetBulan;
                ViewBag.Tahun = targetTahun;

                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LaporanPresensi Error: {ex.Message}");
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> KirimLaporan(int bulan, int tahun, string jenisLaporan)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var guru = await _guruService.GetGuruByUserIdAsync(userId);

                var result = await _presensiService.KirimLaporanAsync(guru.GuruId, bulan, tahun, jenisLaporan);

                if (result)
                {
                    TempData["Success"] = $"Laporan {jenisLaporan} bulan {bulan}/{tahun} berhasil dikirim ke admin";
                }
                else
                {
                    TempData["Error"] = "Gagal mengirim laporan";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"KirimLaporan Error: {ex.Message}");
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            return RedirectToAction("LaporanPresensi", new { bulan, tahun });
        }

        private async Task LoadJadwalOptions()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var guru = await _guruService.GetGuruByUserIdAsync(userId);

                if (guru != null)
                {
                    var jadwalGuru = await _jadwalService.GetJadwalByGuruIdAsync(guru.GuruId);

                    ViewBag.JadwalOptions = jadwalGuru?.Select(j => new SelectListItem
                    {
                        Value = j.JadwalId.ToString(),
                        Text = $"{j.Hari} - {j.JamMulai:hh\\:mm}-{j.JamSelesai:hh\\:mm} | {j.Kelas?.NamaKelas} - {j.MataPelajaran?.NamaMapel}"
                    }).ToList() ?? new List<SelectListItem>();
                }
                else
                {
                    ViewBag.JadwalOptions = new List<SelectListItem>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadJadwalOptions Error: {ex.Message}");
                ViewBag.JadwalOptions = new List<SelectListItem>();
            }
        }
    }
}