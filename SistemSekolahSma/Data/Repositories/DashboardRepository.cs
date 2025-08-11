using Dapper;
using SistemSekolahSMA.Data;
using SistemSekolahSMA.Models;

namespace SistemSekolahSMA.Data.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DashboardRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Dashboard Statistics
        public async Task<int> GetTotalSiswaAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT COUNT(*) FROM Siswa WHERE IsActive = 1";
            return await connection.QuerySingleAsync<int>(sql);
        }

        public async Task<int> GetSiswaAktifAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT COUNT(*) FROM Siswa WHERE IsActive = 1";
            return await connection.QuerySingleAsync<int>(sql);
        }

        public async Task<int> GetTotalGuruAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT COUNT(*) FROM Guru WHERE IsActive = 1";
            return await connection.QuerySingleAsync<int>(sql);
        }

        public async Task<int> GetTotalKelasAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT COUNT(*) FROM Kelas WHERE IsActive = 1";
            return await connection.QuerySingleAsync<int>(sql);
        }

        public async Task<int> GetTotalMataPelajaranAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT COUNT(*) FROM MataPelajaran WHERE IsActive = 1";
            return await connection.QuerySingleAsync<int>(sql);
        }

        // Chart Data
        public async Task<Dictionary<string, int>> GetDistribusiSiswaPerKelasAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT k.NamaKelas, COUNT(s.SiswaId) as JumlahSiswa 
                       FROM Kelas k 
                       LEFT JOIN Siswa s ON k.KelasId = s.KelasId AND s.IsActive = 1
                       WHERE k.IsActive = 1
                       GROUP BY k.NamaKelas, k.Tingkat
                       ORDER BY k.Tingkat, k.NamaKelas";

            var result = await connection.QueryAsync(sql);
            return result.ToDictionary(x => (string)x.NamaKelas, x => (int)x.JumlahSiswa);
        }

        public async Task<Dictionary<string, int>> GetDistribusiSiswaPerTingkatAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT 
                           CASE k.Tingkat
                               WHEN 10 THEN 'Kelas X'
                               WHEN 11 THEN 'Kelas XI' 
                               WHEN 12 THEN 'Kelas XII'
                               ELSE 'Lainnya'
                           END as TingkatNama,
                           COUNT(s.SiswaId) as JumlahSiswa
                       FROM Kelas k 
                       LEFT JOIN Siswa s ON k.KelasId = s.KelasId AND s.IsActive = 1
                       WHERE k.IsActive = 1
                       GROUP BY k.Tingkat
                       ORDER BY k.Tingkat";

            var result = await connection.QueryAsync(sql);
            return result.ToDictionary(x => (string)x.TingkatNama, x => (int)x.JumlahSiswa);
        }

        public async Task<Dictionary<string, int>> GetDistribusiSiswaPerGenderAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT 
                           CASE 
                               WHEN JenisKelamin = 'L' OR JenisKelamin = 'Laki-laki' THEN 'Laki-laki'
                               WHEN JenisKelamin = 'P' OR JenisKelamin = 'Perempuan' THEN 'Perempuan'
                               ELSE 'Tidak Diketahui'
                           END as Gender,
                           COUNT(*) as Jumlah
                       FROM Siswa 
                       WHERE IsActive = 1 AND JenisKelamin IS NOT NULL
                       GROUP BY 
                           CASE 
                               WHEN JenisKelamin = 'L' OR JenisKelamin = 'Laki-laki' THEN 'Laki-laki'
                               WHEN JenisKelamin = 'P' OR JenisKelamin = 'Perempuan' THEN 'Perempuan'
                               ELSE 'Tidak Diketahui'
                           END";

            var result = await connection.QueryAsync(sql);
            return result.ToDictionary(x => (string)x.Gender, x => (int)x.Jumlah);
        }

        // Search Functions
        public async Task<IEnumerable<Siswa>> SearchSiswaByNameOrNISNAsync(string searchTerm)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT s.*, k.* 
                       FROM Siswa s 
                       INNER JOIN Kelas k ON s.KelasId = k.KelasId 
                       WHERE s.IsActive = 1 
                       AND (s.NamaSiswa LIKE @SearchTerm OR s.NISN LIKE @SearchTerm)
                       ORDER BY s.NamaSiswa";

            var siswas = await connection.QueryAsync<Siswa, Kelas, Siswa>(sql,
                (siswa, kelas) =>
                {
                    siswa.Kelas = kelas;
                    return siswa;
                },
                new { SearchTerm = $"%{searchTerm}%" },
                splitOn: "KelasId");

            return siswas;
        }

        public async Task<IEnumerable<Jadwal>> SearchJadwalByKelasAsync(string kelasName)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT j.*, k.*, mp.*, g.*
                       FROM Jadwal j
                       INNER JOIN Kelas k ON j.KelasId = k.KelasId
                       INNER JOIN MataPelajaran mp ON j.MataPelajaranId = mp.MataPelajaranId
                       INNER JOIN Guru g ON j.GuruId = g.GuruId
                       WHERE j.IsActive = 1 
                       AND k.IsActive = 1 
                       AND k.NamaKelas LIKE @KelasName
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
                           END,
                           j.JamMulai";

            var jadwals = await connection.QueryAsync<Jadwal, Kelas, MataPelajaran, Guru, Jadwal>(sql,
                (jadwal, kelas, mataPelajaran, guru) =>
                {
                    jadwal.Kelas = kelas;
                    jadwal.MataPelajaran = mataPelajaran;
                    jadwal.Guru = guru;
                    return jadwal;
                },
                new { KelasName = $"%{kelasName}%" },
                splitOn: "KelasId,MataPelajaranId,GuruId");

            return jadwals;
        }
    }
}