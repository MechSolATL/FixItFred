using Data;
using Data.Models.PetMatrix;
using Microsoft.EntityFrameworkCore;

namespace Services.PetMatrix
{
    /// <summary>
    /// Service for managing the Chase-powered snack shop with pricing, flavors, and purchases.
    /// Implements the $1 = 1 Aura point economy system.
    /// </summary>
    public class SnackShopService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuraRankService _auraRankService;
        private readonly ILogger<SnackShopService> _logger;

        public SnackShopService(ApplicationDbContext context, AuraRankService auraRankService, ILogger<SnackShopService> logger)
        {
            _context = context;
            _auraRankService = auraRankService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all available snacks in the shop.
        /// </summary>
        public async Task<List<Snack>> GetAvailableSnacksAsync()
        {
            return await _context.Snacks
                .Where(s => s.IsActive)
                .OrderBy(s => s.Price)
                .ToListAsync();
        }

        /// <summary>
        /// Gets snacks compatible with a specific pet family.
        /// </summary>
        public async Task<List<Snack>> GetSnacksForPetFamilyAsync(string petFamily)
        {
            return await _context.Snacks
                .Where(s => s.IsActive && (s.TargetFamily == "all" || s.TargetFamily == petFamily))
                .OrderBy(s => s.Price)
                .ToListAsync();
        }

        /// <summary>
        /// Purchases a snack for a pet using the Chase-powered payment system.
        /// Automatically adds aura points based on $1 = 1 point system.
        /// </summary>
        public async Task<SnackPurchaseResult> PurchaseSnackAsync(Guid snackId, Guid petId, string userId, string? flavorType = null)
        {
            var snack = await _context.Snacks.FirstOrDefaultAsync(s => s.SnackId == snackId && s.IsActive);
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetId == petId && p.UserId == userId);

            if (snack == null)
            {
                return new SnackPurchaseResult
                {
                    Success = false,
                    Message = "Snack not found or unavailable"
                };
            }

            if (pet == null)
            {
                return new SnackPurchaseResult
                {
                    Success = false,
                    Message = "Pet not found"
                };
            }

            // Calculate total price including flavor
            var totalPrice = snack.Price;
            if (!string.IsNullOrEmpty(flavorType))
            {
                totalPrice += snack.FlavorPrice;
            }

            // Create purchase record
            var purchase = new SnackPurchase
            {
                SnackId = snackId,
                PetId = petId,
                UserId = userId,
                Quantity = 1,
                TotalPrice = totalPrice,
                FlavorUsed = flavorType ?? "",
                PurchaseTime = DateTime.UtcNow
            };

            // Add to aura points ($1 = 1 Aura point)
            await _auraRankService.AddAuraPurchaseAsync(userId, totalPrice);

            _context.SnackPurchases.Add(purchase);
            await _context.SaveChangesAsync();

            _logger.LogInformation("[Sprint135_PetMatrix] User {UserId} purchased {SnackName} for {Price:C} (Aura: +{AuraPoints})", 
                userId, snack.Name, totalPrice, totalPrice);

            return new SnackPurchaseResult
            {
                Success = true,
                Message = "Snack purchased successfully!",
                PurchaseId = purchase.PurchaseId,
                SnackName = snack.Name,
                TotalPrice = totalPrice,
                FlavorUsed = flavorType ?? "",
                AuraPointsEarned = totalPrice,
                RiskWarning = snack.GetRiskWarning()
            };
        }

        /// <summary>
        /// Seeds the database with default snacks if they don't exist.
        /// </summary>
        public async Task SeedDefaultSnacksAsync()
        {
            var existingSnacks = await _context.Snacks.CountAsync();
            if (existingSnacks > 0)
            {
                return; // Already seeded
            }

            var defaultSnacks = SnackCatalog.DefaultSnacks;
            
            foreach (var snack in defaultSnacks)
            {
                snack.SnackId = Guid.NewGuid(); // Generate new IDs
                snack.CreatedAt = DateTime.UtcNow;
            }

            _context.Snacks.AddRange(defaultSnacks);
            await _context.SaveChangesAsync();

            _logger.LogInformation("[Sprint135_PetMatrix] Seeded {Count} default snacks into the shop", defaultSnacks.Count);
        }

        /// <summary>
        /// Gets the available flavor options for snacks.
        /// </summary>
        public List<FlavorOption> GetAvailableFlavors()
        {
            return SnackCatalog.AvailableFlavors.Select(flavor => new FlavorOption
            {
                Name = flavor,
                Price = 0.25m,
                Effect = GetFlavorEffectDescription(flavor)
            }).ToList();
        }

        /// <summary>
        /// Gets purchase history for a user.
        /// </summary>
        public async Task<List<SnackPurchase>> GetUserPurchaseHistoryAsync(string userId, int limit = 20)
        {
            return await _context.SnackPurchases
                .Where(sp => sp.UserId == userId)
                .Include(sp => sp.Snack)
                .Include(sp => sp.Pet)
                .OrderByDescending(sp => sp.PurchaseTime)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Gets sales analytics for the snack shop.
        /// </summary>
        public async Task<SnackShopAnalytics> GetShopAnalyticsAsync()
        {
            var purchases = await _context.SnackPurchases.Include(sp => sp.Snack).ToListAsync();
            
            var totalRevenue = purchases.Sum(p => p.TotalPrice);
            var totalSales = purchases.Count;
            var averageOrderValue = totalSales > 0 ? totalRevenue / totalSales : 0;

            var topSellingSnacks = purchases
                .GroupBy(p => p.Snack?.Name ?? "Unknown")
                .Select(g => new SnackSalesData
                {
                    SnackName = g.Key,
                    UnitsSold = g.Count(),
                    Revenue = g.Sum(p => p.TotalPrice)
                })
                .OrderByDescending(s => s.UnitsSold)
                .Take(5)
                .ToList();

            var riskItemsSold = purchases.Count(p => p.Snack?.IsRisky == true);
            var flavoredItemsSold = purchases.Count(p => !string.IsNullOrEmpty(p.FlavorUsed));

            return new SnackShopAnalytics
            {
                TotalRevenue = totalRevenue,
                TotalSales = totalSales,
                AverageOrderValue = averageOrderValue,
                TopSellingSnacks = topSellingSnacks,
                RiskItemsSold = riskItemsSold,
                FlavoredItemsSold = flavoredItemsSold,
                TotalAuraPointsGenerated = totalRevenue // $1 = 1 Aura point
            };
        }

        /// <summary>
        /// Creates a custom snack (admin function).
        /// </summary>
        public async Task<Snack> CreateCustomSnackAsync(CreateSnackRequest request)
        {
            var snack = new Snack
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Brand = request.Brand,
                FlavorType = "",
                FlavorPrice = 0.25m,
                IsRisky = request.IsRisky,
                IsMislabeled = request.IsMislabeled,
                TrustBonus = request.TrustBonus,
                HappinessBonus = request.HappinessBonus,
                TargetFamily = request.TargetFamily,
                EffectDescription = request.EffectDescription,
                SpecialEffect = request.SpecialEffect,
                EffectDurationMinutes = request.EffectDurationMinutes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Snacks.Add(snack);
            await _context.SaveChangesAsync();

            _logger.LogInformation("[Sprint135_PetMatrix] Created custom snack: {SnackName} - {Price:C}", 
                snack.Name, snack.Price);

            return snack;
        }

        /// <summary>
        /// Gets the effect description for a flavor type.
        /// </summary>
        private string GetFlavorEffectDescription(string flavor)
        {
            if (flavor.Contains("Spicy")) return "May cause adorable hiccups 🫧";
            if (flavor.Contains("Fizzy")) return "Triggers spontaneous dancing 💃";
            if (flavor.Contains("Fermented")) return "Results in satisfied burping 😋";
            if (flavor.Contains("Aromatic")) return "Inspires melodious howling 🎵";
            if (flavor.Contains("Experimental")) return "Random unpredictable effects ❓";
            
            return "Special effect unknown";
        }
    }

    /// <summary>
    /// Result of a snack purchase operation.
    /// </summary>
    public class SnackPurchaseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public Guid PurchaseId { get; set; }
        public string SnackName { get; set; } = "";
        public decimal TotalPrice { get; set; }
        public string FlavorUsed { get; set; } = "";
        public decimal AuraPointsEarned { get; set; }
        public string RiskWarning { get; set; } = "";
    }

    /// <summary>
    /// Flavor option for snack customization.
    /// </summary>
    public class FlavorOption
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string Effect { get; set; } = "";
    }

    /// <summary>
    /// Analytics data for the snack shop.
    /// </summary>
    public class SnackShopAnalytics
    {
        public decimal TotalRevenue { get; set; }
        public int TotalSales { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<SnackSalesData> TopSellingSnacks { get; set; } = new();
        public int RiskItemsSold { get; set; }
        public int FlavoredItemsSold { get; set; }
        public decimal TotalAuraPointsGenerated { get; set; }
    }

    /// <summary>
    /// Sales data for individual snacks.
    /// </summary>
    public class SnackSalesData
    {
        public string SnackName { get; set; } = "";
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
    }

    /// <summary>
    /// Request model for creating custom snacks.
    /// </summary>
    public class CreateSnackRequest
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public string Brand { get; set; } = "GOOSENECK Brands Inc.";
        public bool IsRisky { get; set; }
        public bool IsMislabeled { get; set; }
        public float TrustBonus { get; set; }
        public float HappinessBonus { get; set; }
        public string TargetFamily { get; set; } = "all";
        public string EffectDescription { get; set; } = "";
        public string SpecialEffect { get; set; } = "";
        public int EffectDurationMinutes { get; set; } = 30;
    }
}