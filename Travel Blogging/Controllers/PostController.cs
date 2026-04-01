using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NToastNotify;
using System.Security.Claims;
using System.Text.Json.Serialization;
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
        [HttpGet("details/{id}")]
        public async Task<IActionResult> PostDetails(int id)
        {
            var post=await _postService.FindPostDetails(id);

            // if user logged-in then show the delete, edit review buttons so that user can easily edit/delete review
            if (User.Identity.IsAuthenticated)
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                ViewData["userId"] = userId;
            }

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
                ViewBag.Action = "UploadPost";
                _toastNotification.AddWarningToastMessage("Please login first");
                return RedirectToAction("Login", "User");
            }

            // if it is authenticated then checks that it's email is confirmed or not
            var isVerified = User.Claims
                    .FirstOrDefault(c => c.Type == "isEmailConfirmed")?.Value;

            if (isVerified == "False")
            {
                return RedirectToAction("CheckEmail", "User");
            }


            ViewBag.Action = "UploadPost";
            return View();
        }

        //this function is for accepting the upload form
        [HttpPost("upload")]
        public async Task<IActionResult> UploadPost(CreatePostDto dto, string actionType)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.Action = "UploadPost";
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
            await _postService.CreatePost(dto, int.Parse(userId), actionType);

            _toastNotification.AddSuccessToastMessage("Successfully post uploaded");

            return RedirectToAction("AllPost", "Post");
        }

        // this function is for giving all the posts of currently loggedin user
        [HttpGet("all-posts")]
        public async Task<IActionResult> UserAllPosts()
        {
            if (!User.Identity.IsAuthenticated)
            {
                _toastNotification.AddAlertToastMessage("please login first");
                return RedirectToAction("Index", "Home");
            }

            // if not verify their email
            var isVerified = User.Claims
                    .FirstOrDefault(c => c.Type == "isEmailConfirmed")?.Value;

            if (isVerified == "False")
            {
                return RedirectToAction("CheckEmail", "User");
            }


            // if verified then show
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var posts = await _postService.FindUserPosts(userId);

            ViewData["posts"] = posts;
            return View();
        }

        // this function is for deleting the post of logged in user if they wants to delete some posts
        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePost([FromBody] EditAndDeleteUserPostDto dto)
        {
            if (!User.Identity.IsAuthenticated)
            {
                _toastNotification.AddAlertToastMessage("please login first");
                return RedirectToAction("Login", "User");
            }
            
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _postService.DeleteUserPost(dto.PostId, userId);
            return Ok();
        }

        // this function is for showing the edit post page with previous data which user's uploaded already
        [HttpGet("edit/{id}")]
        public async Task<IActionResult>EditPost(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                _toastNotification.AddAlertToastMessage("please login first");
                return RedirectToAction("Login", "User");
            }

            var post = await _postService.EditPost(id);

            ViewBag.Action = "UpdatePost"; 

            return View("UploadPost", post); 
        }

        // update post
        [HttpPost("update")]
        public async Task<IActionResult> UpdatePost(CreatePostDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Action = "UpdatePost";
                return View("UploadPost", dto);
            }

            if (!User.Identity.IsAuthenticated)
            {
                _toastNotification.AddWarningToastMessage("you are not logged-in");

                return RedirectToAction("Login", "User");
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var post=await _postService.UpdatePost(dto);

            if (post == null)
            {
                _toastNotification.AddSuccessToastMessage("something went wrong");

                return RedirectToAction("AllPost", "Post");
            }

            _toastNotification.AddSuccessToastMessage("Post Updated Successfully");

            return RedirectToAction("UserAllPosts", "Post");
        }
    }
}
