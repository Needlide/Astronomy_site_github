using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using System.Data;

namespace MVC_app_main.Views.ViewsLogic
{
    public class ThumbnailsLogic
    {
        private const string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AstroDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

        private int totalSize { get; set; } = 0;
        private int itemsPerPage { get; set; } = 50;

        public async Task<List<Thumbnail>?> GetThumbnails(int page)
        {
            List<Thumbnail> Thumbnails = new();

            using SqlConnection conn = new(ConnectionString);
            conn.Open();
            SqlCommand cmd = new("SELECT * FROM [thumbnails] ORDER BY PublishedAt;", conn)
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

            totalSize = Thumbnails.Count;
            Thumbnails.Sort((x, y) => DateTime.Compare(y.PublishedAt, x.PublishedAt));

            return Thumbnails.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();
        }

        public List<object> ToController(string sortBy, int page)
        {
            List<object> list = new();
            string sortOrderP = string.Empty, sortOrderT = string.Empty, sortOrderNS = string.Empty, sortOrderU = string.Empty;
            decimal size = 0;

            var thumbnails = GetThumbnails(page).Result;

            sortOrderP = string.IsNullOrEmpty(sortBy) ? "P" : "";
            sortOrderT = sortBy == "Title" ? "Title_desc" : "Title";
            sortOrderNS = sortBy == "NS" ? "NS_desc" : "NS";
            sortOrderU = sortBy == "U" ? "U_desc" : "U";

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

            size = Math.Floor((decimal)totalSize / itemsPerPage) % 2 == 0 ? Math.Floor((decimal)totalSize / itemsPerPage) : Math.Floor((decimal)totalSize / itemsPerPage) + 1;

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
