using Dapper;
using SistemSekolahSMA.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemSekolahSMA.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT * FROM Users 
                WHERE (Username = @Username OR Email = @Username) 
                AND IsActive = 1";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Users WHERE UserId = @UserId";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Users WHERE IsActive = 1 ORDER BY Username";
            return await connection.QueryAsync<User>(sql);
        }

        public async Task<int> CreateAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Users (Username, Password, Email, Role, IsActive, CreatedDate, CreatedBy) 
                       VALUES (@Username, @Password, @Email, @Role, @IsActive, @CreatedDate, @CreatedBy);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.QuerySingleAsync<int>(sql, user);
        }

        public async Task<bool> UpdateAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Users SET Username = @Username, Password = @Password, Email = @Email, 
                       Role = @Role, IsActive = @IsActive WHERE UserId = @UserId";
            var result = await connection.ExecuteAsync(sql, user);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE Users SET IsActive = 0 WHERE UserId = @UserId";
            var result = await connection.ExecuteAsync(sql, new { UserId = userId });
            return result > 0;
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND IsActive = 1";
            var count = await connection.QuerySingleAsync<int>(sql, new { Username = username });
            return count > 0;
        }
    }
}