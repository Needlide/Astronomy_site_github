using MongoDB.Driver;
using MVC_app_main.Models;
using System.Data;

namespace MVC_app_main.Views.ViewsLogic
{
	public class ThumbnailsLogic
    {
		private readonly string _conn = "mongodb://localhost:27017";

		//Total size of items from thumbnails table
		private int _totalSize { get; set; } = 0;
		//How many items need to be represented on one page
		private int _itemsPerPage { get; set; } = 30;

		/// <summary>
		/// Connects with database and selects all items from thumbnails table. Calculates which and how much items needs to be in the list.
		/// </summary>
		/// <param name="page">Parameter for pagination. Default is 1.</param>
		/// <returns>List with elements type of Thumbnail from thumbnails table in the database.</returns>

        private List<Thumbnail> GetThumbnails(int page)
        {
            MongoClient client = new(_conn);
            return client.GetDatabase("ACU_DB").GetCollection<Thumbnail>("Thumbnails").Find(x => true).ToList().Skip((page - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();
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

            var thumbnails = GetThumbnails(page);

            sortOrderP = string.IsNullOrEmpty(sortBy) ? "PublishedAt_desc" : "PublishedAt";
            sortOrderT = sortBy == "Title" ? "Title_desc" : "Title";
            sortOrderNS = sortBy == "NewsSite" ? "NewsSite_desc" : "NewsSite";
            sortOrderU = sortBy == "UpdatedAt" ? "UpdatedAt_desc" : "UpdatedAt";

            if (thumbnails != null)
            {
                thumbnails = sortBy switch
                {
                    "Title" => thumbnails = thumbnails.OrderBy(s => s.Title).ToList(),
                    "Title_desc" => thumbnails = thumbnails.OrderByDescending(s => s.Title).ToList(),
                    "NewsSite" => thumbnails = thumbnails.OrderBy(s => s.NewsSite).ToList(),
                    "NewsSite_desc" => thumbnails = thumbnails.OrderByDescending(s => s.NewsSite).ToList(),
                    "PublishedAt" => thumbnails = thumbnails.OrderBy(s => s.PublishedAt).ToList(),
                    "UpdatedAt" => thumbnails = thumbnails.OrderBy(s => s.UpdatedAt).ToList(),
                    "UpdatedAt_desc" => thumbnails = thumbnails.OrderByDescending(s => s.UpdatedAt).ToList(),
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
