using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Travel_Blogging.Models;

namespace Travel_Blogging.Controllers
{
    public class HomeController : Controller
    {
        
        [HttpGet("")]
        public IActionResult Index()
        {
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
