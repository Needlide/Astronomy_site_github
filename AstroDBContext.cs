using Microsoft.EntityFrameworkCore;
using MVC_app_main.Models;

namespace MVC_app_main
{
    public partial class AstroDBContext : DbContext
    {
        public AstroDBContext()
        {
        }

        public AstroDBContext(DbContextOptions<AstroDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Thumbnail> Thumbnails { get; set; } = null!;
        public virtual DbSet<Photos> Photos { get; set; } = null!;
        public virtual DbSet<ImagesGallery> ImagesGalleries { get; set; } = null!;
        public virtual DbSet<Apod> APODs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AstroDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Thumbnail>(entity =>
            {
                entity.ToTable("thumbnails");
            });

            modelBuilder.Entity<Photos>(entity =>
            {
                entity.ToTable("photos");
            });

            modelBuilder.Entity<ImagesGallery>(entity => 
            {
                entity.ToTable("NASAImages");
            });

            modelBuilder.Entity<Apod>(entity =>
            {
                entity.ToTable("APOD");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
