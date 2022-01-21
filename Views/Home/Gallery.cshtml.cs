using Microsoft.AspNetCore.Mvc.RazorPages;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MVC_app_main.Views.Home
{
    public class GalleryModel : PageModel
    {
        private NASAImages? Images { get; set; }

        public async Task<NASAImages> GetPhotos()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://images-api.nasa.gov/search?year_start=2022");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                res = res[res.IndexOf('[')..];
                res = res.Remove(res.IndexOf("metadata") - 2);//перевірити
                Images = JsonConvert.DeserializeObject<NASAImages>(res);
            }
            return Images;
        }
    }
}