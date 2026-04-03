using Travel_Blogging.DTOs.PostDto;
using Travel_Blogging.Models;

namespace Travel_Blogging.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewModel> AddReview(CreateReviewDto dto, int userId);

        Task DeleteReview(int postId, int reviewId, int userId);
    }
}
