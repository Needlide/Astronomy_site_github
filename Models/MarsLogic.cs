using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace MVC_app_main.Models
{
    public class MarsLogic
    {
        private const string _conn = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AstroDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

        private int _totalSize { get; set; } = 0;
        private int _itemsPerPage { get; set; } = 30;

        private async Task<List<Photos>> GetPhotosAsync(int page)
        {
            List<Photos> photos = new();
            using SqlConnection conn = new(_conn);
            conn.Open();
            SqlCommand cmdr = new("SELECT * FROM photos ORDER BY Sol", conn)
            {
                CommandType = CommandType.Text
            };
            SqlDataReader reader = await cmdr.ExecuteReaderAsync();

            try
            {
                while (reader.Read())
                {
                    Photos photo = new()
                    {
                        Sol = reader.GetInt32("Sol"),
                        Camera = JsonConvert.DeserializeObject(reader.GetString("Camera")),
                        Img_src = reader.GetString("Img_src"),
                        Earth_date = reader.GetString("Earth_date"),
                        Rover = JsonConvert.DeserializeObject(reader.GetString("Rover"))
                    };
                    photos.Add(photo);
                }
            }
            catch (Exception ex) { }

            await conn.CloseAsync();
            await reader.CloseAsync();

            _totalSize = photos.Count;

            return photos.Skip((page - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();
        }

        public List<object> ToController(/*string sortBy,*/ int page)
        {
            List<object> list = new();
            /*string sortOrderP = String.Empty, sortOrderT = String.Empty, sortOrderNS = String.Empty, sortOrderU = String.Empty;*/
            decimal size = 0;

            var photos = GetPhotosAsync(page).Result;
            size = Math.Floor((decimal)_totalSize / _itemsPerPage) % 2 == 0 ? Math.Floor((decimal)_totalSize / _itemsPerPage) : Math.Floor((decimal)_totalSize / _itemsPerPage) + 1;

            list.Add(photos);
            list.Add(size);

            return list;
        }
    }
}
