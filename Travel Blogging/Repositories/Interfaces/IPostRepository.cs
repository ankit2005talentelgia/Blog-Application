using Travel_Blogging.Models;
using Travel_Blogging.Models.Enums;

namespace Travel_Blogging.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<PostModel> CreatePostAsync(PostModel post);
        Task<List<PostModel>> FindPostsAsync();

        Task<PostModel> FindPostDetailsAsync(int postId);

        Task<List<PostModel>> FindLatestPostsAsync();

        Task<List<PostModel>> FindUserPostsAsync(int userId, PostStatus status); // for loggedin user's post

        Task DeleteUserPostAsync(int postId, int userId);  // deleting any specific post of loggedin users

        //Task DeletePostAsync(int postId, int authorId); // deleting all the post when user's account is deleted

        Task<PostModel> FindPostByIdAsync(int postId);

        Task UpdatePostAsync(); // saving the updated field in the database
    }
}
