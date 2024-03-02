namespace Valuator.Redis
{
    public interface IRedisStorage
    {
        void Save(string key, string value);
        string Get(string key);
        List<string> GetKeys();
    }
}