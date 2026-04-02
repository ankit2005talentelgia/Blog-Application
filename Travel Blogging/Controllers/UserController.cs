using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Claims;
using Travel_Blogging.DTOs.UserDto;
using Travel_Blogging.Services.Implementations;
using Travel_Blogging.Services.Interfaces;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace Travel_Blogging.Controllers
{
    [Route("user")]
    public class UserController:Controller
    {
        private readonly IUserService _userService;
        private readonly IToastNotification _toastNotification;
        private readonly IEmailService _emailService;

        public UserController(IUserService userService,IToastNotification toastNotification, IEmailService emailService)
        {
            _userService = userService;
            _toastNotification = toastNotification;
            _emailService = emailService;
        }

        // this function is for when user clicks on the signup button then reroutes to the signup page
        [HttpGet("signup")]
        public IActionResult Signup()
        {
            if (User.Identity.IsAuthenticated)
            {
                var isVerified = User.Claims
                    .FirstOrDefault(c => c.Type == "isEmailConfirmed")?.Value;

                if (isVerified == "False")
                {
                    return View("CheckEmail");
                }

                // if user is verified then give the permission for profile page
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

            // generate a confirmation link
            var confirmationLink = Url.Action(
                "ConfirmEmail",     // method name
                "User",             // controller name
                new { token = result.EmailConfirmationToken, email = result.Email },
                Request.Scheme      // http / https
            );

            // send confirmation link on the user's email
            await _emailService.SendEmailAsync(
                result.Email,
                "Confirm your email",
                $"<p>Click here: <a href='{confirmationLink}'>Confirm Email</a></p>"
            );


            _toastNotification.AddSuccessToastMessage("Successfully user registered");

            return View("CheckEmail");
        }


        // this function is for showing the UI to the user to verify their email
        [Authorize]
        [HttpGet("CheckEmail")]
        public IActionResult CheckEmail()
        {
            if (User.Identity.IsAuthenticated)
            {
                var isVerified = User.Claims
                    .FirstOrDefault(c => c.Type == "isEmailConfirmed")?.Value;

                if (isVerified == "False")
                {
                    return View();
                }

                // if user is verified then redirect to the profile page
                return RedirectToAction("Profile", "User");
            }
            return View("Login");
        }

        // after signup confirm user's email when user click the confirmation link
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userService.ConfirmUserEmail(token, email);

            if (user == null)
            {
                return View("error");
            }

            return RedirectToAction("Index", "Home");
        }


        // this function is for showing the UI page of update email when user register with wrong email
        [Authorize]
        [HttpGet("update-email")]
        public IActionResult UpdateEmail()
        {
            if (User.Identity.IsAuthenticated)
            {
                var isVerified = User.Claims
                    .FirstOrDefault(c => c.Type == "isEmailConfirmed")?.Value;

                if (isVerified == "False")
                {
                    return View();
                }

                // if user is verified then give the permission for profile page
                return RedirectToAction("Profile", "User");
            }

            _toastNotification.AddWarningToastMessage("please login first");
            return RedirectToAction("Login", "User");
        }


        // this function is for updating the email when user updates their email after register with wrong email
        [HttpPost("update-email")]
        public async Task<IActionResult> UpdateEmail(UpdateEmailDto dto)
        {
            // check the user is authenticated or not
            if (!User.Identity.IsAuthenticated)
            {
                _toastNotification.AddWarningToastMessage("please login first");
                return RedirectToAction("Login", "User");
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            // check that if user is already verified then redirect to the profile
            if(User.Claims.FirstOrDefault(c => c.Type == "isEmailConfirmed")?.Value != "False")
            {
                return RedirectToAction("Profile", "User");
            }

            
            var email = User.FindFirstValue(ClaimTypes.Email);
            // if the user gives same email then gives the warning
            if (email == dto.Email)
            {
                _toastNotification.AddWarningToastMessage("This email is same as the registered email");
                return View(dto);
            }

            
            // now if email is different then register this email
            var result=await _userService.UpdateEmail(dto, email);
            
            // now just send the email-verification link to the user and it automatically update all the things and also
            // generate cookie with updated data

            // generate a confirmation link
            var confirmationLink = Url.Action(
                "ConfirmEmail",     // method name
                "User",             // controller name
                new { token = result.EmailConfirmationToken, email = result.Email },
                Request.Scheme      // http / https
            );

            // send confirmation link on the user's email
            await _emailService.SendEmailAsync(
                result.Email,
                "Confirm your email",
                $"<p>Click here: <a href='{confirmationLink}'>Confirm Email</a></p>"
            );


            _toastNotification.AddSuccessToastMessage("please verify the email");

            return RedirectToAction("CheckEmail", "User");
        }


        // this function is for when user clicks login button then reroutes into login page
        [HttpGet("login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                var isVerified = User.Claims
                    .FirstOrDefault(c => c.Type == "isEmailConfirmed")?.Value;

                if (isVerified == "False")
                {
                    return View("CheckEmail");
                }

                // if user is verified then give the permission for profile page
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

        // this function is for loading the profile page with default user's name and email
        [Authorize]
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

            ViewBag.Section = "Profile";
            return View(new ChangePasswordDto());
        }

        // this function is for logout the user
        [Authorize]
        [HttpGet("logout")]
        async public Task<IActionResult> Logout()
        {
            await _userService.LogoutUser();

            _toastNotification.AddSuccessToastMessage("logout successfully");
            return RedirectToAction("Index", "Home");
        }

        // this function is for loading the change password section of profile page when user's click changepassword btn
        [Authorize]
        [HttpGet("change-password")]
        public IActionResult ChangePasswordPage()
        {
            if (!User.Identity.IsAuthenticated)
            {
                _toastNotification.AddWarningToastMessage("You are not logged in");
                return RedirectToAction("Login", "User");
            }

            // if user cannot verify their email then show verify your email first
            var isVerified = User.Claims
                    .FirstOrDefault(c => c.Type == "isEmailConfirmed")?.Value;
            if (isVerified == "False")
            {
                return RedirectToAction("CheckEmail", "User");
            }

            // if already verified their email then gives required permission
            string email = User.FindFirstValue(ClaimTypes.Email)!;
            string name = User.Identity.Name!;

            // send the same that to the profile page because in frontend page is same so it expect some data (without this in frontend it show error)
            ViewData["name"] = name;
            ViewBag.email = email;

            ViewBag.Section = "ChangePassword";
            return View("Profile", new ChangePasswordDto());
        }

        // this function is for changing the password of current loggedin user
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            // after adding partial view profile page expect some data that's why
            if (!ModelState.IsValid) 
            { 
                ViewBag.Section = "ChangePassword"; 
                ViewData["name"] = User.Identity.Name!; 
                ViewBag.email = email; 
                return View("Profile", dto); 
            }

            var result = await _userService.ChangePassword(dto, email);

            _toastNotification.AddSuccessToastMessage("password change successfully");
            return RedirectToAction("Profile", "User");
        }


        // this function is for deleting the user's profile from database
        [Authorize]
        [HttpGet("profile/delete")]
        public async Task<IActionResult> DeleteProfile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                _toastNotification.AddAlertToastMessage("please login first");
                return RedirectToAction("Login", "User");
            }

            // first check that their email is verified or not
            var isVerified = User.Claims
                    .FirstOrDefault(c => c.Type == "isEmailConfirmed")?.Value;
            if (isVerified == "False")
            {
                return View("CheckEmail");
            }

            // now delete the user from database
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _userService.DeleteAccount(int.Parse(userId));

            _toastNotification.AddSuccessToastMessage("Successfully deleted user's account");
            return RedirectToAction("Index", "Home");
        }


        // this function is for edit the profile section of the user
        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateProfileDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                ViewBag.Section = "ChangePassword";
                ViewData["name"] = User.Identity.Name;
                ViewBag.email = email;
                return View("Profile", dto);
            }

            var result=await _userService.UpdateProfile(dto, email, userId);

            if (email != dto.Email)
            {
                // generate a confirmation link
                var confirmationLink = Url.Action(
                    "ConfirmEmail",     // method name
                    "User",             // controller name
                    new { token = result.EmailConfirmationToken, email = result.Email },
                    Request.Scheme      // http / https
                );

                // send confirmation link on the user's email
                await _emailService.SendEmailAsync(
                    result.Email,
                    "Confirm your email",
                    $"<p>Click here: <a href='{confirmationLink}'>Confirm Email</a></p>"
                );


                _toastNotification.AddSuccessToastMessage("please verify the email");

                return Ok();
            }
            
            _toastNotification.AddSuccessToastMessage("profile update successfully");
            
            return Ok();
        }
    }
}
