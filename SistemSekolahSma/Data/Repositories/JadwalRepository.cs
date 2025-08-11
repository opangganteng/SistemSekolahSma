using Dapper;
using SistemSekolahSMA.Data;
using SistemSekolahSMA.Models;
using SistemSekolahSMA.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemSekolahSMA.Data.Repositories
{
    public class JadwalRepository : IJadwalRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public JadwalRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Jadwal> GetByIdAsync(int jadwalId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT j.*, k.*, mp.*, g.* 
                       FROM Jadwal j
                       INNER JOIN Kelas k ON j.KelasId = k.KelasId
                       INNER JOIN MataPelajaran mp ON j.MataPelajaranId = mp.MataPelajaranId
                       INNER JOIN Guru g ON j.GuruId = g.GuruId
                       WHERE j.JadwalId = @JadwalId AND j.IsActive = 1";

            var result = await connection.QueryAsync<Jadwal, Kelas, MataPelajaran, Guru, Jadwal>(sql,
                (jadwal, kelas, mataPelajaran, guru) =>
                {
                    jadwal.Kelas = kelas;
                    jadwal.MataPelajaran = mataPelajaran;
                    jadwal.Guru = guru;
                    return jadwal;
                },
                new { JadwalId = jadwalId },
                splitOn: "KelasId,MataPelajaranId,GuruId");

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<Jadwal>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT j.*, k.*, mp.*, g.* FROM Jadwal j 
                       INNER JOIN Kelas k ON j.KelasId = k.KelasId 
                       INNER JOIN MataPelajaran mp ON j.MataPelajaranId = mp.MataPelajaranId
                       INNER JOIN Guru g ON j.GuruId = g.GuruId
                       WHERE j.IsActive = 1 ORDER BY j.Hari, j.JamMulai";

            var jadwals = await connection.QueryAsync<Jadwal, Kelas, MataPelajaran, Guru, Jadwal>(sql,
                (jadwal, kelas, mataPelajaran, guru) =>
                {
                    jadwal.Kelas = kelas;
                    jadwal.MataPelajaran = mataPelajaran;
                    jadwal.Guru = guru;
                    return jadwal;
                },
                splitOn: "KelasId,MataPelajaranId,GuruId");

            return jadwals;
        }

        public async Task<IEnumerable<Jadwal>> GetByGuruIdAsync(int guruId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT j.*, k.*, mp.*, g.* 
                       FROM Jadwal j
                       INNER JOIN Kelas k ON j.KelasId = k.KelasId
                       INNER JOIN MataPelajaran mp ON j.MataPelajaranId = mp.MataPelajaranId
                       INNER JOIN Guru g ON j.GuruId = g.GuruId
                       WHERE j.GuruId = @GuruId AND j.IsActive = 1
                       ORDER BY 
                           CASE j.Hari 
                               WHEN 'Senin' THEN 1
                               WHEN 'Selasa' THEN 2
                               WHEN 'Rabu' THEN 3
                               WHEN 'Kamis' THEN 4
                               WHEN 'Jumat' THEN 5
                               WHEN 'Sabtu' THEN 6
                               WHEN 'Minggu' THEN 7
                               ELSE 8
                           END, j.JamMulai";

            var result = await connection.QueryAsync<Jadwal, Kelas, MataPelajaran, Guru, Jadwal>(sql,
                (jadwal, kelas, mataPelajaran, guru) =>
                {
                    jadwal.Kelas = kelas;
                    jadwal.MataPelajaran = mataPelajaran;
                    jadwal.Guru = guru;
                    return jadwal;
                },
                new { GuruId = guruId },
                splitOn: "KelasId,MataPelajaranId,GuruId");

            return result;
        }

        public async Task<IEnumerable<Jadwal>> GetByKelasIdAsync(int kelasId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT j.*, k.*, mp.*, g.* FROM Jadwal j 
                       INNER JOIN Kelas k ON j.KelasId = k.KelasId 
                       INNER JOIN MataPelajaran mp ON j.MataPelajaranId = mp.MataPelajaranId
                       INNER JOIN Guru g ON j.GuruId = g.GuruId
                       WHERE j.KelasId = @KelasId AND j.IsActive = 1 ORDER BY j.Hari, j.JamMulai";

            var jadwals = await connection.QueryAsync<Jadwal, Kelas, MataPelajaran, Guru, Jadwal>(sql,
                (jadwal, kelas, mataPelajaran, guru) =>
                {
                    jadwal.Kelas = kelas;
                    jadwal.MataPelajaran = mataPelajaran;
                    jadwal.Guru = guru;
                    return jadwal;
                },
                new { KelasId = kelasId },
                splitOn: "KelasId,MataPelajaranId,GuruId");

            return jadwals;
        }

        public async Task<int> CreateAsync(Jadwal jadwal)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Jadwal (KelasId, MataPelajaranId, GuruId, Hari, JamMulai, JamSelesai, 
                       Ruangan, TahunAjaran, Semester, IsActive, CreatedDate) 
                       VALUES (@KelasId, @MataPelajaranId, @GuruId, @Hari, @JamMulai, @JamSelesai, 
                       @Ruangan, @TahunAjaran, @Semester, @IsActive, @CreatedDate);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.QuerySingleAsync<int>(sql, jadwal);
        }

        public async Task<bool> UpdateAsync(Jadwal jadwal)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Jadwal SET KelasId = @KelasId, MataPelajaranId = @MataPelajaranId, 
                       GuruId = @GuruId, Hari = @Hari, JamMulai = @JamMulai, JamSelesai = @JamSelesai, 
                       Ruangan = @Ruangan, TahunAjaran = @TahunAjaran, Semester = @Semester, 
                       IsActive = @IsActive WHERE JadwalId = @JadwalId";
            var result = await connection.ExecuteAsync(sql, jadwal);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int jadwalId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE Jadwal SET IsActive = 0 WHERE JadwalId = @JadwalId";
            var result = await connection.ExecuteAsync(sql, new { JadwalId = jadwalId });
            return result > 0;
        }

        // METHOD YANG DIPERBAIKI - TANPA FORMAT!
        public async Task<IEnumerable<JadwalKelas>> SearchJadwalByKelasAsync(string namaKelas)
        {
            using var connection = _connectionFactory.CreateConnection();

            // Query TANPA formatting - biarkan C# yang handle
            var sql = @"SELECT j.JadwalId, 
                               k.NamaKelas, 
                               j.Hari, 
                               j.JamMulai,
                               j.JamSelesai,
                               mp.NamaMapel, 
                               g.NamaGuru, 
                               j.Ruangan
                        FROM Jadwal j 
                        INNER JOIN Kelas k ON j.KelasId = k.KelasId 
                        INNER JOIN MataPelajaran mp ON j.MataPelajaranId = mp.MataPelajaranId
                        INNER JOIN Guru g ON j.GuruId = g.GuruId
                        WHERE UPPER(k.NamaKelas) LIKE UPPER(@NamaKelas) AND j.IsActive = 1 
                        ORDER BY 
                            CASE j.Hari 
                                WHEN 'Senin' THEN 1
                                WHEN 'Selasa' THEN 2
                                WHEN 'Rabu' THEN 3
                                WHEN 'Kamis' THEN 4
                                WHEN 'Jumat' THEN 5
                                WHEN 'Sabtu' THEN 6
                                WHEN 'Minggu' THEN 7
                                ELSE 8
                            END, j.JamMulai";

            try
            {
                System.Diagnostics.Debug.WriteLine($"SearchJadwalByKelasAsync called with: '{namaKelas}'");
                System.Diagnostics.Debug.WriteLine($"SQL Query: {sql}");
                System.Diagnostics.Debug.WriteLine($"Parameter: {$"%{namaKelas}%"}");

                var result = await connection.QueryAsync<JadwalKelas>(sql, new { NamaKelas = $"%{namaKelas}%" });

                System.Diagnostics.Debug.WriteLine($"SearchJadwalByKelasAsync found {result.Count()} results");

                foreach (var item in result)
                {
                    System.Diagnostics.Debug.WriteLine($"Found: {item.NamaKelas} - {item.Hari} - {item.NamaMapel}");
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SearchJadwalByKelasAsync error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}