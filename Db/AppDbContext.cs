using DataTransferApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace DataTransferApi.Db
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<SavedFile> SavedFiles { get; set; } = null!;
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SavedFile>()
                .HasOne<User>(f => f.User)
                .WithMany(u => u.SavedFiles)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
