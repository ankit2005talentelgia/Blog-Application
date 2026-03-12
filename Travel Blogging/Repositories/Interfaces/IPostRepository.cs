using Travel_Blogging.Models;

namespace Travel_Blogging.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<PostModel> CreatePostAsync(PostModel post);
        Task<List<PostModel>> FindPostsAsync();

        Task<PostModel> FindPostDetailsAsync(int postId);
    }
}
