using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using System.Data;

namespace MVC_app_main.Views.ViewsLogic
{
    public class APODLogic
    {
        private const string _conn = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AstroDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

        private int _totalSize { get; set; } = 0;
        private int _itemsPerPage { get; set; } = 20;

        private async Task<List<APOD>> GetPhotosAsync(int page)
        {
            List<APOD> pictures = new();

            using SqlConnection conn = new(_conn);
            conn.Open();
            SqlCommand cmd = new("SELECT * FROM APOD", conn)
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
                        copyright = reader["copyright"].ToString(),
                        date = reader["date"].ToString(),
                        explanation = reader["explanation"].ToString(),
                        hdurl = reader["hdurl"].ToString(),
                        media_type = reader["media_type"].ToString(),
                        service_version = reader["service_version"].ToString(),
                        title = reader["title"].ToString(),
                        url = reader["url"].ToString()
                    };
                    pictures.Add(photo);
                }
            }
            catch (Exception ex) { }

            await reader.CloseAsync();
            await conn.CloseAsync();

            _totalSize = pictures.Count;

            return pictures.Skip((page - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();
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
