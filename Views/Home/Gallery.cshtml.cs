using Microsoft.AspNetCore.Mvc.RazorPages;
using MVC_app_main.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace MVC_app_main.Views.Home
{
    public class GalleryModel : PageModel
    {
        //private ItemsSon? Images { get; set; }
        IList<ItemsSon> nasaImages = new List<ItemsSon>();
        public async Task<IList<ItemsSon>> GetPhotos()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://images-api.nasa.gov/search?year_start=2022");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                //res = res[res.IndexOf('[')..];
                //res = res.Remove(res.IndexOf("metadata") - 3);//перевірити
                //Images = JsonConvert.DeserializeObject<NASAImages>(res);
                JObject result = JObject.Parse(res);
                IList<JToken> items = result["collection"]["items"].Children().ToList();

                

                foreach (JToken item in items)
                {
                    ItemsSon n = item.ToObject<ItemsSon>();
                    nasaImages.Add(n);
                }
            }
            return nasaImages;
        }
    }
}