using AuthorVerseServer.Interfaces.ServiceInterfaces;

namespace AuthorVerseServer.Services
{
    public class RedisCacheService : IRedisCache
    {
        public Task<string> CheckValueByKeyAsync(string key)
        {
            throw new NotImplementedException();
        }

        public T GetValueByKey<T>(string value)
        {
            throw new NotImplementedException();
        }

        public Task SetValueByKey<T>(string key, T value, TimeSpan time)
        {
            throw new NotImplementedException();
        }
    }
}
