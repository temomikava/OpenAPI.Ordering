namespace OpenAPI.Ordering.RateLimiting
{
    public class RateLimitOptions
    {
        public int Limit { get; set; }
        public TimeSpan Period { get; set; }
    }
}
