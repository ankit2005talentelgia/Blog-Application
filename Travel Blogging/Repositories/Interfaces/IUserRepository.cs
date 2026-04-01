using Travel_Blogging.DTOs.UserDto;
using Travel_Blogging.Models;

namespace Travel_Blogging.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserModel> CreateUserAsync(UserModel User);

        Task<UserModel> FindUserByEmailAsync(string email);

        Task ChangePasswordAsync();

        Task DeleteAccountAsync(int userId);

        Task<List<PostModel>> FindPostsByUserIdAsync(int userId);

        Task UpdateProfileAsync();  // it saves the updated data of the user
    }
}
