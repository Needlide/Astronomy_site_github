 using Microsoft.AspNetCore.Mvc;
using MVC_app_main.Models;
using MVC_app_main.Views.ViewsLogic;
using System.Diagnostics;

namespace MVC_app_main.Controllers
{
    public class HomeController : Controller
    {
        const int PAGE_RANGE = 5;

        private readonly AstroDBContext _dbContext;

        public HomeController(AstroDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet, Route("/{page:int:min(1)=1}/{sortBy=PublishedAt_desc}")]
        /// <summary>
        /// Returns Index page which contains selected by pagination and sorted by user selection elements.
        /// </summary>
        /// <param name="sortBy">By which parameter provide sorting. Default is PublishedAt.</param>
        /// <param name="page">Parameter for pagination. Default is 1.</param>
        /// <returns>ViewResult.</returns>
        public IActionResult Index(int page, string sortBy)
        {
            ThumbnailsLogic logic = new(_dbContext.Thumbnails);
            var items = logic.ToController(sortBy, page);
            long totalPages = (long)items.ElementAt(1);

            ViewBag.title = "News";
            ViewBag.data = items.ElementAt(0);
            ViewBag.size = totalPages;
            ViewBag.sortOrderP = items.ElementAt(2);
            ViewBag.sortOrderT = items.ElementAt(3);
            ViewBag.sortOrderNS = items.ElementAt(4);
            ViewBag.sortOrderU = items.ElementAt(5);
            ViewBag.startPage = Math.Max(1, page - (PAGE_RANGE));
            ViewBag.endPage = Math.Min(totalPages, page + PAGE_RANGE);
            ViewBag.currentPage = page;

            return View();
        }

        /// <summary>
        /// Returns Mars page which contains selected by pagination elements.
        /// </summary>
        /// <param name="page">Parameter for pagination. Default is 1.</param>
        /// <returns>ViewResult.</returns>
        [HttpGet, Route("Mars/{page:int:min(1)=1}/{sortBy=Sol}")]
        public IActionResult Mars(string sortBy, int page)
        {
            MarsLogic logic = new(_dbContext.Photos);
            var items = logic.ToController(sortBy, page);
            long totalPages = (long)items.ElementAt(1);

            ViewBag.title = "Mars";
            ViewBag.data = items.ElementAt(0);
            ViewBag.size = totalPages;
            ViewBag.sortOrderS = items.ElementAt(2);
            ViewBag.sortOrderED = items.ElementAt(3);
            ViewBag.startPage = Math.Max(1, page - (PAGE_RANGE));
            ViewBag.endPage = Math.Min(totalPages, page + PAGE_RANGE);
			ViewBag.currentPage = page;

			return View();
        }

        /// <summary>
        /// Returns Gallery page which contains selected by pagination elements.
        /// </summary>
        /// <param name="page">Parameter for pagination. Default is 1.</param>
        /// <returns>ViewResult.</returns>
        [HttpGet, Route("Gallery/{page:int:min(1)=1}/{sortBy=DateCreated}")]
        public IActionResult Gallery(string sortBy, int page)
        {
            GalleryLogic logic = new(_dbContext.ImagesGallery);
            var items = logic.ToController(sortBy, page);
            long totalPages = (long)items.ElementAt(1);

            ViewBag.title = "Gallery";
            ViewBag.data = items.ElementAt(0);
            ViewBag.size = totalPages;
            ViewBag.sortOrderDC = items.ElementAt(2);
            ViewBag.sortOrderT = items.ElementAt(3);
            ViewBag.sortOrderNI = items.ElementAt(4);
            ViewBag.sortOrderC = items.ElementAt(5);
            ViewBag.startPage = Math.Max(1, page - (PAGE_RANGE));
            ViewBag.endPage = Math.Min(totalPages, page + PAGE_RANGE);
            ViewBag.currentPage = page;

            return View();
        }

        /// <summary>
        /// Returns APOD (A Picture Of the Day) page which contains selected by pagination elements.
        /// </summary>
        /// <param name="page">Parameter for pagination. Default is 1.</param>
        /// <returns>ViewResult.</returns>
        [HttpGet, Route("APOD/{page:int:min(1)=1}/{sortBy=Date}")]
        public IActionResult APOD(string sortBy, int page)
        {
            ApodLogic logic = new(_dbContext.APODs);//clear APOD table, many null objects
            var items = logic.ToController(sortBy, page);
            long totalPages = (long)items.ElementAt(1);

            ViewBag.title = "APOD";
            ViewBag.data = items.ElementAt(0);
            ViewBag.size = totalPages;
            ViewBag.sortOrderD = items.ElementAt(2);
            ViewBag.sortOrderT = items.ElementAt(3);
            ViewBag.sortOrderC = items.ElementAt(4);
            ViewBag.startPage = Math.Max(1, page - (PAGE_RANGE));
            ViewBag.endPage = Math.Min(totalPages, page + PAGE_RANGE);
            ViewBag.currentPage = page;

            return View();
        }

        [HttpGet, Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
