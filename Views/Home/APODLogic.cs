using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;

namespace MVC_app_main.Views.Home
{
    public class APODLogic
    {
        private int totalSize { get; set; } = 0;
        private int itemsPerPage { get; set; } = 20;

        private async Task<List<APOD>> GetPhotos(int page)
        {
            SaveDataDB();

            List<APOD> pictures = new();

            using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            conn.Open();
            SqlCommand cmd = new("SELECT * FROM [mobilesdb].dbo.APOD", conn)
            {
                CommandType = CommandType.Text
            };
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            try
            {
                while (reader.Read())
                {
                    APOD photo = new()
                    {
                        copyright = reader.GetValue("copyright").ToString(),
                        date = reader.GetValue("date").ToString(),
                        explanation = reader.GetValue("explanation").ToString(),
                        hdurl = reader.GetValue("hdurl").ToString(),
                        media_type = reader.GetValue("media_type").ToString(),
                        service_version = reader.GetValue("service_version").ToString(),
                        title = reader.GetValue("title").ToString(),
                        url = reader.GetValue("url").ToString()
                    };
                    pictures.Add(photo);
                }
            }
            catch (Exception ex) { }

            await reader.CloseAsync();
            await conn.CloseAsync();

            totalSize = pictures.Count;

            return pictures.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();
        }

        private static async void SaveDataDB()
        {
            List<APOD> pictures = new();
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.nasa.gov/planetary/apod?api_key=0fu6kxm8VJ28tAbk0iRAfazBSiqBW5v344fYDIiR&count=90");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            if (response.IsSuccessStatusCode)
            {
                pictures = JsonConvert.DeserializeObject<List<APOD>>(response.Content.ReadAsStringAsync().Result);
            }

            if (pictures != null)
            {
                using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
                conn.Open();
                SqlCommand cmdr = new("SELECT * FROM [mobilesdb].dbo.APOD;", conn)
                {
                    CommandType = CommandType.Text
                };
                SqlDataReader reader = await cmdr.ExecuteReaderAsync();

                try
                {
                    while (reader.Read())
                    {
                        APOD picture = new()
                        {
                            explanation = reader.GetValue("explanation").ToString(),
                            hdurl = reader.GetValue("hdurl").ToString(),
                            media_type = reader.GetValue("media_type").ToString(),
                            title = reader.GetValue("title").ToString(),
                            url = reader.GetValue("url").ToString()
                        };

                        for (int i = 0; i < pictures.Count; i++)
                        {
                            if (pictures[i].explanation.Equals(picture.explanation) &&
                                pictures[i].media_type.Equals(picture.media_type) &&
                                pictures[i].title.Equals(picture.title) &&
                                pictures[i].url.Equals(picture.url))
                            {
                                pictures.RemoveAt(i);
                            }
                        }
                    }
                }
                catch (Exception ex) { }

                await reader.CloseAsync();

                try
                {
                    for (int i = 0; i < pictures.Count; i++)
                    {
                        if (pictures[i].copyright != null)
                        {
                            if (pictures[i].hdurl != null)
                            {
                                SqlCommand cmd = new(@"INSERT INTO [mobilesdb].dbo.APOD
                                (copyright, date, explanation, hdurl, media_type, service_version, title, url)
                                VALUES (@copyright, @date, @explanation, @hdurl, @media_type, @service_version, @title, @url)", conn)
                                {
                                    CommandType = CommandType.Text
                                };

                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@copyright", pictures[i].copyright);
                                cmd.Parameters.AddWithValue("@date", pictures[i].date);
                                cmd.Parameters.AddWithValue("@explanation", pictures[i].explanation);
                                cmd.Parameters.AddWithValue("@hdurl", pictures[i].hdurl);
                                cmd.Parameters.AddWithValue("@media_type", pictures[i].media_type);
                                cmd.Parameters.AddWithValue("@service_version", pictures[i].service_version);
                                cmd.Parameters.AddWithValue("@title", pictures[i].title);
                                cmd.Parameters.AddWithValue("@url", pictures[i].url);

                                await cmd.ExecuteNonQueryAsync();
                            }
                            else if (pictures[i].hdurl == null)
                            {
                                SqlCommand cmd = new(@"INSERT INTO [mobilesdb].dbo.APOD
                                (copyright, date, explanation, media_type, service_version, title, url)
                                VALUES (@copyright, @date, @explanation, @media_type, @service_version, @title, @url)", conn)
                                {
                                    CommandType = CommandType.Text
                                };

                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@copyright", pictures[i].copyright);
                                cmd.Parameters.AddWithValue("@date", pictures[i].date);
                                cmd.Parameters.AddWithValue("@explanation", pictures[i].explanation);
                                cmd.Parameters.AddWithValue("@media_type", pictures[i].media_type);
                                cmd.Parameters.AddWithValue("@service_version", pictures[i].service_version);
                                cmd.Parameters.AddWithValue("@title", pictures[i].title);
                                cmd.Parameters.AddWithValue("@url", pictures[i].url);

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        else if (pictures[i].copyright == null)
                        {
                            if (pictures[i].hdurl != null)
                            {
                                SqlCommand cmd = new(@"INSERT INTO [mobilesdb].dbo.APOD
                                (date, explanation, hdurl, media_type, service_version, title, url)
                                VALUES (@date, @explanation, @hdurl, @media_type, @service_version, @title, @url)", conn)
                                {
                                    CommandType = CommandType.Text
                                };

                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@date", pictures[i].date);
                                cmd.Parameters.AddWithValue("@explanation", pictures[i].explanation);
                                cmd.Parameters.AddWithValue("@hdurl", pictures[i].hdurl);
                                cmd.Parameters.AddWithValue("@media_type", pictures[i].media_type);
                                cmd.Parameters.AddWithValue("@service_version", pictures[i].service_version);
                                cmd.Parameters.AddWithValue("@title", pictures[i].title);
                                cmd.Parameters.AddWithValue("@url", pictures[i].url);

                                await cmd.ExecuteNonQueryAsync();
                            }
                            else if (pictures[i].hdurl == null)
                            {
                                SqlCommand cmd = new(@"INSERT INTO [mobilesdb].dbo.APOD
                                (date, explanation, media_type, service_version, title, url)
                                VALUES (@date, @explanation, @media_type, @service_version, @title, @url)", conn)
                                {
                                    CommandType = CommandType.Text
                                };

                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@date", pictures[i].date);
                                cmd.Parameters.AddWithValue("@explanation", pictures[i].explanation);
                                cmd.Parameters.AddWithValue("@media_type", pictures[i].media_type);
                                cmd.Parameters.AddWithValue("@service_version", pictures[i].service_version);
                                cmd.Parameters.AddWithValue("@title", pictures[i].title);
                                cmd.Parameters.AddWithValue("@url", pictures[i].url);

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
                catch (SqlException sqlex) { }

                await conn.CloseAsync();
            }
        }

        public List<Object> ToController(int page)
        {
            List<Object> list = new();
            var images = GetPhotos(page).Result;

            decimal size = Math.Floor((decimal)totalSize / itemsPerPage) % 2 == 0 ? Math.Floor((decimal)totalSize / itemsPerPage) : Math.Floor((decimal)totalSize / itemsPerPage) + 1;
            list.Add(images);
            list.Add(size);

            return list;
        }
    }
}
