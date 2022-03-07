using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVC_app_main.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MVC_app_main.Views.Home
{
    public class APODModel : PageModel
    {
        List<APOD> apods = new();

        public async Task<List<APOD>> GetPhotos()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.nasa.gov/planetary/apod?api_key=0fu6kxm8VJ28tAbk0iRAfazBSiqBW5v344fYDIiR&count=6");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            if(response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                apods = JsonConvert.DeserializeObject<List<APOD>>(response.Content.ReadAsStringAsync().Result);
            }
            return apods;
        }
    }
}