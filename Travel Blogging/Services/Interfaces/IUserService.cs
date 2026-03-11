using Travel_Blogging.DTOs.UserDto;
using Travel_Blogging.Models;

namespace Travel_Blogging.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> CreateUser(RegisterUserDto dto);

        Task<UserModel> LoginUser(LoginUserDto dto);
    }
}
