using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MVC_app_main.Models;
using MVC_app_main.Views.Home;

namespace MVC_app_main.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(string sortBy)
        {
            ViewBag.Title = "News";
            IndexModel model = new();
            var thumbnails = model.GetThumbnails().Result;

            ViewBag.sortOrderT = String.IsNullOrEmpty(sortBy) ? "Title_desc" : "";
            ViewBag.sortOrderNS = sortBy == "NS" ? "NS_desc" : "NS";
            ViewBag.sortOrderP = sortBy == "P" ? "P_desc" : "P";
            ViewBag.sortOrderU = sortBy == "U" ? "U_desc" : "U"; 

            thumbnails = sortBy switch
            {
                "Title_desc" => thumbnails = thumbnails.OrderByDescending(s => s.Title).ToList(),
                "NS" => thumbnails = thumbnails.OrderBy(s => s.NewsSite).ToList(),
                "NS_desc" => thumbnails = thumbnails.OrderByDescending(s => s.NewsSite).ToList(),
                "P" => thumbnails = thumbnails.OrderBy(s => s.PublishedAt).ToList(),
                "P_desc" => thumbnails = thumbnails.OrderByDescending(s => s.PublishedAt).ToList(),
                "U" => thumbnails = thumbnails.OrderBy(s => s.UpdatedAt).ToList(),
                "U_desc" => thumbnails = thumbnails.OrderByDescending(s => s.UpdatedAt).ToList(),
                _ => thumbnails = thumbnails.OrderBy(s => s.Title).ToList(),
            };

            ViewBag.Data = thumbnails;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private DataBase db;
        public HomeController(DataBase data)
        {
            db = data;
        }

        [Route("Mars")]
        public async Task<IActionResult> Mars()
        {
            ViewBag.Title = "Mars";
            MarsModel model = new();
            var res = await model.GetPhotos();
            ViewBag.Data = res;
            return View();
        }

        [Route("Gallery")]
        public async Task<IActionResult> Gallery()
        {
            ViewBag.Title = "Gallery";
            GalleryModel model = new();
            var res = await model.GetPhotos();
            ViewBag.Data = res;
            return View();
        }

        [Route("APOD")]
        public async Task<IActionResult> APOD()
        {
            ViewBag.Title = "APOD";
            APODModel model = new();
            var res = await model.GetPhotos();
            ViewBag.Data = res;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Thumbnail thumbnail)
        {
            db.thumbnails.Add(thumbnail);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
