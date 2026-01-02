using ECommerce.Business.DTOs.Reviews.Requests;
using ECommerce.Business.DTOs.Reviews.Responses;

namespace ECommerce.Business.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewSummaryDto>> GetAllReviewsAsync(string? ratingFilter);
        Task<IEnumerable<ReviewProductSummaryDto>> GetAllProductReviewsAsync(int productId);
        Task MarkHelpfulAsync(int reviewId);
        Task<ReviewProductSummaryDto> AddReviewAsync(int productId, AddReviewRequest addReviewRequest);
        Task<ReviewSummaryDto> UpdateReviewAsync(int reviewId, UpdateReviewRequest updateReviewRequest);
        Task DeleteReviewAsync(int reviewId);
    }
}
