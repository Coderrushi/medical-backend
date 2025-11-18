using medical_record_system_backend.Data;
using medical_record_system_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace medical_record_system_backend.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly AppDbContext _db;

        public FileRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MedicalFile> SaveFileAsync(MedicalFile file)
        {
            _db.MedicalFiles.Add(file);
            await _db.SaveChangesAsync();
            return file;
        }

        public async Task<List<MedicalFile>> GetFilesByUserAsync(int userId)
        {
            return await _db.MedicalFiles
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.UploadedAt)
                .ToListAsync();
        }

        public async Task<MedicalFile?> GetFileByIdAsync(int id)
        {
            return await _db.MedicalFiles.FindAsync(id);
        }

        public async Task<bool> DeleteFileAsync(MedicalFile file)
        {
            _db.MedicalFiles.Remove(file);
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<MedicalFile?> GetByIdAsync(int id, int userId)
        {
            return await _db.MedicalFiles
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
        }

    }
}
