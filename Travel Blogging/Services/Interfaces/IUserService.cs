using Travel_Blogging.DTOs.UserDto;
using Travel_Blogging.Models;

namespace Travel_Blogging.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> CreateUser(RegisterUserDto dto);

        Task<UserModel> LoginUser(LoginUserDto dto);

        Task LogoutUser();

        Task<UserModel> ChangePassword(ChangePasswordDto dto, string email);

        Task DeleteAccount(int userId);

        Task<UserModel> UpdateProfile(UpdateProfileDto dto, string email, string userId);

        Task<UserModel> ConfirmUserEmail(string token, string email); // this is for when user confirm their email

        Task<UserModel> UpdateEmail(UpdateEmailDto dto, string email);
    }
}
