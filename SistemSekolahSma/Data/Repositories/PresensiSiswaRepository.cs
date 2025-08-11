using Dapper;
using SistemSekolahSMA.Data;
using SistemSekolahSMA.Models;

namespace SistemSekolahSMA.Data.Repositories
{
    public class PresensiSiswaRepository : IPresensiSiswaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PresensiSiswaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PresensiSiswa> GetByIdAsync(int presensiSiswaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM PresensiSiswa WHERE PresensiSiswaId = @PresensiSiswaId";
            return await connection.QueryFirstOrDefaultAsync<PresensiSiswa>(sql,
                new { PresensiSiswaId = presensiSiswaId });
        }

        public async Task<IEnumerable<PresensiSiswa>> GetByJadwalAndTanggalAsync(int jadwalId, DateTime tanggal)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT ps.*, s.NamaSiswa, s.NISN FROM PresensiSiswa ps
                           INNER JOIN Siswa s ON ps.SiswaId = s.SiswaId
                           WHERE ps.JadwalId = @JadwalId AND ps.TanggalPresensi = @TanggalPresensi
                           ORDER BY s.NamaSiswa";

            return await connection.QueryAsync<PresensiSiswa, Siswa, PresensiSiswa>(sql,
                (presensi, siswa) =>
                {
                    presensi.Siswa = siswa;
                    return presensi;
                },
                new { JadwalId = jadwalId, TanggalPresensi = tanggal.Date },
                splitOn: "NamaSiswa");
        }

        // Implementasi method yang diperlukan interface
        public async Task<IEnumerable<PresensiSiswa>> GetByJadwalAndDateAsync(int jadwalId, DateTime tanggal)
        {
            return await GetByJadwalAndTanggalAsync(jadwalId, tanggal);
        }

        public async Task<IEnumerable<PresensiSiswa>> GetByPeriodeAsync(int bulan, int tahun)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT ps.*, s.NamaSiswa, s.NISN, j.*, k.NamaKelas, mp.NamaMapel 
                           FROM PresensiSiswa ps
                           INNER JOIN Siswa s ON ps.SiswaId = s.SiswaId
                           INNER JOIN Jadwal j ON ps.JadwalId = j.JadwalId
                           INNER JOIN Kelas k ON j.KelasId = k.KelasId
                           INNER JOIN MataPelajaran mp ON j.MataPelajaranId = mp.MataPelajaranId
                           WHERE MONTH(ps.TanggalPresensi) = @Bulan AND YEAR(ps.TanggalPresensi) = @Tahun
                           ORDER BY ps.TanggalPresensi, s.NamaSiswa";

            return await connection.QueryAsync<PresensiSiswa>(sql, new { Bulan = bulan, Tahun = tahun });
        }

        public async Task<int> CreateAsync(PresensiSiswa presensiSiswa)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO PresensiSiswa (JadwalId, SiswaId, TanggalPresensi, StatusKehadiran, 
                           Keterangan, DibuatOleh, CreatedDate) 
                           VALUES (@JadwalId, @SiswaId, @TanggalPresensi, @StatusKehadiran, 
                           @Keterangan, @DibuatOleh, @CreatedDate);
                           SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.QuerySingleAsync<int>(sql, presensiSiswa);
        }

        public async Task<bool> UpdateAsync(PresensiSiswa presensiSiswa)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE PresensiSiswa SET StatusKehadiran = @StatusKehadiran, 
                           Keterangan = @Keterangan WHERE PresensiSiswaId = @PresensiSiswaId";
            var result = await connection.ExecuteAsync(sql, presensiSiswa);
            return result > 0;
        }

        public async Task<bool> CreateOrUpdateAsync(PresensiSiswa presensi)
        {
            using var connection = _connectionFactory.CreateConnection();

            // Cek apakah sudah ada presensi untuk siswa, jadwal, dan tanggal yang sama
            var checkSql = @"SELECT PresensiSiswaId FROM PresensiSiswa 
                             WHERE SiswaId = @SiswaId AND JadwalId = @JadwalId 
                             AND TanggalPresensi = @TanggalPresensi";

            var existingId = await connection.QueryFirstOrDefaultAsync<int?>(checkSql, presensi);

            if (existingId.HasValue)
            {
                // Update existing record
                presensi.PresensiSiswaId = existingId.Value;
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
                           s.NamaSiswa,
                           s.NISN,
                           k.NamaKelas,
                           mp.NamaMapel,
                           COUNT(CASE WHEN ps.StatusKehadiran = 'Hadir' THEN 1 END) as TotalHadir,
                           COUNT(CASE WHEN ps.StatusKehadiran = 'Izin' THEN 1 END) as TotalIzin,
                           COUNT(CASE WHEN ps.StatusKehadiran = 'Sakit' THEN 1 END) as TotalSakit,
                           COUNT(CASE WHEN ps.StatusKehadiran = 'Alpha' THEN 1 END) as TotalAlpha,
                           COUNT(*) as TotalPresensi
                       FROM PresensiSiswa ps
                       INNER JOIN Siswa s ON ps.SiswaId = s.SiswaId
                       INNER JOIN Jadwal j ON ps.JadwalId = j.JadwalId
                       INNER JOIN Kelas k ON j.KelasId = k.KelasId
                       INNER JOIN MataPelajaran mp ON j.MataPelajaranId = mp.MataPelajaranId
                       WHERE MONTH(ps.TanggalPresensi) = @Bulan AND YEAR(ps.TanggalPresensi) = @Tahun
                       GROUP BY s.SiswaId, s.NamaSiswa, s.NISN, k.NamaKelas, mp.NamaMapel
                       ORDER BY k.NamaKelas, s.NamaSiswa";

            return await connection.QueryAsync(sql, new { Bulan = bulan, Tahun = tahun });
        }

        public async Task<bool> DeleteAsync(int presensiSiswaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM PresensiSiswa WHERE PresensiSiswaId = @PresensiSiswaId";
            var result = await connection.ExecuteAsync(sql, new { PresensiSiswaId = presensiSiswaId });
            return result > 0;
        }

        public async Task<bool> SaveBulkAsync(List<PresensiSiswa> presensiList)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Hapus presensi yang sudah ada untuk jadwal dan tanggal yang sama
                var deleteExistingSql = @"DELETE FROM PresensiSiswa 
                                             WHERE JadwalId = @JadwalId AND TanggalPresensi = @TanggalPresensi";

                if (presensiList.Any())
                {
                    await connection.ExecuteAsync(deleteExistingSql,
                        new
                        {
                            JadwalId = presensiList.First().JadwalId,
                            TanggalPresensi = presensiList.First().TanggalPresensi.Date
                        }, transaction);
                }

                // Insert presensi baru
                var insertSql = @"INSERT INTO PresensiSiswa (JadwalId, SiswaId, TanggalPresensi, 
                                     StatusKehadiran, Keterangan, DibuatOleh, CreatedDate) 
                                     VALUES (@JadwalId, @SiswaId, @TanggalPresensi, @StatusKehadiran, 
                                     @Keterangan, @DibuatOleh, @CreatedDate)";

                await connection.ExecuteAsync(insertSql, presensiList, transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}