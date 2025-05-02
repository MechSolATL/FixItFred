using MVP_Core.Data;
using MVP_Core.Data.Models;

namespace MVP_Core.Services
{
    public class ProfileReviewService
    {
        private readonly ApplicationDbContext _dbContext;

        public ProfileReviewService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SaveProfileReviewAsync(ProfileReviewModel profileReviewData)
        {
            if (profileReviewData == null)
                throw new ArgumentNullException(nameof(profileReviewData));

            var profileEntity = new ProfileReview
            {
                Username = profileReviewData.Username,
                Email = profileReviewData.Email,
                PhoneNumber = profileReviewData.PhoneNumber,
                VerificationCode = profileReviewData.VerificationCode,
                ReviewNotes = profileReviewData.ReviewNotes,
                CreatedAt = DateTime.UtcNow,
                SubmittedAt = DateTime.UtcNow
            };

            _dbContext.ProfileReviews.Add(profileEntity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
