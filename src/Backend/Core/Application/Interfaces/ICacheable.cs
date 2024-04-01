namespace Application.Interfaces
{
    public interface ICacheable
    {
        bool BypassCache { get; }
        string CacheKey { get; }
    }
}
