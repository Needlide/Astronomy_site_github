using MongoDB.Driver;
using MVC_app_main.Models;

namespace MVC_app_main
{
    public class AstroDBContext
    {
        private readonly IMongoDatabase _database;

        public AstroDBContext(IMongoClient client)
        {
            _database = client.GetDatabase("ACU_DB");
        }

        public IMongoCollection<Thumbnail> Thumbnails => _database.GetCollection<Thumbnail>("Thumbnails");
        public IMongoCollection<Photos> Photos => _database.GetCollection<Photos>("Photos");
        public IMongoCollection<ImagesGallery> ImagesGallery => _database.GetCollection<ImagesGallery>("NASA");
        public IMongoCollection<Apod> APODs => _database.GetCollection<Apod>("APOD");
    }
}
