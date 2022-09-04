using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Data;

namespace MVC_app_main.Views.ViewsLogic
{
    public class GalleryLogic
    {
        private const string _conn = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AstroDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

        private int _totalSize { get; set; } = 0;
        private int _itemsPerPage { get; set; } = 30;

        public async Task<List<ImagesGallery>> GetPhotosAsync(int page)
        {
            List<ImagesGallery> images = new();
            using SqlConnection conn = new(_conn);
            conn.Open();
            SqlCommand cmdr = new("SELECT * FROM NASAImages ORDER BY date_created", conn)
            {
                CommandType = CommandType.Text
            };
            SqlDataReader reader = await cmdr.ExecuteReaderAsync();

            try
            {
                while (reader.Read())
                {
                    ImagesGallery imagesGallery = new()
                    {
                        center = reader["center"].ToString(),
                        title = reader["title"].ToString(),
                        nasa_id = reader["title"].ToString(),
                        media_type = reader["media_type"].ToString(),
                        keywords = JsonConvert.DeserializeObject<List<string>>(reader["keywords"].ToString()),
                        date_created = (DateTime)reader["date_created"],
                        description_508 = reader["description_508"].ToString(),
                        secondary_creator = reader["secondary_creator"].ToString(),
                        description = reader["description"].ToString(),
                        href = reader["href"].ToString(),
                    };

                    images.Add(imagesGallery);
                }
            }
            catch (Exception ex) { }

            await conn.CloseAsync();
            await reader.CloseAsync();

            _totalSize = images.Count;

            return images.Skip((page - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();
        }

        public List<object> ToController(int page)
        {
            List<object> list = new();
            var images = GetPhotosAsync(page).Result;

            decimal size = Math.Floor((decimal)_totalSize / _itemsPerPage) % 2 == 0 ? Math.Floor((decimal)_totalSize / _itemsPerPage) : Math.Floor((decimal)_totalSize / _itemsPerPage) + 1;
            list.Add(images);
            list.Add(size);

            return list;
        }
    }
}
