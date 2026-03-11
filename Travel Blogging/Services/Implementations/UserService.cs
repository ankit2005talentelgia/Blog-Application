using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NToastNotify;
using System.Security.Claims;
using System.Security.Principal;
using Travel_Blogging.DTOs.UserDto;
using Travel_Blogging.Models;
using Travel_Blogging.Repositories.Interfaces;
using Travel_Blogging.Services.Interfaces;

namespace Travel_Blogging.Services.Implementations
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
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

            var newUser = new UserModel
            {
                Name = dto.Name,
                Email = dto.Email
            };

            // store the hashed password in newUser object
            newUser.Password= PasswordHasher.HashPassword(newUser, dto.Password);

            var result=await _userRepository.CreateUserAsync(newUser);

            // now when user registered then make a cookie for it so that user does not goes to login page again for login

            // 1st step is to make a claim
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.Id.ToString()),
                new Claim(ClaimTypes.Email, result.Email)
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
                new Claim(ClaimTypes.Name, user.Name)
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
    }
}
