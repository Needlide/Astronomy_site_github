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

            ViewBag.sortOrderT = String.IsNullOrEmpty(sortBy) ? "Title" : "";

            thumbnails = ViewBag.sortOrderT switch
            {
                "Title" => thumbnails = thumbnails.OrderBy/*Descending*/(s => s.Title).ToList(),
                _ => thumbnails = thumbnails.OrderByDescending(s => s.Title).ToList(),
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
