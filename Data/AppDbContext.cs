using medical_record_system_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace medical_record_system_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<User> Users { get; set; }
        public DbSet<MedicalFile> MedicalFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<MedicalFile>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);
        }
    }
}
