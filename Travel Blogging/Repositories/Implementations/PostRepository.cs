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
            var post = await _context.Posts.FindAsync(postId);
            return post;
        }
    }
}
