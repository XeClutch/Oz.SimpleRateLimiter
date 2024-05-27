using Oz.RateLimiting;

namespace Oz.Examples;

/// <summary>
/// For demonstration purposes only.
/// </summary>
public class RateLimitedConsoleSpam {
    internal static async Task Main() {
        // create a new limiter @ 5 requests per 1 second
        var rateLimiter = new SimpleRateLimiter(5, new TimeSpan(0, 0, 0, 1));
        var cancellationToken = new CancellationToken();
        
        // try spamming the console
        for (int i = 1; i <= 10; i++) {
            // wait for any potential hold times 
            if (await rateLimiter.WaitForAvailabilityAsync(cancellationToken)) {
                // we're safe to continue
                Console.WriteLine($"Request #{i} - DateTime.Now: {DateTime.Now}");
            }
        }
    }
}