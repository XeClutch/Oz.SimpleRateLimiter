namespace Oz.RateLimiting;

public class SimpleRateLimiter {
    // Fields
    private int _requestCount;
    private DateTime _timeWindowStartUtc;
    
    // Properties
    public int MaxRequests { get; set; }
    public TimeSpan TimeWindow { get; set; }

    /// <summary>
    /// Instantiates a simple rate limiter.
    /// </summary>
    /// <param name="maxRequests">The maximum number of requests per <paramref name="timeWindow"/>.</param>
    /// <param name="timeWindow">A <see cref="TimeSpan"/> representing the amount of time <paramref name="maxRequests"/> can be made.</param>
    public SimpleRateLimiter(int maxRequests, TimeSpan timeWindow) {
        _timeWindowStartUtc = DateTime.UtcNow;
        
        MaxRequests = maxRequests;
        TimeWindow = timeWindow;
    }

    /// <summary>
    /// Checks if the specified rate limit has been reached and if so, waits until we're safe to continue.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token allowing for the interruption of this method.</param>
    /// <returns>Returns true when safe to continue. Returns false if cancelled.</returns>
    public async Task<bool> WaitForAvailabilityAsync(CancellationToken cancellationToken) {
        while (true) {
            lock (this) {
                var utcNow = DateTime.UtcNow;

                if ((utcNow - _timeWindowStartUtc) >= TimeWindow) {
                    _timeWindowStartUtc = utcNow;
                    _requestCount = 0;
                }

                if (_requestCount < MaxRequests) {
                    _requestCount++;
                    return true;
                }
            }

            await Task.Delay(100, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return false;
        }
    }
}