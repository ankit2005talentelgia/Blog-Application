using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Travel_Blogging.Models;
using Travel_Blogging.Services.Interfaces;

namespace Travel_Blogging.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostService _service;

        public HomeController(IPostService service)
        {
            _service = service;
        }

        
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var posts = await _service.FindLatestPosts();

            ViewData["posts"] = posts;

            return View();
        }


        //[HttpGet("privacy")]
        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
