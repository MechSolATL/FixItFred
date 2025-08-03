using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace Pages.Services
{
    public class WheelOfFouModel : PageModel
    {
        public List<WheelPrize> AvailablePrizes { get; set; } = new();
        public List<string> RecentWinners { get; set; } = new();
        public int UserTokens { get; set; } = 0;
        public string? LastSpinResult { get; set; }

        public async Task OnGetAsync()
        {
            await LoadWheelData();
        }

        public async Task<IActionResult> OnPostSpinWheelAsync()
        {
            if (UserTokens < 10)
            {
                TempData["ErrorMessage"] = "Insufficient tokens! You need at least 10 tokens to spin.";
                await LoadWheelData();
                return Page();
            }

            // Simulate wheel spin
            var random = new Random();
            var prizesWithWeights = AvailablePrizes.SelectMany(p => 
                Enumerable.Repeat(p, p.Weight)).ToList();
            
            var wonPrize = prizesWithWeights[random.Next(prizesWithWeights.Count)];
            
            UserTokens -= 10; // Cost to spin
            LastSpinResult = wonPrize.Name;
            
            // Add to recent winners (simulated)
            RecentWinners.Insert(0, $"Anonymous won {wonPrize.Name}");
            if (RecentWinners.Count > 5) RecentWinners.RemoveAt(5);

            TempData["SuccessMessage"] = $"🎉 Congratulations! You won: {wonPrize.Name}!";
            TempData["PrizeDescription"] = wonPrize.Description;
            
            await LoadWheelData();
            return Page();
        }

        private async Task LoadWheelData()
        {
            // Simulate user tokens (in real app, this would come from user account)
            UserTokens = 45;

            AvailablePrizes = new List<WheelPrize>
            {
                // Common prizes (higher weight = more likely)
                new WheelPrize 
                { 
                    Name = "FX Glow Effect", 
                    Description = "Add a subtle glow animation to your profile",
                    Type = "Visual Effect",
                    Rarity = "Common",
                    Weight = 30,
                    Value = "10 tokens"
                },
                new WheelPrize 
                { 
                    Name = "Quote Badge", 
                    Description = "Display a motivational quote on your dashboard",
                    Type = "Badge",
                    Rarity = "Common",
                    Weight = 25,
                    Value = "15 tokens"
                },
                new WheelPrize 
                { 
                    Name = "Pixel Trail", 
                    Description = "Leave pixel dust when navigating pages",
                    Type = "Visual Effect",
                    Rarity = "Common",
                    Weight = 20,
                    Value = "20 tokens"
                },
                
                // Uncommon prizes
                new WheelPrize 
                { 
                    Name = "Heartbeat Theme", 
                    Description = "Pulse animation for all your content",
                    Type = "Theme",
                    Rarity = "Uncommon",
                    Weight = 15,
                    Value = "50 tokens"
                },
                new WheelPrize 
                { 
                    Name = "Fire Ignition", 
                    Description = "Spark animation when you complete tasks",
                    Type = "Animation",
                    Rarity = "Uncommon",
                    Weight = 12,
                    Value = "75 tokens"
                },
                
                // Rare prizes
                new WheelPrize 
                { 
                    Name = "Legend Status", 
                    Description = "Special 'Built by Legends' badge for your profile",
                    Type = "Status",
                    Rarity = "Rare",
                    Weight = 8,
                    Value = "150 tokens"
                },
                new WheelPrize 
                { 
                    Name = "Custom FX Pack", 
                    Description = "Exclusive access to premium visual effects",
                    Type = "FX Pack",
                    Rarity = "Rare",
                    Weight = 5,
                    Value = "200 tokens"
                },
                
                // Epic prizes (very rare)
                new WheelPrize 
                { 
                    Name = "Sparks88 Crown", 
                    Description = "Ultra-rare crown indicating Sparks88 evolution member",
                    Type = "Crown",
                    Rarity = "Epic",
                    Weight = 3,
                    Value = "500 tokens"
                },
                new WheelPrize 
                { 
                    Name = "Nova Command Access", 
                    Description = "Special access to Nova's experimental features",
                    Type = "Access",
                    Rarity = "Epic",
                    Weight = 2,
                    Value = "1000 tokens"
                },
                
                // Legendary prize (extremely rare)
                new WheelPrize 
                { 
                    Name = "Jack-in-the-Box FX", 
                    Description = "The ultimate surprise effect - appears randomly across the platform",
                    Type = "Ultimate FX",
                    Rarity = "Legendary",
                    Weight = 1,
                    Value = "PRICELESS"
                }
            };

            RecentWinners = new List<string>
            {
                "TechMaster won FX Glow Effect",
                "ServicePro won Heartbeat Theme", 
                "FieldExpert won Quote Badge",
                "AdminUser won Pixel Trail",
                "LegendaryTech won Fire Ignition"
            };

            await Task.CompletedTask;
        }
    }

    public class WheelPrize
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";
        public string Rarity { get; set; } = "";
        public int Weight { get; set; } // Higher weight = more likely to win
        public string Value { get; set; } = "";

        public string GetRarityClass()
        {
            return Rarity switch
            {
                "Common" => "text-secondary",
                "Uncommon" => "text-success",
                "Rare" => "text-primary",
                "Epic" => "text-warning",
                "Legendary" => "text-danger",
                _ => "text-muted"
            };
        }

        public string GetRarityBadge()
        {
            return Rarity switch
            {
                "Common" => "bg-secondary",
                "Uncommon" => "bg-success",
                "Rare" => "bg-primary",
                "Epic" => "bg-warning",
                "Legendary" => "bg-danger",
                _ => "bg-muted"
            };
        }
    }
}