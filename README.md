# Oz.SimpleRateLimiter <a href="https://www.nuget.org/packages/Oz.SimpleRateLimiter"><img alt="NuGet" src="https://img.shields.io/nuget/v/Oz.SimpleRateLimiter.svg?label=Oz.SimpleRateLimiter"/> <img alt="NuGet Downloads" src="https://img.shields.io/nuget/dt/Oz.SimpleRateLimiter.svg?color=blue"/></a>
SimpleRateLimiter is.. well.. just that. A short, sweet and to the point rate limiter (with interrupt-ability!). Implement it however you like. Use it in a loop or implement it into a delegate handler. Create a derivative of your favorite class and use it in otherwise pointless overrides that just point back to `base`. Whatever you want.

## Example
Here's a more than pointless ConsoleApp demonstrating exactly how this works.
```csharp
using Oz.RateLimiting;

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
```

## Usage
#### Import the namespace
```csharp
using Oz.RateLimiting;
```
#### Establish a rate limit
Let's get setup for 200 requests per hour.
```csharp
var rateLimiter = new SimpleRateLimiter(200, new TimeSpan(0, 1));
```
Or if maybe this is easier to read. Albeit, definitely less efficient.
```csharp
var now = DateTime.Now;
var oneMinuteFromNow = now.AddHours(1);
var oneMinuteWindow = oneMinuteFromNow - now;

var rateLimiter = new SimpleRateLimiter(200, oneMinuteWindow);
```
#### Abiding by the rate limit
Let's make some API calls to [jsonplaceholder](https://jsonplaceholder.typicode.com/) (a free, fake API for testing).
```csharp
var cancellationToken = new CancellationToken();
var httpClient = new HttpClient();
for (int i = 1; i <= 5000; i++) {
    var request = new HttpRequestMessage(HttpMethod.Get,
        $"https://jsonplaceholder.typicode.com/photos/{i}");
    if (rateLimiter.WaitForAvailabilityAsync(cancellationToken)) {
        var response = await client.SendAsync(request);
    }
}
```
