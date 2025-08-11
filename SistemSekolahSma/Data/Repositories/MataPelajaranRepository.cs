using Dapper;
using SistemSekolahSMA.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemSekolahSMA.Data.Repositories
{
    public class MataPelajaranRepository : IMataPelajaranRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MataPelajaranRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<MataPelajaran> GetByIdAsync(int mataPelajaranId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM MataPelajaran WHERE MataPelajaranId = @MataPelajaranId AND IsActive = 1";
            return await connection.QueryFirstOrDefaultAsync<MataPelajaran>(sql,new { MataPelajaranId = mataPelajaranId });
        }

        public async Task<IEnumerable<MataPelajaran>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM MataPelajaran WHERE IsActive = 1 ORDER BY NamaMapel";
            return await connection.QueryAsync<MataPelajaran>(sql);
        }

        public async Task<int> CreateAsync(MataPelajaran mataPelajaran)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO MataPelajaran (KodeMapel, NamaMapel, Kategori, IsActive, CreatedDate) 
                       VALUES (@KodeMapel, @NamaMapel, @Kategori, @IsActive, @CreatedDate);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.QuerySingleAsync<int>(sql, mataPelajaran);
        }

        public async Task<bool> UpdateAsync(MataPelajaran mataPelajaran)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE MataPelajaran SET KodeMapel = @KodeMapel, NamaMapel = @NamaMapel, 
                       Kategori = @Kategori, IsActive = @IsActive WHERE MataPelajaranId = @MataPelajaranId";
            var result = await connection.ExecuteAsync(sql, mataPelajaran);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int mataPelajaranId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE MataPelajaran SET IsActive = 0 WHERE MataPelajaranId = @MataPelajaranId";
            var result = await connection.ExecuteAsync(sql, new { MataPelajaranId = mataPelajaranId });
            return result > 0;
        }

        public async Task<bool> KodeMapelExistsAsync(string kodeMapel)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT COUNT(1) FROM MataPelajaran WHERE KodeMapel = @KodeMapel AND IsActive = 1";
            var count = await connection.QuerySingleAsync<int>(sql, new { KodeMapel = kodeMapel });
            return count > 0;
        }
    }
}