using Travel_Blogging.DTOs.PostDto;
using Travel_Blogging.Models;

namespace Travel_Blogging.Services.Interfaces
{
    public interface IPostService
    {
        Task<PostModel> CreatePost(CreatePostDto dto, int userId, string actionType);
        Task<List<PostModel>>FindPosts();
        Task<PostModel>FindPostDetails(int postId);

        Task<List<PostModel>> FindLatestPosts();

        Task<List<PostModel>> FindUserPosts(int userId);

        Task DeleteUserPost(int postId, int userId);

        Task<CreatePostDto> EditPost(int postId); // this is for sending the saved data to the clint (httpget)

        Task<PostModel> UpdatePost(CreatePostDto dto);
    }
}
