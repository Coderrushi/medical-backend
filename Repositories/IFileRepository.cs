using Azure.Core.GeoJson;
using medical_record_system_backend.Models;

namespace medical_record_system_backend.Repositories
{
    public interface IFileRepository
    {
        Task<MedicalFile> SaveFileAsync(MedicalFile file);
        Task<List<MedicalFile>> GetFilesByUserAsync(int userId);
        Task<MedicalFile?> GetFileByIdAsync(int id);
        Task<bool> DeleteFileAsync(MedicalFile file);

        Task<MedicalFile?> GetByIdAsync(int id, int userId);
    }
}
