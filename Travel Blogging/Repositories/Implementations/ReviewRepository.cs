using Microsoft.EntityFrameworkCore;
using Travel_Blogging.Data;
using Travel_Blogging.Models;
using Travel_Blogging.Repositories.Interfaces;

namespace Travel_Blogging.Repositories.Implementations
{
    public class ReviewRepository:IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // this function is for deleting all the comments from the database when user deletes
        public async Task DeleteComments(int userId)
        {
            var reviews = await _context.Comments
            .Where(c => c.UserId == userId)
            .ToListAsync();

            foreach (var review in reviews)
            {
                review.IsDeleted = true;
                review.IsActive = false;
                review.UpdatedAt = DateTime.Now;
                review.UpdatedBy = userId;
            }

            await _context.SaveChangesAsync();
        }

        // this function is for creating the review of the post
        public async Task<ReviewModel> AddReviewAsync(ReviewModel review)
        {
            await _context.Comments.AddAsync(review);

            await _context.SaveChangesAsync();

            return review;
        }

        // this function is for deleting the review when user clicks the delete button
        public async Task DeleteReviewAsync(int postId, int reviewId, int userId)
        {
            var review = await _context.Comments
            .FirstOrDefaultAsync(c => c.PostId == postId && c.Id == reviewId);

            if (review != null)
            {
                review.IsDeleted = true;
                review.IsActive = false;
                review.UpdatedAt = DateTime.Now;
                review.UpdatedBy = userId;

                await _context.SaveChangesAsync();
            }
        }

        // this function is for deletes all the review of any specific post if post is deleted
        public async Task DeletePostReviewAsync(int postId)
        {
            var reviews = await _context.Comments
                .Where(r => r.PostId == postId)
                .ToListAsync();

            foreach (var review in reviews)
            {
                review.IsDeleted = true;
                review.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
    }
}
