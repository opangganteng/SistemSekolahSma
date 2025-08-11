using SistemSekolahSMA.Models;

public interface IGuruRepository
{
    Task<Guru> GetByIdAsync(int guruId);
    Task<Guru> GetByUserIdAsync(int userId);
    Task<IEnumerable<Guru>> GetAllAsync();
    Task<int> CreateAsync(Guru guru);
    Task<bool> UpdateAsync(Guru guru);
    Task<bool> DeleteAsync(int guruId);
    Task<bool> NIPExistsAsync(string nip);
}