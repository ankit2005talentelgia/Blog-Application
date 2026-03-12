using Travel_Blogging.DTOs.PostDto;
using Travel_Blogging.Models;

namespace Travel_Blogging.Services.Interfaces
{
    public interface IPostService
    {
        Task<PostModel> CreatePost(CreatePostDto dto, int userId);
        Task<List<PostModel>>FindPosts();
        Task<PostModel>FindPostDetails(int postId);

    }
}
