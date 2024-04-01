namespace Infrastructure.Settings
{
    public class CacheSettings
    {
        public bool PreferRedis { get; set; }
        public string RedisURL { get; set; }
        public int RedisPort { get; set; }
        public int Database { get; set; }
    }
}
