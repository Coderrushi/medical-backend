namespace medical_record_system_backend.Models
{
    public class MedicalFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public string DisplayName { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public string ContentType { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
