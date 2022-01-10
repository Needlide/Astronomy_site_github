﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MVC_app_main.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using MVC_app_main.Views.Home;

namespace MVC_app_main.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "News";
            IndexModel model = new();
            var res = await model.GetThumbnails();
            ViewBag.Data = res;
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
        public IActionResult Gallery()
        {
            ViewBag.Title = "Gallery";
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
