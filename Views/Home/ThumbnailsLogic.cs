using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;

namespace MVC_app_main.Views.Home
{
    public class ThumbnailsLogic
    {
        private int totalSize { get; set; } = 0;

        public async Task<List<Thumbnail>?> GetThumbnails(int page)
        {
            SaveDataDB();
            List<Thumbnail> Thumbnails = new();

            using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            conn.Open();
            SqlCommand cmd = new("SELECT * FROM [mobilesdb].dbo.thumbnails ORDER BY PublishedAt;", conn)
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

            totalSize = Thumbnails.Count;
            Thumbnails.Sort((x, y) => DateTime.Compare(y.PublishedAt, x.PublishedAt));
            Thumbnails = Thumbnails.Skip((page - 1) * 50).Take(50).ToList();

            return Thumbnails;
        }

        public async void SaveDataDB()
        {
            List<Thumbnail>? Thumbnails = new();

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.spaceflightnewsapi.net/v3/articles?_limit=10");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            if (response.IsSuccessStatusCode)
            {
                Thumbnails = JsonConvert.DeserializeObject<List<Thumbnail>>(response.Content.ReadAsStringAsync().Result);
            }

            if (Thumbnails != null)
            {
                for (int i = 0; i < Thumbnails.Count; i++)
                {
                    Thumbnails[i].PublishedAt = Thumbnails[i].PublishedAt.ToUniversalTime();
                }

                using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
                conn.Open();
                SqlCommand cmdr = new("SELECT * FROM [mobilesdb].dbo.thumbnails ORDER BY PublishedAt;", conn)
                {
                    CommandType = CommandType.Text
                };
                SqlDataReader reader = await cmdr.ExecuteReaderAsync();

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

                        for (int i = 0; i < Thumbnails.Count; i++)
                        {
                            if (Thumbnails[i].Title.Equals(thumbnail.Title) &&
                                Thumbnails[i].Url.Equals(thumbnail.Url) &&
                                Thumbnails[i].ImageUrl.Equals(thumbnail.ImageUrl) &&
                                Thumbnails[i].NewsSite.Equals(thumbnail.NewsSite) &&
                                Thumbnails[i].Summary.Equals(thumbnail.Summary) &&
                                Thumbnails[i].PublishedAt.ToString().Equals(thumbnail.PublishedAt.ToString()) &&
                                Thumbnails[i].UpdatedAt.ToString().Equals(thumbnail.UpdatedAt.ToString()))
                            {
                                Thumbnails.RemoveAt(i);
                            }
                        }
                    }
                }
                catch (Exception ex) { }

                await reader.CloseAsync();
                await conn.CloseAsync();

                conn.Open();

                try
                {
                    SqlCommand cmd = new(@"INSERT INTO [mobilesdb].dbo.thumbnails
                    (Title, Url, ImageUrl, NewsSite, Summary, PublishedAt, UpdatedAt)
                    VALUES(@Title, @Url, @ImageUrl, @NewsSite, @Summary, @PublishedAt, @UpdatedAt)", conn)
                    {
                        CommandType = CommandType.Text
                    };

                    for (int i = Thumbnails.Count - 1; i > 0; i--)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Title", Thumbnails[i].Title);
                        cmd.Parameters.AddWithValue("@Url", Thumbnails[i].Url);
                        cmd.Parameters.AddWithValue("@ImageUrl", Thumbnails[i].ImageUrl);
                        cmd.Parameters.AddWithValue("@NewsSite", Thumbnails[i].NewsSite);
                        cmd.Parameters.AddWithValue("@Summary", Thumbnails[i].Summary);
                        cmd.Parameters.AddWithValue("@PublishedAt", Thumbnails[i].PublishedAt);
                        cmd.Parameters.AddWithValue("@UpdatedAt", Thumbnails[i].UpdatedAt);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (SqlException sqlex) { }

                await conn.CloseAsync();
            }
        }

        public List<Object> ToController(string sortBy, int page)
        {
            List<Object> list = new();
            string sortOrderP = String.Empty, sortOrderT = String.Empty, sortOrderNS = String.Empty, sortOrderU = String.Empty;
            decimal size = 0;
            
            var thumbnails = GetThumbnails(page).Result;

            sortOrderP = String.IsNullOrEmpty(sortBy) ? "P" : "";
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

            size = Math.Floor((decimal)totalSize / 50) % 2 == 0 ? Math.Floor((decimal)totalSize / 50) : Math.Floor((decimal)totalSize / 50) + 1;

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
