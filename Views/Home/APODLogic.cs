using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;

namespace MVC_app_main.Views.Home
{
    public class APODLogic
    {
        private int totalSize { get; set; } = 0;
        private int itemsPerPage { get; set; } = 20;

        private async Task<List<APOD>> GetPhotos(int page)
        {
            List<APOD> pictures = new();

            using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            conn.Open();
            SqlCommand cmd = new("SELECT * FROM mobilesdb.dbo.APOD", conn)
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

        public List<Object> ToController(int page)
        {
            List<Object> list = new();
            var images = GetPhotos(page).Result;

            decimal size = Math.Floor((decimal)totalSize / itemsPerPage) % 2 == 0 ? Math.Floor((decimal)totalSize / itemsPerPage) : Math.Floor((decimal)totalSize / itemsPerPage) + 1;
            list.Add(images);
            list.Add(size);

            return list;
        }
    }
}
