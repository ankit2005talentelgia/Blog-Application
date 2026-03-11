using Microsoft.EntityFrameworkCore;
using Travel_Blogging.Data;
using Travel_Blogging.Models;
using Travel_Blogging.Repositories.Interfaces;

namespace Travel_Blogging.Repositories.Implementations
{
    public class UserRepository:IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // this function is for creating the new user
        public async Task<UserModel> CreateUserAsync(UserModel user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // this function is for find the user by email
        public async Task<UserModel>FindUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }
    }
}
