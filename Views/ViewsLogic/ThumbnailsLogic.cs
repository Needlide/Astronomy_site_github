using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using System.Data;

namespace MVC_app_main.Views.ViewsLogic
{
    public class ThumbnailsLogic
    {
        private const string _conn = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AstroDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

		//Total size of items from NASAImages table
		private int _totalSize { get; set; } = 0;
		//How many items needs to be represented on one page
		private int _itemsPerPage { get; set; } = 50;

		/// <summary>
		/// Connects with database and selects all items from thumbnails table. Calculates which and how much items needs to be in the list.
		/// </summary>
		/// <param name="page">Parameter for pagination. Default is 1.</param>
		/// <returns>List with elements type of Thumbnail from thumbnails table in the database.</returns>
		public async Task<List<Thumbnail>?> GetThumbnailsAsync(int page)
        {
            List<Thumbnail> Thumbnails = new();

            using SqlConnection conn = new(_conn);
            conn.Open();
            SqlCommand cmd = new("SELECT * FROM thumbnails ORDER BY PublishedAt", conn)
            {
                CommandType = CommandType.Text
            };
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            try
            {
                while (reader.Read())
                {
                    Thumbnail thumbnail = new()
                    {
                        Title = reader.GetString(1),
                        Url = reader.GetString(2),
                        ImageUrl = reader.GetString(3),
                        NewsSite = reader.GetString(4),
                        Summary = reader.GetString(5),
                        PublishedAt = reader.GetDateTime(reader.GetOrdinal(nameof(thumbnail.PublishedAt))),
                        UpdatedAt = reader.GetDateTime(reader.GetOrdinal(nameof(thumbnail.UpdatedAt))),
                    };

                    Thumbnails.Add(thumbnail);
                }
            }
            catch (Exception ex) { }

            await conn.CloseAsync();
            await reader.CloseAsync();

            _totalSize = Thumbnails.Count;
            Thumbnails.Sort((x, y) => DateTime.Compare(y.PublishedAt, x.PublishedAt));

            return Thumbnails.Skip((page - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();
        }

		/// <summary>
		/// Provides necessary information for the correct display of items on the page.
		/// </summary>
		/// <param name="page">Parameter for pagination. Default is 1.</param>
		/// <returns>List of objects with items and count of available pages which page needs for the correct display of items.</returns>
		public List<object> ToController(string sortBy, int page)
        {
            List<object> list = new();
            string sortOrderP = string.Empty, sortOrderT = string.Empty, sortOrderNS = string.Empty, sortOrderU = string.Empty;

            var thumbnails = GetThumbnailsAsync(page).Result;//timeout

            sortOrderP = string.IsNullOrEmpty(sortBy) ? "P" : string.Empty;
            sortOrderT = sortBy == "Title" ? "Title_desc" : "Title";
            sortOrderNS = sortBy == "NS" ? "NS_desc" : "NS";
            sortOrderU = sortBy == "U" ? "U_desc" : "U";

            if (thumbnails != null)
            {
                thumbnails = sortBy switch
                {
                    "Title" => thumbnails = thumbnails.OrderBy(s => s.Title).ToList(),
                    "Title_desc" => thumbnails = thumbnails.OrderByDescending(s => s.Title).ToList(),
                    "NS" => thumbnails = thumbnails.OrderBy(s => s.NewsSite).ToList(),
                    "NS_desc" => thumbnails = thumbnails.OrderByDescending(s => s.NewsSite).ToList(),
                    "P" => thumbnails = thumbnails.OrderBy(s => s.PublishedAt).ToList(),
                    "U" => thumbnails = thumbnails.OrderBy(s => s.UpdatedAt).ToList(),
                    "U_desc" => thumbnails = thumbnails.OrderByDescending(s => s.UpdatedAt).ToList(),
                    _ => thumbnails = thumbnails.OrderByDescending(s => s.PublishedAt).ToList(),
                };
            }

            decimal size = Math.Floor((decimal)_totalSize / _itemsPerPage) % 2 == 0 ? Math.Floor((decimal)_totalSize / _itemsPerPage) : Math.Floor((decimal)_totalSize / _itemsPerPage) + 1;

            list.Add(thumbnails);
            list.Add(size);
            list.Add(sortOrderP);
            list.Add(sortOrderT);
            list.Add(sortOrderNS);
            list.Add(sortOrderU);

            return list;
        }
    }
}
