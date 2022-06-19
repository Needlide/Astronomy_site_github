using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net.Http.Headers;

namespace MVC_app_main.Views.Home
{
    public class GalleryLogic
    {
        private int totalSize { get; set; } = 0;
        private int itemsPerPage { get; set; } = 30;

        public async Task<List<ImagesGallery>> GetPhotos(int page)
        {
            SaveDataDB();

            List<ImagesGallery> images = new();
            using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            conn.Open();
            SqlCommand cmdr = new("SELECT * FROM [mobilesdb].dbo.NASAImages;", conn)
            {
                CommandType = CommandType.Text
            };
            SqlDataReader reader = await cmdr.ExecuteReaderAsync();

            try
            {
                while (reader.Read())
                {
                    JArray arrayData = (JArray)JsonConvert.DeserializeObject(reader.GetString("data"));
                    JArray arrayLinks = (JArray)JsonConvert.DeserializeObject(reader.GetString("links"));

                    Data dataObj = new()
                    {
                        description = arrayData.Values().ElementAt(0).First.ToString(),
                        title = arrayData.Values().ElementAt(1).First.ToString(),
                        photographer = arrayData.Values().ElementAt(2).First.ToString(),
                        location = arrayData.Values().ElementAt(3).First.ToString(),
                        nasa_id = arrayData.Values().ElementAt(4).First.ToString(),
                        date_created = arrayData.Values().ElementAt(5).First.ToString(),
                        /*keywords = arrayData.Values().ElementAt(6).First.ToObject<string[]>(),*/
                        media_type = arrayData.Values().ElementAt(6/*7*/).First.ToString(),
                        center = arrayData.Values().ElementAt(7/*8*/).First.ToString()
                    };

                    Links linksObj = new()
                    {
                        href = arrayLinks.Values().ElementAt(0).First.ToString(),
                        rel = arrayLinks.Values().ElementAt(1).First.ToString(),
                        render = arrayLinks.Values().ElementAt(2).First.ToString()
                    };

                    List<Data> datas = new()
                    {
                        dataObj
                    };

                    List<Links> links = new() 
                    {
                        linksObj
                    };

                    ImagesGallery image = new()
                    {
                        href = reader.GetString("href"),
                        data = datas,
                        links = links
                    };
                    images.Add(image);    
                }
            }
            catch (Exception ex) { }

            await conn.CloseAsync();
            await reader.CloseAsync();

            totalSize = images.Count;

            return images.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();
        }

        private static async void SaveDataDB()
        {
            List<ImagesGallery> nasaImages = new();

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://images-api.nasa.gov/search?year_start=2021");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(res);
                List<JToken> items = result["collection"]["items"].Children().ToList();

                foreach (JToken item in items)
                {
                    ImagesGallery n = item.ToObject<ImagesGallery>();
                    nasaImages?.Add(n);
                }
            }

            if (nasaImages != null)
            {
                using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
                conn.Open();
                SqlCommand cmdr = new("SELECT * FROM [mobilesdb].dbo.NASAImages;", conn)
                {
                    CommandType = CommandType.Text
                };
                SqlDataReader reader = await cmdr.ExecuteReaderAsync();

                try
                {
                    while (reader.Read())
                    {
                        if(reader.GetString("data") != null || reader.GetString("data") != "" || reader.GetString("links") != null || reader.GetString("links") != "")
                        {
                    //        JArray arrayData = (JArray)JsonConvert.DeserializeObject(reader.GetString("data"));
                    //        JArray arrayLinks = (JArray)JsonConvert.DeserializeObject(reader.GetString("links"));

                    //        Data dataObj = new()
                    //        {
                    //            description = arrayData.Values().ElementAt(0).First.ToString(),
                    //            title = arrayData.Values().ElementAt(1).First.ToString(),
                    //            photographer = arrayData.Values().ElementAt(2).First.ToString(),
                    //            location = arrayData.Values().ElementAt(3).First.ToString(),
                    //            nasa_id = arrayData.Values().ElementAt(4).First.ToString(),
                    //            date_created = arrayData.Values().ElementAt(5).First.ToString(),
                    //            /*keywords = arrayData.Values().ElementAt(6).First.ToObject<string[]>(),*/
                    //            media_type = arrayData.Values().ElementAt(7).First.ToString(),
                    //            center = arrayData.Values().ElementAt(8).First.ToString()
                    //        };

                    //        Links linksObj = new()
                    //        {
                    //            href = arrayLinks.Values().ElementAt(0).First.ToString(),
                    //            rel = arrayLinks.Values().ElementAt(1).First.ToString(),
                    //            render = arrayLinks.Values().ElementAt(2).First.ToString()
                    //        };

                    //        List<Data> datas = new()
                    //{
                    //    dataObj
                    //};

                    //        List<Links> links = new()
                    //{
                    //    linksObj
                    //};

                            ImagesGallery cloneImage = new()
                            {
                                href = reader.GetString("href"),
                                data = /*datas*/null,
                                links = /*links*/null
                            };

                            for (int i = 0; i < nasaImages.Count; i++)
                            {
                                if (nasaImages[i].href.Equals(cloneImage.href) /*&&
                                    nasaImages[i].data.Equals(cloneImage.data) &&
                                    nasaImages[i].links.Equals(cloneImage.links)*/)
                                {
                                    nasaImages.RemoveAt(i);
                                }
                            }
                        } 
                    }
                }
                catch (Exception ex) { }

                await reader.CloseAsync();

                try
                {
                    SqlCommand cmd = new(@"INSERT INTO [mobilesdb].dbo.NASAImages
                    (href, data, links) VALUES (@href, @data, @links)", conn)
                    {
                        CommandType = CommandType.Text
                    };

                    for (int i = 0; i < nasaImages.Count; i++)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@href", nasaImages[i].href);
                        cmd.Parameters.AddWithValue("@data", JsonConvert.SerializeObject(nasaImages[i].data));
                        cmd.Parameters.AddWithValue("@links", JsonConvert.SerializeObject(nasaImages[i].links));

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex) { }

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
