using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Data;

namespace MVC_app_main.Views.ViewsLogic
{
    public class MarsLogic
    {
        private const string _conn = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AstroDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

		//Total size of items from NASAImages table
		private int _totalSize { get; set; } = 0;
		//How many items needs to be represented on one page
		private int _itemsPerPage { get; set; } = 30;

		/// <summary>
		/// Connects with database and selects all items from photos table. Calculates which and how much items needs to be in the list.
		/// </summary>
		/// <param name="page">Parameter for pagination. Default is 1.</param>
		/// <returns>List with elements type of Photos from photos table in the database.</returns>
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
                        ImgSrc = reader.GetString("Img_src"),
                        EarthDate = reader.GetString("Earth_date"),
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

		/// <summary>
		/// Provides necessary information for the correct display of items on the page.
		/// </summary>
		/// <param name="page">Parameter for pagination. Default is 1.</param>
		/// <returns>List of objects with items and count of available pages which page needs for the correct display of items.</returns>
		public List<object> ToController(string sortBy, int page)
        {
            List<object> list = new();
            string sortOrderS = string.Empty, sortOrderED = string.Empty;
            decimal size = 0;

            var photos = GetPhotosAsync(page).Result;

            sortOrderS = string.IsNullOrEmpty(sortBy) ? "Sol" : "Sol_desc";
            sortOrderED = sortBy == "EarthDate" ? "EarthDate_desc" : "EarthDate";

            if(photos != null)
            {
                photos = sortBy switch
                {
                    "EarthDate" => photos = photos.OrderBy(s => s.EarthDate).ToList(),
                    "EarthDate_desc" => photos = photos.OrderByDescending(s => s.EarthDate).ToList(),
                    "Sol" => photos = photos.OrderBy(s => s.Sol).ToList(),
                    _ => photos = photos.OrderByDescending(s => s.Sol).ToList()
                };
            }

            size = Math.Floor((decimal)_totalSize / _itemsPerPage) % 2 == 0 ? Math.Floor((decimal)_totalSize / _itemsPerPage) : Math.Floor((decimal)_totalSize / _itemsPerPage) + 1;

            list.Add(photos);
            list.Add(size);
            list.Add(sortOrderS);
            list.Add(sortOrderED);

            return list;
        }
    }
}
