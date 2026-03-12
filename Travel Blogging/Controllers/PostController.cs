using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NToastNotify;
using System.Security.Claims;
using Travel_Blogging.DTOs.PostDto;
using Travel_Blogging.Services.Interfaces;

namespace Travel_Blogging.Controllers
{
    [Route("post")]
    public class PostController: Controller
    {
        private readonly IToastNotification _toastNotification;
        private readonly IPostService _postService;

        public PostController(IToastNotification toastNotification,IPostService postService)
        {
            _toastNotification = toastNotification;
            _postService = postService;
        }

        // this function is for showing any specific post details
        [HttpGet("details")]
        public async Task<IActionResult> PostDetails(int id)
        {
            var post=await _postService.FindPostDetails(id);

            ViewBag.post = post;
            return View();
        }

        // this function is for showing all the posts
        [HttpGet("")]
        public async Task<IActionResult> AllPost()
        {
            var posts = await _postService.FindPosts();

            // save this data into viewdata
            ViewData["posts"] = posts;
            return View();
        }

        // this function is for showing the upload post page
        [HttpGet("upload")]
        public IActionResult UploadPost()
        {
            if (!User.Identity.IsAuthenticated)
            {
                _toastNotification.AddWarningToastMessage("Please login first");
                return RedirectToAction("Login", "User");
            }
            return View();
        }

        //this function is for accepting the upload form
        [HttpPost("upload")]
        public async Task<IActionResult> UploadPost(CreatePostDto dto)
        {

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            // check the file format
            if(dto.Image==null || dto.Image.Length == 0)
            {
                ViewBag.Error = "please select a valid image";
                return View(dto);
            }

            // validate file types
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            var extension = Path.GetExtension(dto.Image.FileName).ToLower();
            if (Array.IndexOf(allowedExtensions, extension) < 0)
            {
                ViewBag.Error = "Only jpg, jpeg, png, gif image format is valid";
                return View(dto);
            }

            // take the userid
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // call its service function 
            await _postService.CreatePost(dto, int.Parse(userId));

            _toastNotification.AddSuccessToastMessage("Successfully post uploaded");

            return RedirectToAction("AllPost", "Post");
        }
    }
}
