using Travel_Blogging.DTOs.PostDto;
using Travel_Blogging.Models;
using Travel_Blogging.Repositories.Interfaces;
using Travel_Blogging.Services.Interfaces;

namespace Travel_Blogging.Services.Implementations
{
    public class ReviewService:IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        // this function is for creating the review of the post
        public async Task<ReviewModel>AddReview(CreateReviewDto dto, int userId)
        {
            Console.WriteLine("hello from service");
            var review = new ReviewModel
            {
                Content = dto.Content,
                PostId = dto.PostId,
                UserId = userId
            };

            await _reviewRepository.AddReviewAsync(review);

            return review;
        }

        // this function is for deleting any review when user deletes the review button
        public async Task DeleteReview(int postId, int reviewId)
        {
            await _reviewRepository.DeleteReviewAsync(postId, reviewId);
        }
    }
}
