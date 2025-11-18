using medical_record_system_backend.Models;
using medical_record_system_backend.Repositories;

namespace medical_record_system_backend.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _files;
        private readonly IWebHostEnvironment _env;

        public FileService(IFileRepository files, IWebHostEnvironment env)
        {
            _files = files;
            _env = env;
        }

        public async Task<MedicalFile> SaveFileAsync(IFormFile file, int userId, string fileType, string displayName, string contentType)
        {
            var uploadsDir = Path.Combine("uploads", userId.ToString());
            Directory.CreateDirectory(Path.Combine(_env.WebRootPath, uploadsDir));

            var storedFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(_env.WebRootPath, uploadsDir, storedFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine(uploadsDir, storedFileName).Replace("\\", "/");

            var record = new MedicalFile
            {
                FileName = file.FileName,
                DisplayName = displayName,
                FileType = fileType,
                FilePath = relativePath,
                FileSize = file.Length,
                UserId = userId,
                ContentType = file.ContentType
            };

            return await _files.SaveFileAsync(record);
        }

        public async Task<List<MedicalFile>> GetUserFilesAsync(int userId)
        {
            return await _files.GetFilesByUserAsync(userId);
        }

        public async Task<MedicalFile?> GetFileByIdAsync(int fileId)
        {
            return await _files.GetFileByIdAsync(fileId);
        }

        public async Task<bool> DeleteFileAsync(int fileId, int userId)
        {
            var file = await _files.GetFileByIdAsync(fileId);
            if (file == null || file.UserId != userId) return false;

            var fullPath = Path.Combine(_env.WebRootPath, "uploads", file.FilePath);
            if (File.Exists(fullPath)) File.Delete(fullPath);

            await _files.DeleteFileAsync(file);
            return true;
        }

        
    }
}
