using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace WellingtonWeatherRecommendationApi.Services
{
    public class RateLimiterService
    {
        private readonly ConcurrentDictionary<string, RequestInfo> _requestCounts = new ConcurrentDictionary<string, RequestInfo>();
        private readonly int _requestLimit = 5; // Max 5 requests
        private readonly TimeSpan _window = TimeSpan.FromMinutes(1); // 1-minute window
        private readonly TimeSpan _blockDuration = TimeSpan.FromMinutes(5); // 5-minute block

        private class RequestInfo
        {
            public int Count { get; set; }
            public DateTime LastRequestTime { get; set; }
            public DateTime? BlockUntil { get; set; }
        }

        public async Task<bool> IsRequestAllowedAsync(string clientIp)
        {
            var now = DateTime.UtcNow;
            var requestInfo = _requestCounts.GetOrAdd(clientIp, _ => new RequestInfo { LastRequestTime = now });

            lock (requestInfo) // Ensure thread safety for individual client
            {
                // Check if client is blocked
                if (requestInfo.BlockUntil.HasValue && now < requestInfo.BlockUntil.Value)
                {
                    return false;
                }

                // Reset count if window has expired
                if (now - requestInfo.LastRequestTime > _window)
                {
                    requestInfo.Count = 0;
                    requestInfo.BlockUntil = null;
                }

                // Increment request count
                requestInfo.Count++;
                requestInfo.LastRequestTime = now;

                // Block client if limit exceeded
                if (requestInfo.Count > _requestLimit)
                {
                    requestInfo.BlockUntil = now.Add(_blockDuration);
                    return false;
                }

                return true;
            }
        }

        public TimeSpan? GetRetryAfterTime(string clientIp)
        {
            if (_requestCounts.TryGetValue(clientIp, out var requestInfo) && requestInfo.BlockUntil.HasValue)
            {
                var remaining = requestInfo.BlockUntil.Value - DateTime.UtcNow;
                return remaining > TimeSpan.Zero ? remaining : null;
            }
            return null;
        }
    }
}