using Microsoft.AspNetCore.Mvc;
using MVC_app_main.Models;
using MVC_app_main.Views.ViewsLogic;
using System.Diagnostics;

namespace MVC_app_main.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet, Route("/{page:int:min(1)=1}/{sortBy?}")]
        /// <summary>
        /// Returns Index page which contains selected by pagination and sorted by user selection elements.
        /// </summary>
        /// <param name="sortBy">By which parameter provide sorting. Default is PublishedAt.</param>
        /// <param name="page">Parameter for pagination. Default is 1.</param>
        /// <returns>ViewResult.</returns>
        public IActionResult Index(int page, string sortBy)
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

        /// <summary>
        /// Returns Mars page which contains selected by pagination elements.
        /// </summary>
        /// <param name="page">Parameter for pagination. Default is 1.</param>
        /// <returns>ViewResult.</returns>
        [HttpGet, Route("Mars/{page:int:min(1)=1}/{sortBy?}")]
        [Route("Mars")]
        public IActionResult Mars(string sortBy, int page)
        {
            ViewBag.Title = "Mars";
            MarsLogic logic = new();
            var items = logic.ToController(sortBy, page);
            ViewBag.Data = items.ElementAt(0);
            ViewBag.size = items.ElementAt(1);
            ViewBag.sortOrderS = items.ElementAt(2);
            ViewBag.sortOrderED = items.ElementAt(3);

            return View();
        }

        /// <summary>
        /// Returns Gallery page which contains selected by pagination elements.
        /// </summary>
        /// <param name="page">Parameter for pagination. Default is 1.</param>
        /// <returns>ViewResult.</returns>
        [HttpGet, Route("Gallery/{page:int:min(1)=1}/{sortBy?}")]
        [Route("Gallery")]
        public IActionResult Gallery(string sortBy, int page)
        {
            ViewBag.Title = "Gallery";
            GalleryLogic logic = new();
            var items = logic.ToController(sortBy, page);
            ViewBag.Data = items.ElementAt(0);
            ViewBag.size = items.ElementAt(1);
            ViewBag.sortOrderDC = items.ElementAt(2);
            ViewBag.sortOrderT = items.ElementAt(3);
            ViewBag.sortOrderNI = items.ElementAt(4);
            ViewBag.sortOrderC = items.ElementAt(5);

            return View();
        }

        /// <summary>
        /// Returns APOD (A Picture Of the Day) page which contains selected by pagination elements.
        /// </summary>
        /// <param name="page">Parameter for pagination. Default is 1.</param>
        /// <returns>ViewResult.</returns>
        [HttpGet, Route("APOD/{page:int:min(1)=1}/{sortBy?}")]
        [Route("APOD")]
        public IActionResult APOD(string sortBy, int page)
        {
            ViewBag.Title = "APOD";
            ApodLogic logic = new();
            var items = logic.ToController(sortBy, page);
            ViewBag.Data = items.ElementAt(0);
            ViewBag.size = items.ElementAt(1);
            ViewBag.sortOrderD = items.ElementAt(2);
            ViewBag.sortOrderT = items.ElementAt(3);
            ViewBag.sortOrderC = items.ElementAt(4);

            return View();
        }

        [HttpGet, Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private readonly AstroDBContext db;
        /// <summary>
        /// Constructor which initializes database context in the controller class.
        /// </summary>
        /// <param name="data">database context.</param>
        public HomeController(AstroDBContext data)
        {
            db = data;
        }
    }
}
