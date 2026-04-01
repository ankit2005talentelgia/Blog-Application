using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Claims;
using System.Security.Principal;
using Travel_Blogging.DTOs.UserDto;
using Travel_Blogging.Models;
using Travel_Blogging.Repositories.Interfaces;
using Travel_Blogging.Services.Interfaces;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace Travel_Blogging.Services.Implementations
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IReviewRepository _commentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IReviewRepository commentRepository, IWebHostEnvironment webHostEnvironment)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _commentRepository = commentRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // this function is for creating the user
        public async Task<UserModel> CreateUser(RegisterUserDto dto)
        {
            // first check that the new user's email is already registered or not in the db
            var user = await _userRepository.FindUserByEmailAsync(dto.Email);

            if (user!=null)
            {
                return null;
            }

            // initialise the hasher password for hashing the user's password
            var PasswordHasher = new PasswordHasher<UserModel>();

            // generate a token
            var token = Guid.NewGuid().ToString();

            var newUser = new UserModel
            {
                Name = dto.Name,
                Email = dto.Email,
                IsEmailConfirmed = false,              
                EmailConfirmationToken = token
            };

            // store the hashed password in newUser object
            newUser.Password= PasswordHasher.HashPassword(newUser, dto.Password);

            var result=await _userRepository.CreateUserAsync(newUser);

            // After successfully makes the profile then make a cookie for it so that user does not goes to login page again for login

            // 1st step is to make a claim
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.Id.ToString()),
                new Claim(ClaimTypes.Email, result.Email!),
                new Claim(ClaimTypes.Name, result.Name!),
                new Claim("isEmailConfirmed", result.IsEmailConfirmed.ToString())
            };

            // 2nd step is to make a identity
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 3rd step is to make a principle with identity
            var principal = new ClaimsPrincipal(identity);

            // 4th step is to make a cookie with httpcontext.signin methods
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);
            }


            return result;
        }

        // verify the email of the user when user click the link which is send on their email
        public async Task<UserModel> ConfirmUserEmail(string token, string email)
        {
            var user = await _userRepository.FindUserByEmailAsync(email);
            if (user == null || user.EmailConfirmationToken != token)
                return null;

            // now update in the database that user is confirmed
            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;

            await _userRepository.UpdateProfileAsync();

            // remove the cookie from the browser because it stores old data
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            // make the cookie with new data and make new cookie with httpcontext.signin method
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("isEmailConfirmed", user.IsEmailConfirmed.ToString())
            };

            // make identity
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // make principal
            var principal = new ClaimsPrincipal(identity);

            if (context != null)
            {
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }

            return user;
        }



        // this function is for login the user
        public async Task<UserModel>LoginUser(LoginUserDto dto)
        {
            var user =await _userRepository.FindUserByEmailAsync(dto.Email);
            // if user is not registered in this email then return null
            if (user == null)
            {
                return null;
            }

            // if user is registered then verify its password which is stored in the database
            var hasher = new PasswordHasher<UserModel>();
            var result = hasher.VerifyHashedPassword(user, user.Password, dto.Password);
            // when password is wrong then return null for credential failed
            if (result != PasswordVerificationResult.Success)
            {
                return null;
            }

            // now if password is correct then make a cookie for authenticate the user's identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("isEmailConfirmed", user.IsEmailConfirmed.ToString())
            };

            // make identity
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // make principal
            var principal = new ClaimsPrincipal(identity);

            // make cookie with httpcontext.signin method
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }

            // now return the user 
            return user;
        }


        // this function is for logout the user
        public async Task LogoutUser()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // clear the session if anything is in the browser's session
                //context.Session.Clear();
            }
        }


        // this function is for change the password of current user loggedin 
        public async Task<UserModel>ChangePassword(ChangePasswordDto dto, string email)
        {
            var user = await _userRepository.FindUserByEmailAsync(email);

            // verify the current password with db user's password
            var hasher =new PasswordHasher<UserModel>();

            var result = hasher.VerifyHashedPassword(user, user.Password, dto.CurrentPassword);
            if (result != PasswordVerificationResult.Success)
            {
                return null;
            }

            user.Password = hasher.HashPassword(user, dto.NewPassword);
            await _userRepository.ChangePasswordAsync();

            return user;
        }


        // this function is for deleting the user's account from the database
        public async Task DeleteAccount(int userId)
        {
            // find all the posts from the post db
            var posts = await _userRepository.FindPostsByUserIdAsync(userId);

            // delete all the images which is created by user when user's accound is deleted
            foreach (var post in posts)
            {
                if (!string.IsNullOrEmpty(post.ImageUrl))
                {
                    string imagePath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        post.ImageUrl.TrimStart('/')
                    );

                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }
            }

            // delete the user's account by calling the userRepo
            await _userRepository.DeleteAccountAsync(userId);

            // now with deleting the user it's all post is automatically deleted from the database (EF deletes by Foreign key)
            // but user's comments on other's post is not automatically deleted by EF bcz post and comment both connects with the user
            // so EF executes the deletion command through 2 way --> user->post->comment and user->comment so this gives error
            // that's why we manually handle this by saying EF to delete only through post way and deletes comment by manually
            await _commentRepository.DeleteComments(userId);

            // deletes the cookies which is stored in the browser
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }


        // this function is for updating the user's profile
        public async Task<UserModel>UpdateProfile(UpdateProfileDto dto, string email, string userId)
        {
            var user = await _userRepository.FindUserByEmailAsync(email);
            
            user.Name=dto.Name;
            // check that email is different
            if (user.Email != dto.Email)
            {
                user.Email = dto.Email;

                // generate a token
                var token = Guid.NewGuid().ToString();

                user.IsEmailConfirmed = false;
                user.EmailConfirmationToken = token;
            }

            // now call the user repository for saving the latest data in the database
            await _userRepository.UpdateProfileAsync();

            // remove the cookie from the browser because it stores old data
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            // make the cookie with new data and make new cookie with httpcontext.signin method
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, dto.Email),
                new Claim(ClaimTypes.Name, dto.Name),
                new Claim("isEmailConfirmed", user.IsEmailConfirmed.ToString())
            };

            // make identity
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // make principal
            var principal = new ClaimsPrincipal(identity);

            if (context != null)
            {
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }

            return user;
        }


        // this function is for updating the email when user register with wrong email
        public async Task<UserModel> UpdateEmail(UpdateEmailDto dto, string email)
        {
            var user = await _userRepository.FindUserByEmailAsync(email);

            user.Email = dto.Email;

            // update email in the db and from the controller just call the email-verification function
            await _userRepository.UpdateProfileAsync();

            return user;
        }
    }
}
