using medical_record_system_backend.Models;

namespace medical_record_system_backend.Services
{
    public interface IFileService
    {
        Task<MedicalFile> SaveFileAsync(IFormFile file, int userId, string fileType, string displayName, string contentType);
        Task<List<MedicalFile>> GetUserFilesAsync(int userId);
        Task<MedicalFile?> GetFileByIdAsync(int fileId);
        Task<bool> DeleteFileAsync(int fileId, int userId);
        

    }
}
