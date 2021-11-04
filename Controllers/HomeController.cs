using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MVC_app_main.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MVC_app_main.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await db.thumbnails.ToListAsync());
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

        [HttpPost]
        public async Task<IActionResult> Create(Thumbnail thumbnail)
        {
            db.thumbnails.Add(thumbnail);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
