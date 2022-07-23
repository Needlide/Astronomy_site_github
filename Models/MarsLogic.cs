using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace MVC_app_main.Models
{
    public class MarsLogic
    {
        private const string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AstroDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

        private int totalSize { get; set; } = 0;
        private int itemsPerPage { get; set; } = 30;

        private async Task<List<DeserializedPhoto>> GetPhotos(int page)
        {
            List<DeserializedPhoto> photos = new();
            using SqlConnection conn = new(ConnectionString);
            conn.Open();
            SqlCommand cmdr = new("SELECT * FROM [photos] ORDER BY Sol", conn)
            {
                CommandType = CommandType.Text
            };
            SqlDataReader reader = await cmdr.ExecuteReaderAsync();

            try
            {
                while (reader.Read())
                {
                    DeserializedPhoto photo = new()
                    {
                        Sol = reader.GetInt32("Sol"),
                        Camera = (IDictionary<string, string>)JObject.Parse(reader.GetString("Camera"))/*JsonConvert.DeserializeObject(reader.GetString("Camera"))*/,
                        Img_src = reader.GetString("Img_src"),
                        Earth_date = reader.GetString("Earth_date"),
                        Rover = (IDictionary<string, string>)JObject.Parse(reader.GetString("Rover"))/*JsonConvert.DeserializeObject(reader.GetString("Rover"))*/
                    };
                    photos.Add(photo);
                }
            }
            catch (Exception ex) { }

            await conn.CloseAsync();
            await reader.CloseAsync();

            totalSize = photos.Count;

            return photos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();
        }

        public List<object> ToController(/*string sortBy,*/ int page)
        {
            List<object> list = new();
            /*string sortOrderP = String.Empty, sortOrderT = String.Empty, sortOrderNS = String.Empty, sortOrderU = String.Empty;*/
            decimal size = 0;

            var photos = GetPhotos(page).Result;
            size = Math.Floor((decimal)totalSize / itemsPerPage) % 2 == 0 ? Math.Floor((decimal)totalSize / itemsPerPage) : Math.Floor((decimal)totalSize / itemsPerPage) + 1;

            list.Add(photos);
            list.Add(size);

            return list;
        }
    }

    class DeserializedPhoto
    {
#pragma warning disable
        public int Id { get; set; }
        public int Sol { get; set; }
        public IDictionary<string, string> Camera { get; set; }
        public string Img_src { get; set; }
        public string Earth_date { get; set; }
        public IDictionary<string, string> Rover { get; set; }
#pragma warning restore
    }
}
