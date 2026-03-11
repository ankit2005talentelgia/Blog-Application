using Travel_Blogging.Models;

namespace Travel_Blogging.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserModel> CreateUserAsync(UserModel User);

        Task<UserModel> FindUserByEmailAsync(string email);
    }
}
