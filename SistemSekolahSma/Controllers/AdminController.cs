using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemSekolahSMA.Data.Repositories;
using SistemSekolahSMA.Models;
using SistemSekolahSMA.Services;
using System.Text;

namespace SistemSekolahSMA.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IGuruService _guruService;
        private readonly ISiswaService _siswaService;
        private readonly IJadwalService _jadwalService;
        private readonly IPresensiService _presensiService;
        private readonly IKelasService _kelasService;
        private readonly IMataPelajaranService _mataPelajaranService;

        public AdminController(
            IUserService userService,
            IGuruService guruService,
            ISiswaService siswaService,
            IJadwalService jadwalService,
            IPresensiService presensiService,
            IKelasService kelasService,
            IMataPelajaranService mataPelajaranService)
        {
            _userService = userService;
            _guruService = guruService;
            _siswaService = siswaService;
            _jadwalService = jadwalService;
            _presensiService = presensiService;
            _kelasService = kelasService;
            _mataPelajaranService = mataPelajaranService;
        }

        // ===== DASHBOARD =====
        public async Task<IActionResult> Index()
        {
            try
            {
                var totalUsers = (await _userService.GetAllUsersAsync()).Count();
                var totalGuru = (await _guruService.GetAllGuruAsync()).Count();
                var totalSiswa = (await _siswaService.GetAllSiswaAsync()).Count();
                var totalKelas = (await _kelasService.GetAllKelasAsync()).Count();
                var totalMataPelajaran = (await _mataPelajaranService.GetAllMataPelajaranAsync()).Count();
                var totalJadwal = (await _jadwalService.GetAllJadwalAsync()).Count();

                ViewBag.TotalUsers = totalUsers;
                ViewBag.TotalGuru = totalGuru;
                ViewBag.TotalSiswa = totalSiswa;
                ViewBag.TotalKelas = totalKelas;
                ViewBag.TotalMataPelajaran = totalMataPelajaran;
                ViewBag.TotalJadwal = totalJadwal;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return View();
            }
        }

        // ===== CRUD USERS =====
        public async Task<IActionResult> ManajemenUser()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return View(new List<Models.User>());
            }
        }

        public IActionResult TambahUser()
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "Guru", Text = "Guru" },
                new SelectListItem { Value = "Siswa", Text = "Siswa" }
            };
            return View(new Models.User());
        }

        [HttpPost]
        public async Task<IActionResult> TambahUser(Models.User model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<SelectListItem>
        {
            new SelectListItem { Value = "Admin", Text = "Admin" },
            new SelectListItem { Value = "Guru", Text = "Guru" },
            new SelectListItem { Value = "Siswa", Text = "Siswa" }
        };
                return View(model);
            }

            try
            {
                // Check if username already exists - gunakan method yang sudah ada
                if (await _userService.UsernameExistsAsync(model.Username))
                {
                    ModelState.AddModelError("Username", "Username sudah digunakan");
                    ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "Guru", Text = "Guru" },
                new SelectListItem { Value = "Siswa", Text = "Siswa" }
            };
                    return View(model);
                }

                // Set default values
                model.IsActive = true;
                model.CreatedDate = DateTime.Now;

                var result = await _userService.CreateUserAsync(model);
                if (result > 0)
                {
                    TempData["Success"] = "User berhasil ditambahkan";
                    return RedirectToAction("ManajemenUser");
                }

                ModelState.AddModelError("", "Gagal menambahkan user");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            ViewBag.Roles = new List<SelectListItem>
    {
        new SelectListItem { Value = "Admin", Text = "Admin" },
        new SelectListItem { Value = "Guru", Text = "Guru" },
        new SelectListItem { Value = "Siswa", Text = "Siswa" }
    };
            return View(model);
        }

        public async Task<IActionResult> EditUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User tidak ditemukan";
                    return RedirectToAction("ManajemenUser");
                }

                ViewBag.Roles = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Admin", Text = "Admin", Selected = user.Role == "Admin" },
                    new SelectListItem { Value = "Guru", Text = "Guru", Selected = user.Role == "Guru" },
                    new SelectListItem { Value = "Siswa", Text = "Siswa", Selected = user.Role == "Siswa" }
                };

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return RedirectToAction("ManajemenUser");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(Models.User model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Admin", Text = "Admin", Selected = model.Role == "Admin" },
                    new SelectListItem { Value = "Guru", Text = "Guru", Selected = model.Role == "Guru" },
                    new SelectListItem { Value = "Siswa", Text = "Siswa", Selected = model.Role == "Siswa" }
                };
                return View(model);
            }

            try
            {
                var result = await _userService.UpdateUserAsync(model);
                if (result)
                {
                    TempData["Success"] = "User berhasil diupdate";
                    return RedirectToAction("ManajemenUser");
                }

                ModelState.AddModelError("", "Gagal mengupdate user");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Admin", Selected = model.Role == "Admin" },
                new SelectListItem { Value = "Guru", Text = "Guru", Selected = model.Role == "Guru" },
                new SelectListItem { Value = "Siswa", Text = "Siswa", Selected = model.Role == "Siswa" }
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> HapusUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (result)
                {
                    TempData["Success"] = "User berhasil dihapus";
                }
                else
                {
                    TempData["Error"] = "Gagal menghapus user";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            return RedirectToAction("ManajemenUser");
        }

        // ===== CRUD GURU =====
        public async Task<IActionResult> MasterGuru()
        {
            try
            {
                var gurus = await _guruService.GetAllGuruAsync();
                return View(gurus);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return View(new List<Models.Guru>());
            }
        }

        public async Task<IActionResult> TambahGuru()
        {
            try
            {
                await LoadUserOptions();
                return View(new Models.Guru());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading form: " + ex.Message;
                ViewBag.Users = new List<SelectListItem>();
                return View(new Models.Guru());
            }
        }

        [HttpPost]
        public async Task<IActionResult> TambahGuru(Models.Guru model)
        {
            if (ModelState.ContainsKey("User"))
            {
                ModelState.Remove("User");
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    await LoadUserOptions();
                }
                catch
                {
                    ViewBag.Users = new List<SelectListItem>();
                }
                return View(model);
            }

            try
            {
                model.IsActive = true;
                model.CreatedDate = DateTime.Now;
                var result = await _guruService.CreateGuruAsync(model);
                if (result > 0)
                {
                    TempData["Success"] = "Guru berhasil ditambahkan";
                    return RedirectToAction("MasterGuru");
                }
                ModelState.AddModelError("", "Gagal menambahkan guru");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            try
            {
                await LoadUserOptions();
            }
            catch
            {
                ViewBag.Users = new List<SelectListItem>();
            }
            return View(model);
        }

        public async Task<IActionResult> EditGuru(int id)
        {
            try
            {
                var guru = await _guruService.GetGuruByIdAsync(id);
                if (guru == null)
                {
                    TempData["Error"] = "Guru tidak ditemukan";
                    return RedirectToAction("MasterGuru");
                }

                try
                {
                    await LoadUserOptions();
                }
                catch
                {
                    ViewBag.Users = new List<SelectListItem>();
                }

                return View(guru);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return RedirectToAction("MasterGuru");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditGuru(Models.Guru model)
        {
            if (ModelState.ContainsKey("User"))
            {
                ModelState.Remove("User");
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    await LoadUserOptions();
                }
                catch
                {
                    ViewBag.Users = new List<SelectListItem>();
                }
                return View(model);
            }

            try
            {
                var result = await _guruService.UpdateGuruAsync(model);
                if (result)
                {
                    TempData["Success"] = "Guru berhasil diupdate";
                    return RedirectToAction("MasterGuru");
                }
                ModelState.AddModelError("", "Gagal mengupdate guru");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            try
            {
                await LoadUserOptions();
            }
            catch
            {
                ViewBag.Users = new List<SelectListItem>();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> HapusGuru(int id)
        {
            try
            {
                var result = await _guruService.DeleteGuruAsync(id);
                if (result)
                {
                    TempData["Success"] = "Guru berhasil dihapus";
                }
                else
                {
                    TempData["Error"] = "Gagal menghapus guru";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            return RedirectToAction("MasterGuru");
        }

        // ===== CRUD SISWA =====
        public async Task<IActionResult> MasterSiswa()
        {
            try
            {
                var siswa = await _siswaService.GetAllSiswaAsync();
                return View(siswa);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return View(new List<Models.Siswa>());
            }
        }

        public async Task<IActionResult> TambahSiswa()
        {
            await LoadKelasOptions();
            return View(new Models.Siswa());
        }

        [HttpPost]
        public async Task<IActionResult> TambahSiswa(Models.Siswa model)
        {
            if (ModelState.ContainsKey("Kelas"))
            {
                ModelState.Remove("Kelas");
            }

            if (!ModelState.IsValid)
            {
                await LoadKelasOptions();
                return View(model);
            }

            try
            {
                if (await _siswaService.NISNExistsAsync(model.NISN))
                {
                    ModelState.AddModelError("NISN", "NISN sudah digunakan");
                    await LoadKelasOptions();
                    return View(model);
                }

                var result = await _siswaService.CreateSiswaAsync(model);
                if (result > 0)
                {
                    TempData["Success"] = "Siswa berhasil ditambahkan";
                    return RedirectToAction("MasterSiswa");
                }

                ModelState.AddModelError("", "Gagal menambahkan siswa");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            await LoadKelasOptions();
            return View(model);
        }

        public async Task<IActionResult> EditSiswa(int id)
        {
            try
            {
                var siswa = await _siswaService.GetSiswaByIdAsync(id);
                if (siswa == null)
                {
                    return NotFound();
                }

                await LoadKelasOptions();
                return View(siswa);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return RedirectToAction("MasterSiswa");
            }
        }

        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSiswa(Siswa model)
        {
            // Pastikan ID siswa ada
            if (model.SiswaId <= 0)
            {
                TempData["Error"] = "ID siswa tidak valid.";
                return RedirectToAction("MasterSiswa");
            }

            // Hilangkan validasi untuk properti navigasi
            if (ModelState.ContainsKey("Kelas"))
            {
                ModelState.Remove("Kelas");
            }

            if (!ModelState.IsValid)
            {
                await LoadKelasOptions();
                return View(model);
            }

            try
            {
                var result = await _siswaService.UpdateSiswaAsync(model);
                if (result)
                {
                    TempData["Success"] = "Data siswa berhasil diperbarui.";
                    return RedirectToAction("MasterSiswa");
                }

                ModelState.AddModelError("", "Gagal mengupdate data siswa.");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            await LoadKelasOptions();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> HapusSiswa(int id)
        {
            try
            {
                var result = await _siswaService.DeleteSiswaAsync(id);
                if (result)
                {
                    TempData["Success"] = "Siswa berhasil dihapus";
                }
                else
                {
                    TempData["Error"] = "Gagal menghapus siswa";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            return RedirectToAction("MasterSiswa");
        }

        // ===== CRUD KELAS =====
        public async Task<IActionResult> MasterKelas()
        {
            try
            {
                var kelas = await _kelasService.GetAllKelasAsync();
                return View(kelas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return View(new List<Models.Kelas>());
            }
        }

        public async Task<IActionResult> TambahKelas()
        {
            await LoadGuruOptions();
            return View(new Models.Kelas());
        }

        [HttpPost]
        public async Task<IActionResult> TambahKelas(Models.Kelas model)
        {
            if (ModelState.ContainsKey("WaliKelasNavigation"))
            {
                ModelState.Remove("WaliKelasNavigation");
            }

            if (!ModelState.IsValid)
            {
                await LoadGuruOptions();
                return View(model);
            }

            try
            {
                if (await _kelasService.NamaKelasExistsAsync(model.NamaKelas))
                {
                    ModelState.AddModelError("NamaKelas", "Nama kelas sudah digunakan");
                    await LoadGuruOptions();
                    return View(model);
                }

                model.IsActive = true;
                model.CreatedDate = DateTime.Now;

                var result = await _kelasService.CreateKelasAsync(model);
                if (result > 0)
                {
                    TempData["Success"] = "Kelas berhasil ditambahkan";
                    return RedirectToAction("MasterKelas");
                }

                ModelState.AddModelError("", "Gagal menambahkan kelas");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            await LoadGuruOptions();
            return View(model);
        }

        public async Task<IActionResult> EditKelas(int id)
        {
            try
            {
                var kelas = await _kelasService.GetKelasByIdAsync(id);
                if (kelas == null)
                {
                    return NotFound();
                }
                await LoadGuruOptions();
                return View(kelas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return RedirectToAction("MasterKelas");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditKelas(Models.Kelas model)
        {
            if (ModelState.ContainsKey("WaliKelasNavigation"))
            {
                ModelState.Remove("WaliKelasNavigation");
            }

            if (!ModelState.IsValid)
            {
                await LoadGuruOptions();
                return View(model);
            }

            try
            {
                var result = await _kelasService.UpdateKelasAsync(model);
                if (result)
                {
                    TempData["Success"] = "Kelas berhasil diupdate";
                    return RedirectToAction("MasterKelas");
                }

                ModelState.AddModelError("", "Gagal mengupdate kelas");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            await LoadGuruOptions();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> HapusKelas(int id)
        {
            try
            {
                var result = await _kelasService.DeleteKelasAsync(id);
                if (result)
                {
                    TempData["Success"] = "Kelas berhasil dihapus";
                }
                else
                {
                    TempData["Error"] = "Gagal menghapus kelas";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            return RedirectToAction("MasterKelas");
        }

        // ===== CRUD MATA PELAJARAN =====
        public async Task<IActionResult> MasterMataPelajaran()
        {
            try
            {
                var mataPelajarans = await _mataPelajaranService.GetAllMataPelajaranAsync();
                return View(mataPelajarans);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return View(new List<Models.MataPelajaran>());
            }
        }

        public IActionResult TambahMataPelajaran()
        {
            return View(new Models.MataPelajaran());
        }

        [HttpPost]
        public async Task<IActionResult> TambahMataPelajaran(Models.MataPelajaran model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (await _mataPelajaranService.KodeMapelExistsAsync(model.KodeMapel))
                {
                    ModelState.AddModelError("KodeMapel", "Kode mata pelajaran sudah digunakan");
                    return View(model);
                }

                model.IsActive = true;
                model.CreatedDate = DateTime.Now;

                var result = await _mataPelajaranService.CreateMataPelajaranAsync(model);
                if (result > 0)
                {
                    TempData["Success"] = "Mata pelajaran berhasil ditambahkan";
                    return RedirectToAction("MasterMataPelajaran");
                }

                ModelState.AddModelError("", "Gagal menambahkan mata pelajaran");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            return View(model);
        }

        public async Task<IActionResult> EditMataPelajaran(int id)
        {
            try
            {
                var mataPelajaran = await _mataPelajaranService.GetMataPelajaranByIdAsync(id);
                if (mataPelajaran == null)
                {
                    return NotFound();
                }
                return View(mataPelajaran);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return RedirectToAction("MasterMataPelajaran");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditMataPelajaran(Models.MataPelajaran model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _mataPelajaranService.UpdateMataPelajaranAsync(model);
                if (result)
                {
                    TempData["Success"] = "Mata pelajaran berhasil diupdate";
                    return RedirectToAction("MasterMataPelajaran");
                }

                ModelState.AddModelError("", "Gagal mengupdate mata pelajaran");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> HapusMataPelajaran(int id)
        {
            try
            {
                var result = await _mataPelajaranService.DeleteMataPelajaranAsync(id);
                if (result)
                {
                    TempData["Success"] = "Mata pelajaran berhasil dihapus";
                }
                else
                {
                    TempData["Error"] = "Gagal menghapus mata pelajaran";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            return RedirectToAction("MasterMataPelajaran");
        }

        // ===== CRUD JADWAL =====
        public async Task<IActionResult> MasterJadwal()
        {
            try
            {
                var jadwal = await _jadwalService.GetAllJadwalAsync();
                return View(jadwal);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return View(new List<Models.Jadwal>());
            }
        }

        public async Task<IActionResult> TambahJadwal()
        {
            await LoadJadwalOptions();
            return View(new Models.Jadwal());
        }

        [HttpPost]
        public async Task<IActionResult> TambahJadwal(Models.Jadwal model)
        {
            ModelState.Remove("Kelas");
            ModelState.Remove("MataPelajaran");
            ModelState.Remove("Guru");

            if (!ModelState.IsValid)
            {
                await LoadJadwalOptions();
                return View(model);
            }

            try
            {
                var result = await _jadwalService.CreateJadwalAsync(model);
                if (result > 0)
                {
                    TempData["Success"] = "Jadwal berhasil ditambahkan";
                    return RedirectToAction("MasterJadwal");
                }

                ModelState.AddModelError("", "Gagal menambahkan jadwal");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            await LoadJadwalOptions();
            return View(model);
        }

        public async Task<IActionResult> EditJadwal(int id)
        {
            try
            {
                var jadwal = await _jadwalService.GetJadwalByIdAsync(id);
                if (jadwal == null)
                {
                    return NotFound();
                }
                await LoadJadwalOptions();
                return View(jadwal);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                return RedirectToAction("MasterJadwal");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditJadwal(Models.Jadwal model)
        {
            ModelState.Remove("Kelas");
            ModelState.Remove("MataPelajaran");
            ModelState.Remove("Guru");

            if (!ModelState.IsValid)
            {
                await LoadJadwalOptions();
                return View(model);
            }

            try
            {
                var result = await _jadwalService.UpdateJadwalAsync(model);
                if (result)
                {
                    TempData["Success"] = "Jadwal berhasil diupdate";
                    return RedirectToAction("MasterJadwal");
                }

                ModelState.AddModelError("", "Gagal mengupdate jadwal");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            await LoadJadwalOptions();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> HapusJadwal(int id)
        {
            try
            {
                var result = await _jadwalService.DeleteJadwalAsync(id);
                if (result)
                {
                    TempData["Success"] = "Jadwal berhasil dihapus";
                }
                else
                {
                    TempData["Error"] = "Gagal menghapus jadwal";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
            }

            return RedirectToAction("MasterJadwal");
        }

        // ===== LAPORAN PRESENSI =====
        public async Task<IActionResult> LaporanPresensi(int? bulan, int? tahun)
        {
            var targetBulan = bulan ?? DateTime.Now.Month;
            var targetTahun = tahun ?? DateTime.Now.Year;

            try
            {
                var laporanSiswa = await _presensiService.GetLaporanPresensiSiswaAsync(targetBulan, targetTahun);
                var laporanGuru = await _presensiService.GetLaporanPresensiGuruAsync(targetBulan, targetTahun);

                ViewBag.LaporanSiswa = laporanSiswa ?? new List<dynamic>();
                ViewBag.LaporanGuru = laporanGuru ?? new List<dynamic>();
                ViewBag.Bulan = targetBulan;
                ViewBag.Tahun = targetTahun;

                return View(); // ✅ Return ke LaporanPresensi.cshtml
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Terjadi kesalahan: " + ex.Message;
                ViewBag.LaporanSiswa = new List<dynamic>();
                ViewBag.LaporanGuru = new List<dynamic>();
                ViewBag.Bulan = targetBulan;
                ViewBag.Tahun = targetTahun;
                return View(); // ✅ Return ke LaporanPresensi.cshtml
            }
        }

        [HttpPost]
        public async Task<IActionResult> FilterLaporan(int bulan, int tahun)
        {
            // ✅ PERBAIKAN: Redirect ke LaporanPresensi dengan parameter
            return RedirectToAction("LaporanPresensi", new { bulan = bulan, tahun = tahun });
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(int bulan, int tahun)
        {
            try
            {
                var laporanSiswa = await _presensiService.GetLaporanPresensiSiswaAsync(bulan, tahun);

                // Buat file CSV yang bisa dibuka di Excel
                var csvContent = new StringBuilder();
                csvContent.AppendLine("No,NISN,Nama Siswa,Kelas,Hadir,Izin,Sakit,Alpha,Total Hari,Persentase Kehadiran");

                int no = 1;
                if (laporanSiswa != null)
                {
                    foreach (dynamic siswa in laporanSiswa)
                    {
                        int hadir = GetSafeInt(siswa.TotalHadir);
                        int izin = GetSafeInt(siswa.TotalIzin);
                        int sakit = GetSafeInt(siswa.TotalSakit);
                        int alpha = GetSafeInt(siswa.TotalAlpha);
                        int totalHari = hadir + izin + sakit + alpha;
                        decimal persentase = totalHari > 0 ? (decimal)hadir / totalHari * 100 : 0;

                        csvContent.AppendLine($"{no},\"{siswa.NISN}\",\"{siswa.NamaSiswa}\",\"{siswa.NamaKelas}\",{hadir},{izin},{sakit},{alpha},{totalHari},{persentase:F1}%");
                        no++;
                    }
                }

                byte[] buffer = Encoding.UTF8.GetBytes(csvContent.ToString());
                string fileName = $"Laporan_Presensi_{GetNamaBulan(bulan)}_{tahun}.csv";

                return File(buffer, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error export Excel: " + ex.Message;
                return RedirectToAction("LaporanPresensi", new { bulan, tahun });
            }
        }
        [HttpGet]
        public async Task<IActionResult> ExportPDF(int bulan, int tahun)
        {
            try
            {
                var laporanSiswa = await _presensiService.GetLaporanPresensiSiswaAsync(bulan, tahun);

                // Generate HTML untuk PDF
                var htmlContent = GenerateHTMLForPDF(laporanSiswa, bulan, tahun);

                // Return sebagai HTML yang bisa di-print as PDF
                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error export PDF: " + ex.Message;
                return RedirectToAction("LaporanPresensi", new { bulan, tahun });
            }
        }
        [HttpGet]
        public async Task<IActionResult> CetakLaporan(int bulan, int tahun)
        {
            try
            {
                var laporanSiswa = await _presensiService.GetLaporanPresensiSiswaAsync(bulan, tahun);
                var laporanGuru = await _presensiService.GetLaporanPresensiGuruAsync(bulan, tahun);

                ViewBag.LaporanSiswa = laporanSiswa ?? new List<dynamic>();
                ViewBag.LaporanGuru = laporanGuru ?? new List<dynamic>();
                ViewBag.Bulan = bulan;
                ViewBag.Tahun = tahun;
                ViewBag.IsCetak = true; // Flag untuk print version

                return View("LaporanPresensi"); // ✅ Explicit view name
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error cetak: " + ex.Message;
                return RedirectToAction("LaporanPresensi", new { bulan, tahun });
            }
        }

        // Helper method untuk handle null values
        private int GetSafeInt(dynamic value)
        {
            if (value == null) return 0;
            if (int.TryParse(value.ToString(), out int result))
                return result;
            return 0;
        }

        // ===== HELPER METHODS =====
        private async Task LoadUserOptions()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var guruUsers = users.Where(u => u.Role == "Guru" && u.IsActive).ToList();

                ViewBag.Users = guruUsers.Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = $"{u.Username} ({u.Email})"
                }).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadUserOptions error: {ex.Message}");
                ViewBag.Users = new List<SelectListItem>();
                throw;
            }
        }

        private async Task LoadKelasOptions()
        {
            var kelasList = await _kelasService.GetAllKelasAsync();
            ViewBag.Kelas = kelasList;
        }

        private async Task LoadGuruOptions()
        {
            try
            {
                var gurus = await _guruService.GetAllGuruAsync();
                ViewBag.Gurus = gurus.Select(g => new SelectListItem
                {
                    Value = g.GuruId.ToString(),
                    Text = g.NamaGuru
                }).ToList();
            }
            catch
            {
                ViewBag.Gurus = new List<SelectListItem>();
            }
        }

        private async Task LoadJadwalOptions()
        {
            try
            {
                // Load Kelas - langsung sebagai collection, bukan SelectList
                var kelasList = await _kelasService.GetAllKelasAsync();
                ViewBag.Kelas = kelasList?.Where(k => k.IsActive == true).ToList() ?? new List<SistemSekolahSMA.Models.Kelas>();

                // Load Guru - sebagai SelectList
                var gurus = await _guruService.GetAllGuruAsync();
                ViewBag.Gurus = gurus?.Where(g => g.IsActive == true)
                    .Select(g => new SelectListItem
                    {
                        Value = g.GuruId.ToString(),
                        Text = g.NamaGuru
                    }).ToList() ?? new List<SelectListItem>();

                // Load Mata Pelajaran - sebagai SelectList
                var mataPelajarans = await _mataPelajaranService.GetAllMataPelajaranAsync();
                ViewBag.MataPelajaran = mataPelajarans?.Where(mp => mp.IsActive == true)
                    .Select(mp => new SelectListItem
                    {
                        Value = mp.MataPelajaranId.ToString(),
                        Text = mp.NamaMapel
                    }).ToList() ?? new List<SelectListItem>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadJadwalOptions error: {ex.Message}");
                ViewBag.Kelas = new List<SistemSekolahSMA.Models.Kelas>();
                ViewBag.Gurus = new List<SelectListItem>();
                ViewBag.MataPelajaran = new List<SelectListItem>();
            }
        }
        private string GetNamaBulan(int bulan)
        {
            string[] namaBulan = { "", "Januari", "Februari", "Maret", "April", "Mei", "Juni",
                          "Juli", "Agustus", "September", "Oktober", "November", "Desember" };
            return namaBulan[bulan];
        }
        private string GenerateHTMLForPDF(IEnumerable<dynamic> laporanSiswa, int bulan, int tahun)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head>");
            html.AppendLine("<meta charset='utf-8' />");
            html.AppendLine("<title>Laporan Presensi</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; font-size: 12px; }");
            html.AppendLine(".header { text-align: center; margin-bottom: 30px; }");
            html.AppendLine(".header h1 { margin: 0; font-size: 18px; font-weight: bold; }");
            html.AppendLine(".header h2 { margin: 5px 0; font-size: 14px; color: #666; }");
            html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            html.AppendLine("th, td { border: 1px solid #ddd; padding: 6px; text-align: center; font-size: 10px; }");
            html.AppendLine("th { background-color: #f5f5f5; font-weight: bold; }");
            html.AppendLine(".text-left { text-align: left; }");
            html.AppendLine("</style>");
            html.AppendLine("</head><body>");

            html.AppendLine($"<div class='header'>");
            html.AppendLine($"<h1>LAPORAN PRESENSI SISWA</h1>");
            html.AppendLine($"<h2>Bulan: {GetNamaBulan(bulan)} {tahun}</h2>");
            html.AppendLine($"</div>");

            if (laporanSiswa != null && laporanSiswa.Any())
            {
                html.AppendLine("<table>");
                html.AppendLine("<thead>");
                html.AppendLine("<tr>");
                html.AppendLine("<th>No</th><th>NISN</th><th>Nama Siswa</th><th>Kelas</th>");
                html.AppendLine("<th>Hadir</th><th>Izin</th><th>Sakit</th><th>Alpha</th>");
                html.AppendLine("<th>Total</th><th>Persentase</th>");
                html.AppendLine("</tr>");
                html.AppendLine("</thead><tbody>");

                int no = 1;
                foreach (dynamic siswa in laporanSiswa)
                {
                    int hadir = GetSafeInt(siswa.TotalHadir);
                    int izin = GetSafeInt(siswa.TotalIzin);
                    int sakit = GetSafeInt(siswa.TotalSakit);
                    int alpha = GetSafeInt(siswa.TotalAlpha);
                    int totalHari = hadir + izin + sakit + alpha;
                    decimal persentase = totalHari > 0 ? (decimal)hadir / totalHari * 100 : 0;

                    html.AppendLine($"<tr>");
                    html.AppendLine($"<td>{no}</td>");
                    html.AppendLine($"<td>{siswa.NISN ?? "N/A"}</td>");
                    html.AppendLine($"<td class='text-left'>{siswa.NamaSiswa ?? "N/A"}</td>");
                    html.AppendLine($"<td>{siswa.NamaKelas ?? "N/A"}</td>");
                    html.AppendLine($"<td>{hadir}</td>");
                    html.AppendLine($"<td>{izin}</td>");
                    html.AppendLine($"<td>{sakit}</td>");
                    html.AppendLine($"<td>{alpha}</td>");
                    html.AppendLine($"<td>{totalHari}</td>");
                    html.AppendLine($"<td>{persentase:F1}%</td>");
                    html.AppendLine($"</tr>");
                    no++;
                }

                html.AppendLine("</tbody></table>");
            }
            else
            {
                html.AppendLine("<p style='text-align: center; color: #666; margin-top: 50px;'>");
                html.AppendLine("Belum ada data presensi siswa untuk bulan ini");
                html.AppendLine("</p>");
            }

            html.AppendLine("<script>");
            html.AppendLine("window.onload = function() { window.print(); };");
            html.AppendLine("</script>");
            html.AppendLine("</body></html>");
            return html.ToString();
        }
    }
}