using Microsoft.EntityFrameworkCore;
using Travel_Blogging.Data;
using Travel_Blogging.DTOs.UserDto;
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

        // this function is for change the password of loggedin user
        public async Task ChangePasswordAsync()
        {
            await _context.SaveChangesAsync();
        }

        //this function is for deleting the user from the database
        public async Task DeleteAccountAsync(int userId)
        {
            var user=await _context.Users.FindAsync(userId);

            user.IsDeleted = true;
            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }

        // find all the posts with the help of userid
        public async Task<List<PostModel>> FindPostsByUserIdAsync(int userId)
        {
            return await _context.Posts
                .Where(p => p.AuthorId == userId)
                .ToListAsync();
        }

       
        public async Task UpdateProfileAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
