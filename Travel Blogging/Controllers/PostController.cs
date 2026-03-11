using Microsoft.AspNetCore.Mvc;

namespace Travel_Blogging.Controllers
{
    [Route("post")]
    public class PostController: Controller
    {
        [HttpGet("details")]
        public IActionResult PostDetails()
        {
            return View();
        }
    }
}
