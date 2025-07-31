// ? File: E:\source\MVP-Core\Services\ProfileReviewService.cs

using Data;
using Data.Models;

namespace Services
{
    public class ProfileReviewService
    {
        private readonly ApplicationDbContext _dbContext;

        public ProfileReviewService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void SaveProfileReview(ProfileReview profileReviewData)
        {
            if (profileReviewData == null)
            {
                throw new ArgumentNullException(nameof(profileReviewData));
            }

            profileReviewData.SubmittedAt = DateTime.UtcNow;

            _ = _dbContext.ProfileReviews.Add(profileReviewData);
            _ = _dbContext.SaveChanges();
        }

        public List<ProfileReview> GetAll()
        {
            return _dbContext.ProfileReviews
                .OrderByDescending(static p => p.CreatedAt)
                .ToList();
        }

        public ProfileReview? GetById(int id)
        {
            return _dbContext.ProfileReviews
                .FirstOrDefault(p => p.Id == id);
        }
    }
}
