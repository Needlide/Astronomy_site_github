﻿using Microsoft.Data.SqlClient;
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
                    JArray dataJ = (JArray)JsonConvert.DeserializeObject(reader.GetString("data"));
                    JArray linksJ = (JArray)JsonConvert.DeserializeObject(reader.GetString("links"));

                    //var item = JObject.Parse(dataJ.Children().First().Children().ToString()).ToObject(typeof(Data));

                    ImagesGallery image = new()
                    {
                        href = reader.GetString("href"),
                        /*data = JObject.Parse(reader.GetString("data")).ToObject<List<Data>>(),
                        links = JObject.Parse(reader.GetString("links")).ToObject<List<Links>>()*/
                        /*data = dataJ.Children().First().Children(),
                        links = linksJ.Children(),*/
                    };
                    images.Add(image);
                }
            }
            catch (Exception ex) { }

            await conn.CloseAsync();
            await reader.CloseAsync();

            totalSize = images.Count;

            return images.Skip((page - 1) * 50).Take(50).ToList();
        }

        private static async void SaveDataDB()
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
                List<JToken> items = result["collection"]["items"].Children().ToList();

                foreach (JToken item in items)
                {
                    ImagesGallery n = item.ToObject<ImagesGallery>();
                    if (n.data != null && n.data[0].date_created != null)
                        n.data[0].date_created = n.data[0].date_created.Value.ToUniversalTime();
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
                        ImagesGallery cloneImage = new()
                        {
                            href = reader.GetString("href"),
                            data = JObject.Parse(reader.GetValue("data").ToString()).ToObject<List<Data>>(),
                            links = JObject.Parse(reader.GetValue("data").ToString()).ToObject<List<Links>>()
                        };

                        for (int i = 0; i < nasaImages.Count; i++)
                        {
                            if (nasaImages[i].href.Equals(cloneImage.href) &&
                                nasaImages[i].data.Equals(cloneImage.data) &&
                                nasaImages[i].links.Equals(cloneImage.links))
                            {
                                nasaImages.RemoveAt(i);
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

            decimal size = Math.Floor((decimal)totalSize / 50) % 2 == 0 ? Math.Floor((decimal)totalSize / 50) : Math.Floor((decimal)totalSize / 50) + 1;
            list.Add(images);
            list.Add(size);

            return list;
        }
    }
}