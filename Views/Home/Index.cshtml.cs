using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using MVC_app_main.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MVC_app_main.Views.Home
{
    public class IndexModel : PageModel
    {
        public async Task<List<Thumbnail>> GetThumbnails()
        { 
            var Thumbnails = SaveDataDB().Result; //Change urls in database to images, because long loading of pages

            using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            conn.Open();
            SqlCommand cmd = new("SELECT * FROM [mobilesdb].dbo.thumbnails", conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                Thumbnail thumbnail = new();
                thumbnail.Title = reader.GetString(1);
                thumbnail.Url = reader.GetString(2);
                thumbnail.ImageUrl = reader.GetString(3);
                thumbnail.NewsSite = reader.GetString(4);
                thumbnail.Summary = reader.GetString(5);
                thumbnail.PublishedAt = reader.GetString(6);
                thumbnail.UpdatedAt = reader.GetString(7);
                Thumbnails.Add(thumbnail);
            }

            conn.Close();

            return Thumbnails;
        }

        private async Task<List<Thumbnail>?> SaveDataDB()
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
                    Thumbnails[i].PublishedAt = DateTime.Parse(Thumbnails[i].PublishedAt).ToUniversalTime().ToString() + "UTC";
                }

                using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
                conn.Open();
                SqlCommand cmdr = new("SELECT * FROM [mobilesdb].dbo.thumbnails;", conn);
                cmdr.CommandType = CommandType.Text;
                SqlDataReader reader = await cmdr.ExecuteReaderAsync();

                var clone = new List<Thumbnail>();

                while (reader.Read())
                {
                    Thumbnail thumbnail = new();
                    thumbnail.Title = reader.GetString(1);
                    thumbnail.Url = reader.GetString(2);
                    thumbnail.ImageUrl = reader.GetString(3);
                    thumbnail.NewsSite = reader.GetString(4);
                    thumbnail.Summary = reader.GetString(5);
                    thumbnail.PublishedAt = reader.GetString(6);
                    thumbnail.UpdatedAt = reader.GetString(7);

                    for (int i = 0; i < Thumbnails.Count; i++)
                    {
                        if (Thumbnails[i].Title.Equals(thumbnail.Title) &&
                            Thumbnails[i].Url.Equals(thumbnail.Url) &&
                            Thumbnails[i].ImageUrl.Equals(thumbnail.ImageUrl) &&
                            Thumbnails[i].NewsSite.Equals(thumbnail.NewsSite) &&
                            Thumbnails[i].Summary.Equals(thumbnail.Summary) &&
                            Thumbnails[i].PublishedAt.Equals(thumbnail.PublishedAt) &&
                            Thumbnails[i].UpdatedAt.Equals(thumbnail.UpdatedAt))
                        {
                            clone.Add(Thumbnails[i]);
                        }
                    }
                }

                conn.Close();

                for (int i = 0; i < clone.Count; i++)
                {
                    Thumbnails.Remove(clone[i]);
                }

                conn.Open();

                SqlCommand cmd = new(@"INSERT INTO [mobilesdb].dbo.thumbnails
                (Title, Url, ImageUrl, NewsSite, Summary, PublishedAt, UpdatedAt)
                VALUES(@Title, @Url, @ImageUrl, @NewsSite, @Summary, @PublishedAt, @UpdatedAt)", conn);
                cmd.CommandType = CommandType.Text;

                for (int i = 0; i < Thumbnails.Count; i++)
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

                conn.Close();
            }
            return Thumbnails;
        }
    }
}
