namespace Valuator.Redis
{
    public interface IRedisStorage
    {
        void Save(string key, string value, string obj);
        string Get(string key, string obj);
        List<string> GetKeys();
        public void SaveIdToRegion(string id, string country);
    }
}