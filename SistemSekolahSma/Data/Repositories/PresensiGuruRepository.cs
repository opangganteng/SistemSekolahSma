using Dapper;
using SistemSekolahSMA.Data;
using SistemSekolahSMA.Models;

namespace SistemSekolahSMA.Data.Repositories
{
    public class PresensiGuruRepository : IPresensiGuruRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PresensiGuruRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PresensiGuru> GetByIdAsync(int presensiGuruId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM PresensiGuru WHERE PresensiGuruId = @PresensiGuruId";
            return await connection.QueryFirstOrDefaultAsync<PresensiGuru>(sql,
                new { PresensiGuruId = presensiGuruId });
        }

        public async Task<IEnumerable<PresensiGuru>> GetByGuruIdAsync(int guruId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT pg.*, j.*, k.NamaKelas, mp.NamaMapel FROM PresensiGuru pg
                           INNER JOIN Jadwal j ON pg.JadwalId = j.JadwalId
                           INNER JOIN Kelas k ON j.KelasId = k.KelasId
                           INNER JOIN MataPelajaran mp ON j.MataPelajaranId = mp.MataPelajaranId
                           WHERE pg.GuruId = @GuruId
                           ORDER BY pg.TanggalPresensi DESC";

            return await connection.QueryAsync<PresensiGuru>(sql, new { GuruId = guruId });
        }

        // Implementasi method yang diperlukan interface
        public async Task<IEnumerable<PresensiGuru>> GetByGuruIdAndDateAsync(int guruId, DateTime tanggal)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT pg.*, j.*, k.NamaKelas, mp.NamaMapel FROM PresensiGuru pg
                           INNER JOIN Jadwal j ON pg.JadwalId = j.JadwalId
                           INNER JOIN Kelas k ON j.KelasId = k.KelasId
                           INNER JOIN MataPelajaran mp ON j.MataPelajaranId = mp.MataPelajaranId
                           WHERE pg.GuruId = @GuruId AND pg.TanggalPresensi = @TanggalPresensi
                           ORDER BY pg.TanggalPresensi DESC";

            return await connection.QueryAsync<PresensiGuru>(sql,
                new { GuruId = guruId, TanggalPresensi = tanggal.Date });
        }

        public async Task<IEnumerable<PresensiGuru>> GetByPeriodeAsync(int bulan, int tahun)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT pg.*, j.*, k.NamaKelas, mp.NamaMapel, g.NamaGuru 
                           FROM PresensiGuru pg
                           INNER JOIN Jadwal j ON pg.JadwalId = j.JadwalId
                           INNER JOIN Kelas k ON j.KelasId = k.KelasId
                           INNER JOIN MataPelajaran mp ON j.MataPelajaranId = mp.MataPelajaranId
                           INNER JOIN Guru g ON pg.GuruId = g.GuruId
                           WHERE MONTH(pg.TanggalPresensi) = @Bulan AND YEAR(pg.TanggalPresensi) = @Tahun
                           ORDER BY pg.TanggalPresensi DESC";

            return await connection.QueryAsync<PresensiGuru>(sql, new { Bulan = bulan, Tahun = tahun });
        }

        public async Task<int> CreateAsync(PresensiGuru presensiGuru)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO PresensiGuru (JadwalId, GuruId, TanggalPresensi, StatusKehadiran, 
                           MateriPelajaran, Keterangan, CreatedDate) 
                           VALUES (@JadwalId, @GuruId, @TanggalPresensi, @StatusKehadiran, 
                           @MateriPelajaran, @Keterangan, @CreatedDate);
                           SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.QuerySingleAsync<int>(sql, presensiGuru);
        }

        public async Task<bool> UpdateAsync(PresensiGuru presensiGuru)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE PresensiGuru SET StatusKehadiran = @StatusKehadiran, 
                           MateriPelajaran = @MateriPelajaran, Keterangan = @Keterangan 
                           WHERE PresensiGuruId = @PresensiGuruId";
            var result = await connection.ExecuteAsync(sql, presensiGuru);
            return result > 0;
        }

        public async Task<bool> CreateOrUpdateAsync(PresensiGuru presensi)
        {
            using var connection = _connectionFactory.CreateConnection();

            // Cek apakah sudah ada presensi untuk guru, jadwal, dan tanggal yang sama
            var checkSql = @"SELECT PresensiGuruId FROM PresensiGuru 
                             WHERE GuruId = @GuruId AND JadwalId = @JadwalId 
                             AND TanggalPresensi = @TanggalPresensi";

            var existingId = await connection.QueryFirstOrDefaultAsync<int?>(checkSql, presensi);

            if (existingId.HasValue)
            {
                // Update existing record
                presensi.PresensiGuruId = existingId.Value;
                return await UpdateAsync(presensi);
            }
            else
            {
                // Create new record
                var newId = await CreateAsync(presensi);
                return newId > 0;
            }
        }

        public async Task<IEnumerable<dynamic>> GetLaporanByMonthAsync(int bulan, int tahun)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT 
                           g.NamaGuru,
                           g.NIP,
                           k.NamaKelas,
                           mp.NamaMapel,
                           COUNT(CASE WHEN pg.StatusKehadiran = 'Hadir' THEN 1 END) as TotalHadir,
                           COUNT(CASE WHEN pg.StatusKehadiran = 'Izin' THEN 1 END) as TotalIzin,
                           COUNT(CASE WHEN pg.StatusKehadiran = 'Sakit' THEN 1 END) as TotalSakit,
                           COUNT(CASE WHEN pg.StatusKehadiran = 'Alpha' THEN 1 END) as TotalAlpha,
                           COUNT(*) as TotalPresensi
                       FROM PresensiGuru pg
                       INNER JOIN Guru g ON pg.GuruId = g.GuruId
                       INNER JOIN Jadwal j ON pg.JadwalId = j.JadwalId
                       INNER JOIN Kelas k ON j.KelasId = k.KelasId
                       INNER JOIN MataPelajaran mp ON j.MataPelajaranId = mp.MataPelajaranId
                       WHERE MONTH(pg.TanggalPresensi) = @Bulan AND YEAR(pg.TanggalPresensi) = @Tahun
                       GROUP BY g.GuruId, g.NamaGuru, g.NIP, k.NamaKelas, mp.NamaMapel
                       ORDER BY g.NamaGuru, k.NamaKelas";

            return await connection.QueryAsync(sql, new { Bulan = bulan, Tahun = tahun });
        }

        public async Task<bool> DeleteAsync(int presensiGuruId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM PresensiGuru WHERE PresensiGuruId = @PresensiGuruId";
            var result = await connection.ExecuteAsync(sql, new { PresensiGuruId = presensiGuruId });
            return result > 0;
        }
    }
}