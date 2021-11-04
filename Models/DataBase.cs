using Microsoft.EntityFrameworkCore;

namespace MVC_app_main.Models
{
    public class DataBase : DbContext
    {
        public DbSet<Thumbnail> thumbnails {  get; set; }
        public DataBase(DbContextOptions<DataBase> options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}