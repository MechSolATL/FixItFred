// ===============================
// 📂 File: Data/Seeders/ImageSeeder.cs
// ===============================

using System;
using System.IO;
using MVP_Core.Data;
using MVP_Core.Data.Models;

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
                    var imageBytes = File.ReadAllBytes(imagePath);

                    var backgroundImage = new BackgroundImage
                    {
                        ImageData = imageBytes,
                        ContentType = "image/jpeg"
                    };

                    dbContext.BackgroundImages.Add(backgroundImage);
                    dbContext.SaveChanges();
                }
                else
                {
                    Console.WriteLine($"⚠️ Image file not found at: {imagePath}");
                }
            }
        }
    }
}
