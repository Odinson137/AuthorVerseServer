namespace AuthorVerseServer.Interfaces.ServiceInterfaces
{
    public interface IRedisCache
    {
        Task<string> CheckValueByKeyAsync(string key);
        T GetValueByKey<T>(string value);
        Task SetValueByKey<T>(string key, T value, TimeSpan time);
    }
}
