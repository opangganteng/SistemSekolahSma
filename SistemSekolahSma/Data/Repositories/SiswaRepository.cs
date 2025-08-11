using Dapper;
using SistemSekolahSMA.Data;
using SistemSekolahSMA.Models;

namespace SistemSekolahSMA.Data.Repositories
{
    public class SiswaRepository : ISiswaRepository  // Harus ISiswaRepository, bukan SiswaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SiswaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Siswa> GetByIdAsync(int siswaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT s.*, k.* FROM Siswa s 
                           INNER JOIN Kelas k ON s.KelasId = k.KelasId 
                           WHERE s.SiswaId = @SiswaId AND s.IsActive = 1";

            var siswa = await connection.QueryAsync<Siswa, Kelas, Siswa>(sql,
                (siswa, kelas) =>
                {
                    siswa.Kelas = kelas;
                    return siswa;
                },
                new { SiswaId = siswaId },
                splitOn: "KelasId");

            return siswa.FirstOrDefault();
        }

        public async Task<IEnumerable<Siswa>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT s.*, k.* FROM Siswa s 
                           INNER JOIN Kelas k ON s.KelasId = k.KelasId 
                           WHERE s.IsActive = 1 ORDER BY s.NamaSiswa";

            var siswas = await connection.QueryAsync<Siswa, Kelas, Siswa>(sql,
                (siswa, kelas) =>
                {
                    siswa.Kelas = kelas;
                    return siswa;
                },
                splitOn: "KelasId");

            return siswas;
        }

        public async Task<IEnumerable<Siswa>> GetByKelasIdAsync(int kelasId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT s.*, k.* FROM Siswa s 
                           INNER JOIN Kelas k ON s.KelasId = k.KelasId 
                           WHERE s.KelasId = @KelasId AND s.IsActive = 1 ORDER BY s.NamaSiswa";

            var siswas = await connection.QueryAsync<Siswa, Kelas, Siswa>(sql,
                (siswa, kelas) =>
                {
                    siswa.Kelas = kelas;
                    return siswa;
                },
                new { KelasId = kelasId },
                splitOn: "KelasId");

            return siswas;
        }

        public async Task<IEnumerable<Siswa>> SearchByNameAsync(string name)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT s.*, k.* FROM Siswa s 
                           INNER JOIN Kelas k ON s.KelasId = k.KelasId 
                           WHERE s.NamaSiswa LIKE @Name AND s.IsActive = 1 ORDER BY s.NamaSiswa";

            var siswas = await connection.QueryAsync<Siswa, Kelas, Siswa>(sql,
                (siswa, kelas) =>
                {
                    siswa.Kelas = kelas;
                    return siswa;
                },
                new { Name = $"%{name}%" },
                splitOn: "KelasId");

            return siswas;
        }

        public async Task<int> CreateAsync(Siswa siswa)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Siswa (NISN, NamaSiswa, KelasId, TempatLahir, TanggalLahir, 
                           JenisKelamin, Alamat, NoTelepon, NamaOrtu, IsActive, CreatedDate) 
                           VALUES (@NISN, @NamaSiswa, @KelasId, @TempatLahir, @TanggalLahir, 
                           @JenisKelamin, @Alamat, @NoTelepon, @NamaOrtu, @IsActive, @CreatedDate);
                           SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.QuerySingleAsync<int>(sql, siswa);
        }

        public async Task<bool> UpdateAsync(Siswa siswa)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Siswa SET NISN = @NISN, NamaSiswa = @NamaSiswa, KelasId = @KelasId, 
                           TempatLahir = @TempatLahir, TanggalLahir = @TanggalLahir, 
                           JenisKelamin = @JenisKelamin, Alamat = @Alamat, NoTelepon = @NoTelepon, 
                           NamaOrtu = @NamaOrtu, IsActive = @IsActive WHERE SiswaId = @SiswaId";
            var result = await connection.ExecuteAsync(sql, siswa);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int siswaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE Siswa SET IsActive = 0 WHERE SiswaId = @SiswaId";
            var result = await connection.ExecuteAsync(sql, new { SiswaId = siswaId });
            return result > 0;
        }

        public async Task<bool> NISNExistsAsync(string nisn)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT COUNT(1) FROM Siswa WHERE NISN = @NISN AND IsActive = 1";
            var count = await connection.QuerySingleAsync<int>(sql, new { NISN = nisn });
            return count > 0;
        }
    }
}