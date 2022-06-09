using Microsoft.Data.SqlClient;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;

namespace MVC_app_main.Views.Home
{
    public class MarsLogic
    {
        private int totalSize { get; set; } = 0;

        private async Task<List<Photos>> GetPhotos(int page)
        {
            var Photos = SaveDataDB().Result;
            using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            conn.Open();
            SqlCommand cmdr = new("SELECT * FROM [mobilesdb].dbo.photos", conn)
            {
                CommandType = CommandType.Text
            };
            SqlDataReader reader = await cmdr.ExecuteReaderAsync();

            try
            {
                while (reader.Read())
                {
                    Photos photo = new()
                    {
                        Sol = reader.GetString("Sol"),
                        Camera = (IDictionary<string, string>)JsonConvert.DeserializeObject(reader.GetValue("Camera").ToString()),
                        Img_src = reader.GetString("Img_src"),
                        Earth_date = reader.GetString("Earth_date"),
                        Rover = (IDictionary<string, string>)JsonConvert.DeserializeObject(reader.GetValue("Rover").ToString())
                    };
                    Photos.Add(photo);
                }
            }
            catch (Exception ex) { }



            return Photos;
        }

        private async Task<List<Photos>?> SaveDataDB()
        {
            List<Photos> Photos = new();
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos?sol=1000&api_key=0fu6kxm8VJ28tAbk0iRAfazBSiqBW5v344fYDIiR");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                res = res.Remove(0, 10);
                res = res.Remove(res.Length - 1, 1);
                Photos = JsonConvert.DeserializeObject<List<Photos>>(res);
            }

            if (Photos != null)
            {
                using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
                conn.Open();
                SqlCommand cmdr = new("SELECT * FROM [mobilesdb].dbo.photos", conn)
                {
                    CommandType = CommandType.Text
                };
                SqlDataReader reader = await cmdr.ExecuteReaderAsync();

                var clone = new List<Photos>();

                try
                {
                    while (reader.Read())
                    {
                        Photos photo = new()
                        {
                            Sol = reader.GetString("Sol"),
                            Camera = (IDictionary<string, string>)JsonConvert.DeserializeObject(reader.GetValue("Camera").ToString()),
                            Img_src = reader.GetString("Img_src"),
                            Earth_date = reader.GetString("Earth_date"),
                            Rover = (IDictionary<string, string>)JsonConvert.DeserializeObject(reader.GetValue("Rover").ToString())
                        };

                        for (int i = 0; i < Photos.Count; i++)
                        {
                            if (Photos[i].Sol.Equals(photo.Sol) &&
                                Photos[i].Camera.Equals(photo.Camera) &&
                                Photos[i].Img_src.Equals(photo.Img_src) &&
                                Photos[i].Earth_date.Equals(photo.Earth_date) &&
                                Photos[i].Rover.Equals(photo.Rover))
                            {
                                clone.Add(Photos[i]);
                            }
                        }
                    }
                }
                catch (Exception ex) { }

                await reader.CloseAsync();
                await conn.CloseAsync();

                for (int i = 0; i < clone.Count; i++)
                {
                    Photos.Remove(clone[i]);
                }

                conn.Open();

                try
                {
                    SqlCommand cmd = new(@"INSERT INTO [mobilesdb].dbo.photos
                    (Sol, Camera, Img_src, Earth_date, Rover)
                    VALUES (@Sol, @Camera, @Img_src, @Earth_date, @Rover)", conn)
                    {
                        CommandType = CommandType.Text
                    };

                    for (int i = 0; i < Photos.Count; i++)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Sol", Photos[i].Sol);
                        cmd.Parameters.AddWithValue("@Camera", JsonConvert.SerializeObject(Photos[i].Camera));
                        cmd.Parameters.AddWithValue("@Img_src", Photos[i].Img_src);
                        cmd.Parameters.AddWithValue("@Earth_date", Photos[i].Earth_date);
                        cmd.Parameters.AddWithValue("@Rover", JsonConvert.SerializeObject(Photos[i].Rover));

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex) { }

                await conn.CloseAsync();
            }
            return Photos;
        }

        public List<Object> ToController(string sortBy, int page)
        {
            List<Object> list = new();
            string sortOrderP = String.Empty, sortOrderT = String.Empty, sortOrderNS = String.Empty, sortOrderU = String.Empty;
            decimal size = 0;

            var photos = GetPhotos(page).Result;

            return list;
        }
    }
}
