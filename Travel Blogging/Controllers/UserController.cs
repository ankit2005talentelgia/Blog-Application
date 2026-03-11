using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Claims;
using Travel_Blogging.DTOs.UserDto;
using Travel_Blogging.Services.Interfaces;

namespace Travel_Blogging.Controllers
{
    [Route("user")]
    public class UserController:Controller
    {
        private readonly IUserService _userService;
        private readonly IToastNotification _toastNotification;

        public UserController(IUserService userService,IToastNotification toastNotification)
        {
            _userService = userService;
            _toastNotification = toastNotification;
        }

        // this function is for when user clicks on the signup button then reroutes to the signup page
        [HttpGet("signup")]
        public IActionResult Signup()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Profile", "User");
            }
            return View();
        }

        // this controller function is for creating the user
        [HttpPost("signup")]
        public async Task<IActionResult> Signup(RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result=await _userService.CreateUser(dto);

            if (result == null)
            {
                _toastNotification.AddErrorToastMessage("this email is already registered");
                return View(dto);
            }

            _toastNotification.AddSuccessToastMessage("Successfully user registered");

            return RedirectToAction("Index","Home");
        }

        // this function is for when user clicks login button then reroutes into login page
        [HttpGet("login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Profile", "User");
            }
            return View();
        }

        // this function is for login the user
        [HttpPost("login")]
        public async Task<IActionResult>Login(LoginUserDto dto)
        {
            // if login form is not validate according to the dto
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            // when form is validated then call the proper service method for login
            var result = await _userService.LoginUser(dto);

            if (result == null)
            {
                _toastNotification.AddErrorToastMessage("User Credentials failed");
                return View(dto);
            }

            // otherwise user successfully login
            _toastNotification.AddSuccessToastMessage("Login Successfully");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            // check that if user is not logged in then redire to the login page
            if (!User.Identity.IsAuthenticated)
            {
                _toastNotification.AddWarningToastMessage("You are not logged in");
                return RedirectToAction("Login", "User");
            }

            string email = User.FindFirstValue(ClaimTypes.Email)!;
            
            string name = User.Identity.Name!;

            ViewData["name"] = name;
            ViewBag.email = email;

            return View();
        }
    }
}
