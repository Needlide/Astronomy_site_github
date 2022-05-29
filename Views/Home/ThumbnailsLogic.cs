using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;

namespace MVC_app_main.Views.Home
{
    public class ThumbnailsLogic
    {
        public int page { get; set; } = 1;
        public int size { get; set; } = 20;
        public int totalSize { get; set; } = 0;

        public async Task<List<Thumbnail>?> GetThumbnails()
        {
            var Thumbnails = SaveDataDB().Result;
            List<Thumbnail> ThumbnailsList = new();

            using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            conn.Open();
            SqlCommand cmd = new("SELECT * FROM [mobilesdb].dbo.thumbnails", conn)
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

                    ThumbnailsList.Add(thumbnail);
                }
            }
            catch (Exception ex) { }

            await conn.CloseAsync();

            if (ThumbnailsList.Count > 0)
            {
                for (int i = ThumbnailsList.Count - 1; i > 0; i--)
                {
                    Thumbnails.Remove(ThumbnailsList[i]);
                }
            }

            for (int i = ThumbnailsList.Count - 1; i > 0; i--)
            {
                Thumbnails.Add(ThumbnailsList[i]);
            }
            int totalSize = Thumbnails.Count;
            Thumbnails.Sort((x, y) => DateTime.Compare(x.PublishedAt, y.PublishedAt));
            Thumbnails = Thumbnails.Skip((page - 1) * size).Take(size).ToList();

            return Thumbnails;
        }

        public async Task<List<Thumbnail>?> SaveDataDB()
        {
            List<Thumbnail>? Thumbnails = new();

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.spaceflightnewsapi.net/v3/articles?_limit=50");
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

                var clone = new List<Thumbnail>();

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
                                clone.Add(Thumbnails[i]);
                            }
                        }
                    }
                }
                catch (Exception ex) { }

                await reader.CloseAsync();
                await conn.CloseAsync();

                for (int i = 0; i < clone.Count; i++)
                {
                    Thumbnails.Remove(clone[i]);
                }

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
            return Thumbnails;
        }
    }
}
