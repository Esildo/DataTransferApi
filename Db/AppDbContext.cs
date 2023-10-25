using DataTransferApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Xml;


namespace DataTransferApi.Db
{
    public class AppDbContext : IdentityDbContext<User>
    {

        public DbSet<TokenLink> TokenLinks { get; set; } = null!;   
        public DbSet<FileGroup> FileGroups { get; set; } = null!;
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

            modelBuilder.Entity<SavedFile>()
                .HasOne<FileGroup>(f => f.FileGroup)
                .WithMany(u => u.SavedFiles)
                .HasForeignKey(f => f.FileGroupId);

            modelBuilder.Entity<SavedFile>()
                .HasIndex(e => e.SavedFileName);

            modelBuilder.Entity<FileGroup>()
                .HasIndex(e => e.Name);
            modelBuilder.Entity<FileGroup>()
                .HasIndex(e => e.Name);

            modelBuilder.Entity<TokenLink>()
                .HasIndex(e => e.Token);
        }
    }
}
