using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MVC_app_main.Models;
using MVC_app_main.Views.Home;

namespace MVC_app_main.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string sortBy, int page)
        {
            ThumbnailsLogic logic = new();
            var items = logic.ToController(sortBy, page);

            ViewBag.Title = "News";
            ViewBag.Data = items.ElementAt(0);
            ViewBag.Size = items.ElementAt(1);
            ViewBag.sortOrderP = items.ElementAt(2);
            ViewBag.sortOrderT = items.ElementAt(3);
            ViewBag.sortOrderNS = items.ElementAt(4);
            ViewBag.sortOrderU = items.ElementAt(5);

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
