using Microsoft.AspNetCore.Mvc.RazorPages;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MVC_app_main.Views.Home
{
    public class MarsModel : PageModel
    {
        private List<Photos> Photos { get; set; }

        public async Task<List<Photos>> GetPhotos()
        {
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
            return Photos;
        }
    }
}
