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

            while(reader.Read())
            {
                APOD photo = new();
                photo.copyright = reader.GetString(1);
                photo.date = reader.GetString(2);
                photo.explanation = reader.GetString(3);
                photo.hdurl = reader.GetString(4);
                photo.media_type = reader.GetString(5);
                photo.service_version = reader.GetString(6);
                photo.title = reader.GetString(7);
                photo.url = reader.GetString(8);
            }

            conn.Close();

            return pictures;
        }

        private async Task<List<APOD>?> SaveDataDB()
        {
            List<APOD> pictures = new();
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.nasa.gov/planetary/apod?api_key=0fu6kxm8VJ28tAbk0iRAfazBSiqBW5v344fYDIiR&count=16");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            if (response.IsSuccessStatusCode)
            {
                pictures = JsonConvert.DeserializeObject<List<APOD>>(response.Content.ReadAsStringAsync().Result);
            }

            if(pictures != null)
            {
                using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
                conn.Open();
                SqlCommand cmdr = new("SELECT * FROM [mobilesdb].dbo.thumbnails;", conn);
                cmdr.CommandType = CommandType.Text;
                SqlDataReader reader = await cmdr.ExecuteReaderAsync();

                var clone = new List<APOD>();

                if(reader.Depth > 0)
                {
                    while (reader.Read())
                    {
                        APOD picture = new();
                        picture.copyright = reader.GetString("copyright");
                        picture.date = reader.GetString("date");
                        picture.explanation = reader.GetString("explanation");
                        picture.hdurl = reader.GetString("hdurl");
                        picture.media_type = reader.GetString("media_type");
                        picture.service_version = reader.GetString("service_version");
                        picture.title = reader.GetString("title");
                        picture.url = reader.GetString("url");

                        for(int i = 0; i < pictures.Count; i++)
                        {
                            if(pictures[i].copyright.Equals(picture.copyright) && 
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

                conn.Close();

                for(int i = 0; i < clone.Count; i++)
                {
                    pictures.Remove(clone[i]);
                }

                conn.Open();

                SqlCommand cmd = new(@"INSERT INTO [mobilesdb].dbo.APOD
                (Id, copyright, date, explanation, hdurl, media_type, service_version, title, url)
                VALUES (@copyright, @date, @explanation, @hdurl, @media_type, @service_version, @title, @url)", conn);
                cmd.CommandType = CommandType.Text;

                for(int i = 0; i < pictures.Count; i++)
                {
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

                conn.Close();
            }
            return pictures;//null on page
        }
    }
}