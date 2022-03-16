using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;

namespace MVC_app_main.Views.Home
{
    public class APODModel : PageModel
    {
        public async Task<List<APOD>> GetPhotos()
        {
            var pictures = SaveDataDB().Result;

            using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            conn.Open();
            SqlCommand cmd = new("SELECT * FROM [mobilesdb].dbo.APOD", conn);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            try
            {
                while (reader.Read())
                {
                    APOD photo = new();
                    photo.copyright = reader.GetValue(0).ToString();
                    photo.date = reader.GetString(1);
                    photo.explanation = reader.GetString(2);
                    photo.hdurl = reader.GetValue(3).ToString();
                    photo.media_type = reader.GetString(4);
                    photo.service_version = reader.GetString(5);
                    photo.title = reader.GetString(6);
                    photo.url = reader.GetString(7);
                }
            }
            catch (Exception ex) { }

            await reader.CloseAsync();
            await conn.CloseAsync();

            return pictures;
        }

        private async Task<List<APOD>?> SaveDataDB()
        {
            List<APOD> pictures = new();
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.nasa.gov/planetary/apod?api_key=0fu6kxm8VJ28tAbk0iRAfazBSiqBW5v344fYDIiR&count=32");
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
                SqlCommand cmdr = new("SELECT * FROM [mobilesdb].dbo.thumbnails;", conn);
                cmdr.CommandType = CommandType.Text;
                SqlDataReader reader = await cmdr.ExecuteReaderAsync();

                var clone = new List<APOD>();

                try
                {
                    if (reader.Depth > 0)
                    {
                        while (reader.Read())
                        {
                            APOD picture = new();
                            picture.copyright = reader.GetValue("copyright").ToString();
                            picture.date = reader.GetString("date");
                            picture.explanation = reader.GetString("explanation");
                            picture.hdurl = reader.GetValue("hdurl").ToString();
                            picture.media_type = reader.GetString("media_type");
                            picture.service_version = reader.GetString("service_version");
                            picture.title = reader.GetString("title");
                            picture.url = reader.GetString("url");

                            for (int i = 0; i < pictures.Count; i++)
                            {
                                if (pictures[i].copyright.Equals(picture.copyright) &&
                                   pictures[i].date.Equals(picture.date) &&
                                   pictures[i].explanation.Equals(picture.explanation) &&
                                   pictures[i].hdurl.Equals(picture.hdurl) &&
                                   pictures[i].media_type.Equals(picture.media_type) &&
                                   pictures[i].service_version.Equals(picture.service_version) &&
                                   pictures[i].title.Equals(picture.title) &&
                                   pictures[i].url.Equals(picture.url))
                                {
                                    clone.Add(pictures[i]);
                                }
                            }
                        }
                    }
                }catch (Exception ex) { }

                await reader.CloseAsync();
                await conn.CloseAsync();

                for (int i = 0; i < clone.Count; i++)
                {
                    pictures.Remove(clone[i]);
                }

                conn.Open();

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
                                VALUES (@copyright, @date, @explanation, @hdurl, @media_type, @service_version, @title, @url)", conn);
                                cmd.CommandType = CommandType.Text;

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
                                VALUES (@copyright, @date, @explanation, @media_type, @service_version, @title, @url)", conn);
                                cmd.CommandType = CommandType.Text;

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
                                VALUES (@date, @explanation, @hdurl, @media_type, @service_version, @title, @url)", conn);
                                cmd.CommandType = CommandType.Text;

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
                                VALUES (@date, @explanation, @media_type, @service_version, @title, @url)", conn);
                                cmd.CommandType = CommandType.Text;

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
                }catch (SqlException sqlex) { }

                await conn.CloseAsync();
            }
            return pictures;
        }
    }
}