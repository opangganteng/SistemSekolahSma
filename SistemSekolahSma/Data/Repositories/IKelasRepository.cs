using SistemSekolahSMA.Models;

public interface IKelasRepository
{
    Task<Kelas> GetByIdAsync(int kelasId);
    Task<IEnumerable<Kelas>> GetAllAsync();
    Task<int> CreateAsync(Kelas kelas);
    Task<bool> UpdateAsync(Kelas kelas);
    Task<bool> DeleteAsync(int kelasId);
    Task<bool> NamaKelasExistsAsync(string namaKelas);
}