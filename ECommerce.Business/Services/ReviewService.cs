using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Reviews.Requests;
using ECommerce.Business.DTOs.Reviews.Responses;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommerce.Business.Services
{
    public class ReviewService(AppDbContext context,
        IMapper mapper,
        ILogger<BrandService> logger,
        IHttpContextAccessor httpContext) : IReviewService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<BrandService> _logger = logger;
        private readonly IHttpContextAccessor _httpContext = httpContext;

        public async Task<IEnumerable<ReviewSummaryDto>> GetAllReviewsAsync(string? ratingFilter)
        {
            var currentUserId = GetCurrentUserId();

            var query = _context.Reviews
                .Where(r => r.UserId == currentUserId)
                .OrderByDescending(r => r.Updated)
                .ThenByDescending(r => r.Created)
                .AsQueryable();

            //Filter
            if (ratingFilter is not null)
            {
                query = ratingFilter switch
                {
                    "5" => query.Where(r => r.Rating == 5),
                    "4" => query.Where(r => r.Rating == 4),
                    "3&below" => query.Where(r => r.Rating <= 3),
                    "all" => query,
                    _ => query
                };
            }
            var items = await query.ProjectTo<ReviewSummaryDto>(_mapper.ConfigurationProvider).ToListAsync();
            return items;

        }

        public async Task<IEnumerable<ReviewProductSummaryDto>> GetAllProductReviewsAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.HelpfulCount)
                .ThenByDescending(r => r.Created)
                .ProjectTo<ReviewProductSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task MarkHelpfulAsync(int reviewId)
        {
            var reviewToMarkHelpful = await _context.Reviews.FindAsync(reviewId)
                ?? throw new NotFoundException("Review does not exist");

            var currentUserId = GetCurrentUserId();

            if (reviewToMarkHelpful.UserId == currentUserId)
                throw new BadRequestException("You cannot mark your own review as helpful.");

            reviewToMarkHelpful.HelpfulCount++;

            await _context.SaveChangesAsync();
        }

        public async Task<ReviewProductSummaryDto> AddReviewAsync(int productId, AddReviewRequest addReviewRequest)
        {
            //Validation ( User cannot post a review on a product he has not bought and delivered) , (user cannot post more than once on a product )
            var currentUserId = GetCurrentUserId();

            var canPost = await _context.OrderItems
                .AnyAsync(oi => oi.Order.UserId == currentUserId
                && oi.OrderedProduct.Id == productId
                && oi.Order.Status == Core.Enums.OrderStatus.Delivered);

            if (!canPost)
                throw new BadRequestException("Posting reviews is restricted only for users who bought the product.");

            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.ProductId == productId && r.UserId == currentUserId);

            if (alreadyReviewed)
                throw new BadRequestException("Cannot review a prodcut twice.");

            var productToReview = await _context.Products.FindAsync(productId)
                ?? throw new NotFoundException("Product does not exist.");

            var currentUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId)
                ?? throw new NotFoundException("User does not exist.");

            var reviewToAdd = new Review
            {
                Rating = addReviewRequest.Rating,
                Comment = addReviewRequest.Comment,
                HelpfulCount = 0,
                Created = DateTime.UtcNow,
                UserId = currentUserId,
                ProductId = productId
            };

            _context.Reviews.Add(reviewToAdd);
            await _context.SaveChangesAsync();

            await UpdateProductStatsAsync(productId);

            reviewToAdd.User = currentUser;

            return _mapper.Map<ReviewProductSummaryDto>(reviewToAdd);


        }

        public async Task<ReviewSummaryDto> UpdateReviewAsync(int reviewId, UpdateReviewRequest updateReviewRequest)
        {
            var currentUserId = GetCurrentUserId();
            var reviewToUpdate = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == currentUserId && r.Id == reviewId)
                ?? throw new NotFoundException("Review does not exist");

            var productToReview = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == reviewToUpdate.ProductId)
                ?? throw new NotFoundException("Product does not exist");

            reviewToUpdate.Rating = updateReviewRequest.Rating;
            reviewToUpdate.Comment = updateReviewRequest.Comment;
            reviewToUpdate.Updated = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await UpdateProductStatsAsync(reviewToUpdate.ProductId);
            reviewToUpdate.Product = productToReview;
            return _mapper.Map<ReviewSummaryDto>(reviewToUpdate);
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var currentUserId = GetCurrentUserId();
            var reviewToDelete = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == currentUserId && r.Id == reviewId)
                ?? throw new NotFoundException("Review does not exist.");

            var productToReview = await _context.Products.FindAsync(reviewToDelete.ProductId)
                ?? throw new NotFoundException("Product does not exist.");

            _context.Reviews.Remove(reviewToDelete);
            await _context.SaveChangesAsync();

            await UpdateProductStatsAsync(reviewToDelete.ProductId);


        }

        //Helper Methods
        private string GetCurrentUserId()
        {
            var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            return userId;
        }

        private async Task UpdateProductStatsAsync(int productId)
        {
            var stats = await _context.Reviews
                .AsNoTracking()
                .Where(r => r.ProductId == productId)
                .GroupBy(r => r.ProductId)
                .Select(g => new
                {
                    Average = Convert.ToDecimal(g.Average(r => r.Rating)),
                    Count = g.Count()
                })
                .FirstOrDefaultAsync();

            var product = await _context.Products.FindAsync(productId);

            if (product != null)
            {
                product.AverageRating = stats?.Average ?? 0;
                product.ReviewsCount = stats?.Count ?? 0;

                await _context.SaveChangesAsync();
            }
        }
    }
}
