using Dapper;
using SistemSekolahSMA.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemSekolahSMA.Data.Repositories
{
    public class KelasRepository : IKelasRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public KelasRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Kelas> GetByIdAsync(int kelasId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT k.*, g.* FROM Kelas k 
                       LEFT JOIN Guru g ON k.WaliKelas = g.GuruId 
                       WHERE k.KelasId = @KelasId AND k.IsActive = 1";

            var kelas = await connection.QueryAsync<Kelas, Guru, Kelas>(sql,
                (kelas, guru) =>
                {
                    kelas.WaliKelasNavigation = guru;
                    return kelas;
                },
                new { KelasId = kelasId },
                splitOn: "GuruId");

            return kelas.FirstOrDefault();
        }

        public async Task<IEnumerable<Kelas>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT k.*, g.* FROM Kelas k 
                       LEFT JOIN Guru g ON k.WaliKelas = g.GuruId 
                       WHERE k.IsActive = 1 ORDER BY k.Tingkat, k.NamaKelas";

            var kelas = await connection.QueryAsync<Kelas, Guru, Kelas>(sql,
                (kelas, guru) =>
                {
                    kelas.WaliKelasNavigation = guru;
                    return kelas;
                },
                splitOn: "GuruId");

            return kelas;
        }

        public async Task<int> CreateAsync(Kelas kelas)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Kelas (NamaKelas, Tingkat, Jurusan, WaliKelas, IsActive, CreatedDate) 
                       VALUES (@NamaKelas, @Tingkat, @Jurusan, @WaliKelas, @IsActive, @CreatedDate);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.QuerySingleAsync<int>(sql, kelas);
        }

        public async Task<bool> UpdateAsync(Kelas kelas)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Kelas SET NamaKelas = @NamaKelas, Tingkat = @Tingkat, 
                       Jurusan = @Jurusan, WaliKelas = @WaliKelas, IsActive = @IsActive 
                       WHERE KelasId = @KelasId";
            var result = await connection.ExecuteAsync(sql, kelas);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int kelasId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE Kelas SET IsActive = 0 WHERE KelasId = @KelasId";
            var result = await connection.ExecuteAsync(sql, new { KelasId = kelasId });
            return result > 0;
        }

        public async Task<bool> NamaKelasExistsAsync(string namaKelas)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT COUNT(1) FROM Kelas WHERE NamaKelas = @NamaKelas AND IsActive = 1";
            var count = await connection.QuerySingleAsync<int>(sql, new { NamaKelas = namaKelas });
            return count > 0;
        }
    }
}