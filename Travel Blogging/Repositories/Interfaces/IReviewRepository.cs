using Travel_Blogging.Models;

namespace Travel_Blogging.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task DeleteComments(int userId);  // this is for deleting all the comments of specific user when user deletes

        Task<ReviewModel> AddReviewAsync(ReviewModel review);  

        Task DeleteReviewAsync(int postId, int reviewId);  // delete any specific comments if any user wants to deletes it's own comments

        Task DeletePostReviewAsync(int postId); // this is for deleting all the comments of any specific post if post is delete
    }
}
