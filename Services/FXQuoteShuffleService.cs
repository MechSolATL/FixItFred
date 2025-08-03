using Microsoft.Extensions.Logging;

namespace MVP_Core.Services
{
    public interface IFXQuoteShuffleService
    {
        Task<string> GetRandomQuoteAsync();
        Task<string> GetQuoteByCategoryAsync(string category);
        Task<List<string>> GetAllQuotesAsync();
    }

    public class FXQuoteShuffleService : IFXQuoteShuffleService
    {
        private readonly ILogger<FXQuoteShuffleService> _logger;
        private readonly List<QuoteData> _quotes;

        public FXQuoteShuffleService(ILogger<FXQuoteShuffleService> logger)
        {
            _logger = logger;
            _quotes = InitializeQuotes();
        }

        public async Task<string> GetRandomQuoteAsync()
        {
            var random = new Random();
            var quote = _quotes[random.Next(_quotes.Count)];
            _logger.LogInformation("Retrieved random quote from {Artist}: {Quote}", quote.Artist, quote.Text);
            return await Task.FromResult($"{quote.Text} - {quote.Artist}");
        }

        public async Task<string> GetQuoteByCategoryAsync(string category)
        {
            var categoryQuotes = _quotes.Where(q => q.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!categoryQuotes.Any())
            {
                return await GetRandomQuoteAsync();
            }

            var random = new Random();
            var quote = categoryQuotes[random.Next(categoryQuotes.Count)];
            return await Task.FromResult($"{quote.Text} - {quote.Artist}");
        }

        public async Task<List<string>> GetAllQuotesAsync()
        {
            return await Task.FromResult(_quotes.Select(q => $"{q.Text} - {q.Artist}").ToList());
        }

        private List<QuoteData> InitializeQuotes()
        {
            return new List<QuoteData>
            {
                // Self
                new QuoteData { Text = "I'm self-made, selfish with my image, well made", Artist = "Nas", Category = "Self" },
                new QuoteData { Text = "I know that I can make it matter, what I've got", Artist = "Nas", Category = "Self" },
                new QuoteData { Text = "I am hip-hop", Artist = "KRS-One", Category = "Self" },
                new QuoteData { Text = "You must learn", Artist = "KRS-One", Category = "Self" },
                new QuoteData { Text = "Knowledge reigns supreme over nearly everyone", Artist = "KRS-One", Category = "Self" },
                new QuoteData { Text = "I'm cold getting busy, the rhyme is so tricky", Artist = "Just-Ice", Category = "Self" },
                new QuoteData { Text = "Straight outta Compton, crazy motherf***er named Ice Cube", Artist = "Ice Cube", Category = "Self" },
                new QuoteData { Text = "I started from the bottom now I'm here", Artist = "Rakim", Category = "Self" },
                new QuoteData { Text = "Thinking of a master plan", Artist = "Rakim", Category = "Self" },
                new QuoteData { Text = "It's been a long time, I shouldn't have left you", Artist = "Rakim", Category = "Self" },
                
                // Action
                new QuoteData { Text = "Life's a b**** and then you die", Artist = "Nas", Category = "Action" },
                new QuoteData { Text = "The world is yours", Artist = "Nas", Category = "Action" },
                new QuoteData { Text = "Stop the violence", Artist = "KRS-One", Category = "Action" },
                new QuoteData { Text = "By all means necessary", Artist = "KRS-One", Category = "Action" },
                new QuoteData { Text = "Put the needle to the groove, I get smooth", Artist = "Just-Ice", Category = "Action" },
                new QuoteData { Text = "F*** the police", Artist = "Ice Cube", Category = "Action" },
                new QuoteData { Text = "Check the rhyme", Artist = "Ice Cube", Category = "Action" },
                new QuoteData { Text = "Pump up the volume", Artist = "Rakim", Category = "Action" },
                new QuoteData { Text = "Follow the leader", Artist = "Rakim", Category = "Action" },
                new QuoteData { Text = "Bring the noise", Artist = "Red Alert", Category = "Action" },
                
                // Consequence  
                new QuoteData { Text = "What goes around, comes around", Artist = "Nas", Category = "Consequence" },
                new QuoteData { Text = "You reap what you sow", Artist = "KRS-One", Category = "Consequence" },
                new QuoteData { Text = "Every action has a reaction", Artist = "Just-Ice", Category = "Consequence" },
                new QuoteData { Text = "You can't escape the consequences", Artist = "Ice Cube", Category = "Consequence" },
                new QuoteData { Text = "The chickens come home to roost", Artist = "Rakim", Category = "Consequence" },
                new QuoteData { Text = "What you put out comes back", Artist = "Red Alert", Category = "Consequence" },
                new QuoteData { Text = "The universe balances itself", Artist = "Mr. Magic", Category = "Consequence" },
                
                // Redemption
                new QuoteData { Text = "I can", Artist = "Nas", Category = "Redemption" },
                new QuoteData { Text = "The message", Artist = "KRS-One", Category = "Redemption" },
                new QuoteData { Text = "Back on the scene", Artist = "Just-Ice", Category = "Redemption" },
                new QuoteData { Text = "Today was a good day", Artist = "Ice Cube", Category = "Redemption" },
                new QuoteData { Text = "When I B on the mic", Artist = "Rakim", Category = "Redemption" },
                new QuoteData { Text = "Yo! MTV Raps", Artist = "Red Alert", Category = "Redemption" },
                new QuoteData { Text = "Rap Attack", Artist = "Mr. Magic", Category = "Redemption" },
                
                // Additional quotes for variety
                new QuoteData { Text = "Represent", Artist = "Nas", Category = "Self" },
                new QuoteData { Text = "Hip hop lives", Artist = "KRS-One", Category = "Action" },
                new QuoteData { Text = "Back to the old school", Artist = "Just-Ice", Category = "Redemption" },
                new QuoteData { Text = "AmeriKKKa's Most Wanted", Artist = "Ice Cube", Category = "Consequence" },
                new QuoteData { Text = "Microphone fiend", Artist = "Rakim", Category = "Self" },
                new QuoteData { Text = "Let the rhythm hit 'em", Artist = "Red Alert", Category = "Action" },
                new QuoteData { Text = "The magic's in the mix", Artist = "Mr. Magic", Category = "Self" }
            };
        }

        private class QuoteData
        {
            public string Text { get; set; } = "";
            public string Artist { get; set; } = "";
            public string Category { get; set; } = "";
        }
    }
}