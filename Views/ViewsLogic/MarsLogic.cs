using MongoDB.Driver;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Data;

namespace MVC_app_main.Views.ViewsLogic
{
    public class MarsLogic
    {
        readonly IMongoCollection<Photos> _photos;

        //Total size of items from NASA table
        private long _totalSize { get; set; } = 0;
		//How many items need to be represented on one page
		private int _itemsPerPage { get; set; } = 30;

        public MarsLogic(IMongoCollection<Photos> photos)
        {
            _photos = photos;
            _totalSize = _photos.EstimatedDocumentCount();
        }

        /// <summary>
        /// Connects with database and selects all items from photos table. Calculates which and how much items needs to be in the list.
        /// </summary>
        /// <param name="page">Parameter for pagination. Default is 1.</param>
        /// <returns>List with elements type of Photos from photos table in the database.</returns>
        private List<Photos> GetPhotos(int page)
        {
            return _photos.Find(x => true).ToList().Skip((page - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();
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

            var photos = GetPhotos(page);

            for( int i = 0; i < photos.Count; i++ )
            {
                photos[i].Camera = JsonConvert.DeserializeObject((string)photos[i].Camera);
                photos[i].Rover = JsonConvert.DeserializeObject((string)photos[i].Rover);
            }

            sortOrderS = sortBy == "Sol_desc" ? "Sol" : "Sol_desc";
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

            long size = (long)(Math.Floor((decimal)_totalSize / _itemsPerPage) % 2 == 0 ? Math.Floor((decimal)_totalSize / _itemsPerPage) : Math.Floor((decimal)_totalSize / _itemsPerPage) + 1);

            list.Add(photos);
            list.Add(size);
            list.Add(sortOrderS);
            list.Add(sortOrderED);

            return list;
        }
    }
}
