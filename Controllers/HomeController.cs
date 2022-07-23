using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MVC_app_main.Models;
using MVC_app_main.Views.ViewsLogic;

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

        private readonly AstroDBContext db;
        public HomeController(AstroDBContext data)
        {
            db = data;
        }

        [Route("Mars")]
        public IActionResult Mars(/*string sortBy,*/ int page)
        {
            ViewBag.Title = "Mars";
            MarsLogic logic = new();
            var items = logic.ToController(/*sortBy,*/ page);
            ViewBag.Data = items.ElementAt(0);
            ViewBag.size = items.ElementAt(1);

            return View();
        }

        [Route("Gallery")]
        public IActionResult Gallery(int page)
        {
            ViewBag.Title = "Gallery";
            GalleryLogic logic = new();
            var items = logic.ToController(page);
            ViewBag.Data = items.ElementAt(0);
            ViewBag.size = items.ElementAt(1);

            return View();
        }

        [Route("APOD")]
        public IActionResult APOD(int page)
        {
            ViewBag.Title = "APOD";
            APODLogic logic = new();
            var items = logic.ToController(page);
            ViewBag.Data = items.ElementAt(0);
            ViewBag.size = items.ElementAt(1);
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(Thumbnail thumbnail)
        //{
        //    db.thumbnails.Add(thumbnail);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}
    }
}
