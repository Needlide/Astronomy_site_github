using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using MVC_app_main.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;

namespace MVC_app_main.Views.Home
{
    public class IndexModel : PageModel
    {
        public List<Thumbnail>? Thumbnails {  get; set; }

        public async Task<List<Thumbnail>> GetThumbnails()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.spaceflightnewsapi.net/v3/articles?_limit=18");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            if (response.IsSuccessStatusCode)
            {
                Thumbnails = JsonConvert.DeserializeObject<List<Thumbnail>>(response.Content.ReadAsStringAsync().Result);
            }

            for (int i = 0; i < Thumbnails.Count; i++)
            {
                var publ = DateTime.Parse(Thumbnails[i].PublishedAt);
                publ = publ.ToUniversalTime();
                Thumbnails[i].PublishedAt = publ.ToString() + " UTC";
            }

            //using SqlConnection conn = new("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            //conn.Open();
            //SqlCommand cmd = new("[mobilesdb].dbo.sp_insert", conn);
            //cmd.CommandType = System.Data.CommandType.StoredProcedure;
            //for(int i = 0; i < Thumbnails.Count; i++)
            //{
            //    cmd.Parameters.Clear();
            //    cmd.Parameters.Add(new SqlParameter("@Title", Thumbnails[i].Title));
            //    cmd.Parameters.Add(new SqlParameter("@Url", Thumbnails[i].Url));
            //    cmd.Parameters.Add(new SqlParameter("@ImageUrl", Thumbnails[i].ImageUrl));
            //    cmd.Parameters.Add(new SqlParameter("@NewsSite", Thumbnails[i].NewsSite));
            //    cmd.Parameters.Add(new SqlParameter("@Summary", Thumbnails[i].Summary));
            //    cmd.Parameters.Add(new SqlParameter("@PublishedAt", Thumbnails[i].PublishedAt));
            //    cmd.Parameters.Add(new SqlParameter("@UpdatedAt", Thumbnails[i].UpdatedAt));
            //}
            //SqlDataReader rdr = cmd.ExecuteReader();

            //conn.Close();
            return Thumbnails;
        }
    }
}
