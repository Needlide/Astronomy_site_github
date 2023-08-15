using MongoDB.Driver;
using MVC_app_main.Models;
using System.Data;

namespace MVC_app_main.Views.ViewsLogic
{
    public class ApodLogic
    {
        readonly IMongoCollection<Apod> _apods;

        //Total size of items from APOD table
        private long _totalSize { get; set; } = 0;
        //How many items need to be represented on one page
        private int _itemsPerPage { get; set; } = 20;

        public ApodLogic(IMongoCollection<Apod> apods)
        {
            _apods = apods;
            _totalSize = _apods.EstimatedDocumentCount();
        }

        /// <summary>
        /// Connects with database and selects all items from Apod table. Calculates which and how much items needs to be in the list.
        /// </summary>
        /// <param name="page">Parameter for pagination. Default is 1.</param>
        /// <returns>List with elements type of Apod from APOD table in the database.</returns>
        private List<Apod> GetApod(int page)
        {
            return _apods.Find(x => true).ToList().Skip((page - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();
        }

        /// <summary>
        /// Provides necessary information for the correct display of items on the page.
        /// </summary>
        /// <param name="page">Parameter for pagination. Default is 1.</param>
        /// <returns>List of objects with items and count of available pages which page needs for the correct display of items.</returns>
        public List<object> ToController(string sortBy, int page)
        {
            List<object> list = new();
            string sortOrderD = string.Empty, sortOrderT = string.Empty, sortOrderC = string.Empty;
            var images = GetApod(page);

            sortOrderD = sortBy == "Date_desc" ? "Date" : "Date_desc";
            sortOrderT = sortBy == "Title" ? "Title_desc" : "Title";
            sortOrderC = sortBy == "Author" ? "Author_desc" : "Author";

            if(images != null)
            {
                images = sortBy switch
                {
                    "Title" => images = images.OrderBy(x => x.Title).ToList(),
                    "Title_desc" => images = images.OrderByDescending(x => x.Title).ToList(),
                    "Author" => images = images.OrderBy(x => x.Copyright).ToList(),
                    "Author_desc" => images = images.OrderByDescending(x => x.Copyright).ToList(),
                    "Date" => images = images.OrderBy(x => x.Date).ToList(),
                    _ => images = images.OrderByDescending(x => x.Date).ToList()
                };
            }

            long size = (long)(Math.Floor((decimal)_totalSize / _itemsPerPage) % 2 == 0 ? Math.Floor((decimal)_totalSize / _itemsPerPage) : Math.Floor((decimal)_totalSize / _itemsPerPage) + 1);
            list.Add(images);
            list.Add(size);
            list.Add(sortOrderD);
            list.Add(sortOrderT);
            list.Add(sortOrderC);

            return list;
        }
    }
}
