using MongoDB.Driver;
using MVC_app_main.Models;
using System.Data;

namespace MVC_app_main.Views.ViewsLogic
{
	public class GalleryLogic
    {
		private readonly string _conn = "mongodb://localhost:27017";

		//Total size of items from NASAImages table
		private int _totalSize { get; set; } = 0;
		//How many items needs to be represented on one page
		private int _itemsPerPage { get; set; } = 30;

		/// <summary>
		/// Connects with database and selects all items from NASAImages table. Calculates which and how much items needs to be in the list.
		/// </summary>
		/// <param name="page">Parameter for pagination. Default is 1.</param>
		/// <returns>List with elements type of ImagesGallery from NASAImages table in the database.</returns>
        private List<ImagesGallery> GetImages(int page)
        {
            MongoClient client = new(_conn);
            return client.GetDatabase("ACU_DB").GetCollection<ImagesGallery>("NASA").Find(x => true).ToList().Skip((page - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();
        }
		/// <summary>
		/// Provides necessary information for the correct display of items on the page.
		/// </summary>
		/// <param name="page">Parameter for pagination. Default is 1.</param>
		/// <returns>List of objects with items and count of available pages which page needs for the correct display of items.</returns>
		public List<object> ToController(string sortBy, int page)
        {
            List<object> list = new();
            string sortOrderDC = string.Empty, sortOrderT = string.Empty, sortOrderNI = string.Empty, sortOrderC = string.Empty;
            decimal size = 0;
            
            var images = GetImages(page);

            sortOrderDC = string.IsNullOrEmpty(sortBy) ? "DateCreated" : "DateCreated_desc";
            sortOrderT = sortBy == "Title" ? "Title_desc" : "Title";
            sortOrderNI = sortBy == "NASAId" ? "NASAId_desc" : "NASAId";
            sortOrderC = sortBy == "Center" ? "Center_desc" : "Center";

            if(images != null)
            {
                images = sortBy switch
                {
                    "Title" => images = images.OrderBy(x => x.Title).ToList(),
                    "Title_desc" => images = images.OrderByDescending(x => x.Title).ToList(),
                    "NASAId" => images = images.OrderBy(x => x.NASAID).ToList(),
                    "NASAId_desc" => images = images.OrderByDescending(x => x.NASAID).ToList(),
                    "DateCreated" => images = images.OrderBy(x => x.DateCreated).ToList(),
                    "Center" => images = images.OrderBy(x => x.Center).ToList(),
                    "Center_desc" => images = images.OrderByDescending(x => x.Center).ToList(),
                    _ => images = images.OrderByDescending(x => x.DateCreated).ToList()
                };
            }

            size = Math.Floor((decimal)_totalSize / _itemsPerPage) % 2 == 0 ? Math.Floor((decimal)_totalSize / _itemsPerPage) : Math.Floor((decimal)_totalSize / _itemsPerPage) + 1;
            list.Add(images);
            list.Add(size);
            list.Add(sortOrderDC);
            list.Add(sortOrderT);
            list.Add(sortOrderNI);
            list.Add(sortOrderC);

            return list;
        }
    }
}
