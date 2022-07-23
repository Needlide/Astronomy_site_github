using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using System.Data;

namespace MVC_app_main.Views.ViewsLogic
{
    public class APODLogic
    {
        private const string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AstroDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

        private int totalSize { get; set; } = 0;
        private int itemsPerPage { get; set; } = 20;

        private async Task<List<APOD>> GetPhotos(int page)
        {
            List<APOD> pictures = new();

            using SqlConnection conn = new(ConnectionString);
            conn.Open();
            SqlCommand cmd = new("SELECT * FROM [APOD]", conn)
            {
                CommandType = CommandType.Text
            };
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            try
            {
                while (reader.Read())
                {
                    APOD photo = new()
                    {
                        copyright = reader.GetValue("copyright").ToString(),
                        date = reader.GetValue("date").ToString(),
                        explanation = reader.GetValue("explanation").ToString(),
                        hdurl = reader.GetValue("hdurl").ToString(),
                        media_type = reader.GetValue("media_type").ToString(),
                        service_version = reader.GetValue("service_version").ToString(),
                        title = reader.GetValue("title").ToString(),
                        url = reader.GetValue("url").ToString()
                    };
                    pictures.Add(photo);
                }
            }
            catch (Exception ex) { }

            await reader.CloseAsync();
            await conn.CloseAsync();

            totalSize = pictures.Count;

            return pictures.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();
        }

        public List<object> ToController(int page)
        {
            List<object> list = new();
            var images = GetPhotos(page).Result;

            decimal size = Math.Floor((decimal)totalSize / itemsPerPage) % 2 == 0 ? Math.Floor((decimal)totalSize / itemsPerPage) : Math.Floor((decimal)totalSize / itemsPerPage) + 1;
            list.Add(images);
            list.Add(size);

            return list;
        }
    }
}
