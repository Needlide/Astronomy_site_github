using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using System.Data;

namespace MVC_app_main.Views.ViewsLogic
{
    public class ApodLogic
    {
        private const string _conn = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AstroDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

        //Total size of items from APOD table
        private int _totalSize { get; set; } = 0;
        //How many items needs to be represented on one page
        private int _itemsPerPage { get; set; } = 20;

        /// <summary>
        /// Connects with database and selects all items from Apod table. Calculates which and how much items needs to be in the list.
        /// </summary>
        /// <param name="page">Parameter for pagination. Default is 1.</param>
        /// <returns>List with elements type of Apod from APOD table in the database.</returns>
        private async Task<List<Apod>> GetPhotosAsync(int page)
        {
            List<Apod> pictures = new();

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
                    Apod photo = new()
                    {
                        Copyright = reader["copyright"].ToString(),
                        Date = reader["date"].ToString(),
                        Explanation = reader["explanation"].ToString(),
                        HdUrl = reader["hdurl"].ToString(),
                        MediaType = reader["media_type"].ToString(),
                        ServiceVersion = reader["service_version"].ToString(),
                        Title = reader["title"].ToString(),
                        Url = reader["url"].ToString()
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

        /// <summary>
        /// Provides necessary information for the correct display of items on the page.
        /// </summary>
        /// <param name="page">Parameter for pagination. Default is 1.</param>
        /// <returns>List of objects with items and count of available pages which page needs for the correct display of items.</returns>
        public List<object> ToController(string sortBy, int page)
        {
            List<object> list = new();
            string sortOrderD = string.Empty, sortOrderT = string.Empty, sortOrderC = string.Empty;
            var images = GetPhotosAsync(page).Result;

            sortOrderD = string.IsNullOrEmpty(sortBy) ? "Date" : "Date_desc";
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

            decimal size = Math.Floor((decimal)_totalSize / _itemsPerPage) % 2 == 0 ? Math.Floor((decimal)_totalSize / _itemsPerPage) : Math.Floor((decimal)_totalSize / _itemsPerPage) + 1;
            list.Add(images);
            list.Add(size);
            list.Add(sortOrderD);
            list.Add(sortOrderT);
            list.Add(sortOrderC);

            return list;
        }
    }
}
