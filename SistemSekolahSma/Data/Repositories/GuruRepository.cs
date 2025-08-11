// Data/Repositories/GuruRepository.cs
using Dapper;
using SistemSekolahSMA.Models;
using System.Data;

namespace SistemSekolahSMA.Data.Repositories
{
    public class GuruRepository : IGuruRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GuruRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Guru> GetByIdAsync(int guruId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Guru WHERE GuruId = @GuruId";
            return await connection.QueryFirstOrDefaultAsync<Guru>(sql, new { GuruId = guruId });
        }

        public async Task<Guru> GetByUserIdAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Guru WHERE UserId = @UserId";
            return await connection.QueryFirstOrDefaultAsync<Guru>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<Guru>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Guru WHERE IsActive = 1 ORDER BY NamaGuru";
            return await connection.QueryAsync<Guru>(sql);
        }

        public async Task<int> CreateAsync(Guru guru)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                System.Diagnostics.Debug.WriteLine("=== GuruRepository.CreateAsync ===");
                System.Diagnostics.Debug.WriteLine($"Connection: {connection?.State}");

                var sql = @"INSERT INTO Guru (UserId, NIP, NamaGuru, TempatLahir, TanggalLahir, JenisKelamin, Alamat, NoTelepon, IsActive, CreatedDate) 
                           VALUES (@UserId, @NIP, @NamaGuru, @TempatLahir, @TanggalLahir, @JenisKelamin, @Alamat, @NoTelepon, @IsActive, @CreatedDate);
                           SELECT CAST(SCOPE_IDENTITY() as int)";

                System.Diagnostics.Debug.WriteLine($"SQL: {sql}");
                System.Diagnostics.Debug.WriteLine($"Parameters: UserId={guru.UserId}, NIP={guru.NIP}, NamaGuru={guru.NamaGuru}");

                var result = await connection.QuerySingleAsync<int>(sql, new
                {
                    UserId = guru.UserId,
                    NIP = guru.NIP,
                    NamaGuru = guru.NamaGuru,
                    TempatLahir = guru.TempatLahir,
                    TanggalLahir = guru.TanggalLahir,
                    JenisKelamin = guru.JenisKelamin,
                    Alamat = guru.Alamat,
                    NoTelepon = guru.NoTelepon,
                    IsActive = guru.IsActive,
                    CreatedDate = guru.CreatedDate
                });

                System.Diagnostics.Debug.WriteLine($"Insert result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GuruRepository.CreateAsync error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Guru guru)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Guru SET 
                       UserId = @UserId, 
                       NIP = @NIP, 
                       NamaGuru = @NamaGuru, 
                       TempatLahir = @TempatLahir, 
                       TanggalLahir = @TanggalLahir, 
                       JenisKelamin = @JenisKelamin, 
                       Alamat = @Alamat, 
                       NoTelepon = @NoTelepon, 
                       IsActive = @IsActive 
                       WHERE GuruId = @GuruId";

            var result = await connection.ExecuteAsync(sql, guru);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int guruId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE Guru SET IsActive = 0 WHERE GuruId = @GuruId";
            var result = await connection.ExecuteAsync(sql, new { GuruId = guruId });
            return result > 0;
        }

        public async Task<bool> NIPExistsAsync(string nip)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT COUNT(1) FROM Guru WHERE NIP = @NIP AND IsActive = 1";
            var count = await connection.QuerySingleAsync<int>(sql, new { NIP = nip });
            return count > 0;
        }
    }
}