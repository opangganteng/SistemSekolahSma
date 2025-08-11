using SistemSekolahSMA.Models;

public interface IMataPelajaranRepository
{
    Task<MataPelajaran> GetByIdAsync(int mataPelajaranId);
    Task<IEnumerable<MataPelajaran>> GetAllAsync();
    Task<int> CreateAsync(MataPelajaran mataPelajaran);
    Task<bool> UpdateAsync(MataPelajaran mataPelajaran);
    Task<bool> DeleteAsync(int mataPelajaranId);
    Task<bool> KodeMapelExistsAsync(string kodeMapel);
}