using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;

namespace MVC_app_main.Views.Home
{
    public class MarsLogic
    {
        private int totalSize { get; set; } = 0;
        private int itemsPerPage { get; set; } = 30;

        private async Task<List<Photos>> GetPhotos(int page)
        {
            List<Photos> photos = new();
            using SqlConnection conn = new("Data Source=sql.bsite.net\\MSSQL2016;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            conn.Open();
            SqlCommand cmdr = new("SELECT * FROM [needlide_mobilesdb].dbo.photos ORDER BY Sol", conn)
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

            totalSize = photos.Count;
            
            return photos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();
        }

        public List<Object> ToController(/*string sortBy,*/ int page)
        {
            List<Object> list = new();
            /*string sortOrderP = String.Empty, sortOrderT = String.Empty, sortOrderNS = String.Empty, sortOrderU = String.Empty;*/
            decimal size = 0;

            var photos = GetPhotos(page).Result;
            size = Math.Floor((decimal)totalSize / itemsPerPage) % 2 == 0 ? Math.Floor((decimal)totalSize / itemsPerPage) : Math.Floor((decimal)totalSize / itemsPerPage) + 1;

            list.Add(photos);
            list.Add(size);

            return list;
        }
    }
}
