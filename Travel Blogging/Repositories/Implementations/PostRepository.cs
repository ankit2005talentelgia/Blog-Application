using Microsoft.EntityFrameworkCore;
using Travel_Blogging.Data;
using Travel_Blogging.Models;
using Travel_Blogging.Repositories.Interfaces;

namespace Travel_Blogging.Repositories.Implementations
{
    public class PostRepository:IPostRepository
    {
        private readonly ApplicationDbContext _context;
        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // this function is for creating the new post
        public async Task<PostModel>CreatePostAsync(PostModel post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        // this function is for find the all post from the database
        public async Task<List<PostModel>> FindPostsAsync()
        {
            var posts = await _context.Posts.ToListAsync();
            return posts;
        }

        // this function is for finding the post details of any specific post
        public async Task<PostModel> FindPostDetailsAsync(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.Author) 
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == postId);

            return post;
        }

        // this function is for finding the latest posts from the db
        public async Task<List<PostModel>> FindLatestPostsAsync()
        {
            var tenDaysAgo = DateTime.Now.AddDays(-10);

            return await _context.Posts
                .Where(p => p.CreatedAt >= tenDaysAgo)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // this function is for finding all the post of currently loggedin user
        public async Task<List<PostModel>> FindUserPostsAsync(int userId)
        {
            return await _context.Posts
                .Where(p => p.AuthorId == userId)
                .ToListAsync();
        }

        // this function is for deleting any specific post of loggedin users
        public async Task DeleteUserPostAsync(int postId, int userId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == postId && p.AuthorId == userId);

            if (post != null)
            {
                post.IsDeleted = true;
                post.UpdatedAt = DateTime.Now;
                post.UpdatedBy = userId;

                await _context.SaveChangesAsync();
            }
        }

        // this function is for finding the post with postid
        public async Task<PostModel> FindPostByIdAsync(int postId)
        {
            return await _context.Posts.FindAsync(postId);
        }

        // this function is for saving the updated field in the database
        public async Task UpdatePostAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
