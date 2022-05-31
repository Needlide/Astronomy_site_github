using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MVC_app_main.Models;
using MVC_app_main.Views.Home;
using Microsoft.AspNetCore.Components.Forms;

namespace MVC_app_main.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string sortBy, int page)
        {
            ViewBag.Title = "News";
            ThumbnailsLogic model = new();
            var thumbnails = model.GetThumbnails(page).Result;

            ViewBag.sortOrderP = String.IsNullOrEmpty(sortBy) ? "P" : "";
            ViewBag.sortOrderT = sortBy == "Title" ? "Title_desc" : "Title";
            ViewBag.sortOrderNS = sortBy == "NS" ? "NS_desc" : "NS";
            ViewBag.sortOrderU = sortBy == "U" ? "U_desc" : "U";

            thumbnails = sortBy switch
            {
                "Title" => thumbnails = thumbnails.OrderBy(s => s.Title).ToList(),
                "Title_desc" => thumbnails = thumbnails.OrderByDescending(s => s.Title).ToList(),
                "NS" => thumbnails = thumbnails.OrderBy(s => s.NewsSite).ToList(),
                "NS_desc" => thumbnails = thumbnails.OrderByDescending(s => s.NewsSite).ToList(),
                "P" => thumbnails = thumbnails.OrderBy(s => s.PublishedAt).ToList(),
                "U" => thumbnails = thumbnails.OrderBy(s => s.UpdatedAt).ToList(),
                "U_desc" => thumbnails = thumbnails.OrderByDescending(s => s.UpdatedAt).ToList(),
                _ => thumbnails = thumbnails.OrderByDescending(s => s.PublishedAt).ToList(),
            };

            ViewBag.size = Math.Floor((decimal)model.totalSize / 50);
            ViewBag.Data = thumbnails;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private readonly DataBase db;
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
