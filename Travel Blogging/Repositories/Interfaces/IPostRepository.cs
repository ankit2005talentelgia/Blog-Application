using Travel_Blogging.Models;

namespace Travel_Blogging.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<PostModel> CreatePostAsync(PostModel post);
        Task<List<PostModel>> FindPostsAsync();

        Task<PostModel> FindPostDetailsAsync(int postId);

        Task<List<PostModel>> FindLatestPostsAsync();

        Task<List<PostModel>> FindUserPostsAsync(int userId);

        Task DeleteUserPostAsync(int postId, int userId);

        Task<PostModel> FindPostByIdAsync(int postId);

        Task UpdatePostAsync();
    }
}
