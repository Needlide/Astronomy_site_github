using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Data;

namespace MVC_app_main.Views.ViewsLogic
{
    public class GalleryLogic
    {
        private const string _conn = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AstroDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

		//Total size of items from NASAImages table
		private int _totalSize { get; set; } = 0;
		//How many items needs to be represented on one page
		private int _itemsPerPage { get; set; } = 30;

		/// <summary>
		/// Connects with database and selects all items from NASAImages table. Calculates which and how much items needs to be in the list.
		/// </summary>
		/// <param name="page">Parameter for pagination. Default is 1.</param>
		/// <returns>List with elements type of ImagesGallery from NASAImages table in the database.</returns>
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
                        Center = reader["center"].ToString(),
                        Title = reader["title"].ToString(),
                        NasaId = reader["title"].ToString(),
                        MediaType = reader["media_type"].ToString(),
                        Keywords = JsonConvert.DeserializeObject<List<string>>(reader["keywords"].ToString()),
                        DateCreated = (DateTime)reader["date_created"],
                        Description508 = reader["description_508"].ToString(),
                        SecondaryCreator = reader["secondary_creator"].ToString(),
                        Description = reader["description"].ToString(),
                        Href = reader["href"].ToString(),
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

		/// <summary>
		/// Provides necessary information for the correct display of items on the page.
		/// </summary>
		/// <param name="page">Parameter for pagination. Default is 1.</param>
		/// <returns>List of objects with items and count of available pages which page needs for the correct display of items.</returns>
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
