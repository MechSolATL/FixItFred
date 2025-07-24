// ===============================
// ?? File: Data/Seeders/ImageSeeder.cs
// ===============================

namespace MVP_Core.Data.Seeders
{
    public static class ImageSeeder
    {
        public static void SeedBackgroundImage(ApplicationDbContext dbContext, string imagePath)
        {
            if (!dbContext.BackgroundImages.Any())
            {
                if (File.Exists(imagePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(imagePath);

                    BackgroundImage backgroundImage = new()
                    {
                        ImageData = imageBytes,
                        ContentType = "image/jpeg"
                    };

                    _ = dbContext.BackgroundImages.Add(backgroundImage);
                    _ = dbContext.SaveChanges();
                }
                else
                {
                    Console.WriteLine($"?? Image file not found at: {imagePath}");
                }
            }
        }
    }
}
