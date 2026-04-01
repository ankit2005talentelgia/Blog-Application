using Microsoft.AspNetCore.Mvc;

namespace Travel_Blogging.Controllers
{
    [Route("admin")]
    public class AdminController:Controller
    {
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
    }
}
