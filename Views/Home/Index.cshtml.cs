using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MVC_app_main.Models;
using System.Linq;
using Newtonsoft.Json;
using MVC_app_main.Controllers;

namespace MVC_app_main.Views.Home
{
    public class IndexModel : PageModel
    {
        public List<Thumbnail>? Thumbnails {  get; set; }
        async Task<List<Thumbnail>> GetThumbnails()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.spaceflightnewsapi.net/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync("v3/articles?_limit=1");
            if (response.IsSuccessStatusCode)
            {
                Thumbnails = JsonConvert.DeserializeObject<List<Thumbnail>>(response.Content.ReadAsStringAsync().Result);
            }
            return Thumbnails;
        }

        public void OnGet()
        {
            _ = GetThumbnails();
        }
    }
}
