using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net.Http.Headers;

namespace MVC_app_main.Views.Home
{
    public class GalleryModel : PageModel
    {
        public async Task<IList<ImagesGallery>> GetPhotos()
        {
            var nasaImages = SaveDataDB().Result;

            using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            conn.Open();
            SqlCommand cmdr = new("SELECT * FROM [mobilesdb].dbo.NASAImages;", conn);
            cmdr.CommandType = CommandType.Text;
            SqlDataReader reader = await cmdr.ExecuteReaderAsync();

            try
            {
                while(reader.Read())
                {
                    ImagesGallery image = new();
                    image.href = reader.GetString("href");
                    image.data = JObject.Parse(reader.GetValue("data").ToString()).ToObject<List<Data>>();
                    image.links = JObject.Parse(reader.GetValue("links").ToString()).ToObject<List<Links>>();
                    nasaImages.Add(image);
                }
            }catch (Exception ex) { }

            await conn.CloseAsync();

            return nasaImages;
        }

        private async Task<List<ImagesGallery>?> SaveDataDB()
        {
            List<ImagesGallery> nasaImages = new();

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://images-api.nasa.gov/search?year_start=2022");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(res);
                IList<JToken> items = result["collection"]["items"].Children().ToList();

                foreach (JToken item in items)
                {
                    ImagesGallery n = item.ToObject<ImagesGallery>();
                    if (n.data != null && n.data[0].date_created != null)
                        n.data[0].date_created = DateTime.Parse(n.data[0].date_created).ToUniversalTime().ToShortDateString() + " UTC";
                    nasaImages?.Add(n);
                }
            }

            if(nasaImages != null)
            {
                using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
                conn.Open();
                SqlCommand cmdr = new("SELECT * FROM [mobilesdb].dbo.NASAImages;", conn);
                cmdr.CommandType = CommandType.Text;
                SqlDataReader reader = await cmdr.ExecuteReaderAsync();

                var clone = new List<ImagesGallery>();

                try
                {
                    while (reader.Read())
                    {
                        ImagesGallery cloneImage = new();
                        cloneImage.href = reader.GetString("href");
                        cloneImage.data = JObject.Parse(reader.GetValue("data").ToString()).ToObject<List<Data>>();
                        cloneImage.links = JObject.Parse(reader.GetValue("data").ToString()).ToObject<List<Links>>();

                        for(int i = 0; i < nasaImages.Count; i++)
                        {
                            if (nasaImages[i].href.Equals(cloneImage.href) &&
                                nasaImages[i].data.Equals(cloneImage.data) &&
                                nasaImages[i].links.Equals(cloneImage.links))
                            {
                                clone.Add(nasaImages[i]);
                            }
                        }
                    }
                }catch (Exception ex) { }

                await reader.CloseAsync();
                await conn.CloseAsync();

                for(int i = 0; i < clone.Count; i++)
                {
                    nasaImages.Remove(clone[i]);
                }

                conn.Open();

                try
                {
                    SqlCommand cmd = new(@"INSERT INTO [mobilesdb].dbo.NASAImages
                    (href, data, links) VALUES (@href, @data, @links)", conn);
                    cmd.CommandType = CommandType.Text;

                    for(int i = 0; i < nasaImages.Count; i++)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@href", nasaImages[i].href);
                        cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(nasaImages[i].data));
                        cmd.Parameters.AddWithValue("@links", JsonConvert.SerializeObject(nasaImages[i].links));

                        await cmd.ExecuteNonQueryAsync();
                    }
                }catch (Exception ex) { }
            }

            return nasaImages;
        }
    }
}