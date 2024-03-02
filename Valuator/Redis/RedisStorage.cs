using StackExchange.Redis;

namespace Valuator.Redis
{
    public class RedisStorage : IRedisStorage
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IConfiguration _configuration;
        private IDatabase _db;

        public RedisStorage(IConfiguration configuration)
        {
            _configuration = configuration;

            var host = _configuration["RedisValues:HOST_NAME"];
            _connection = ConnectionMultiplexer.Connect(host);
            _db = _connection.GetDatabase();
        }

        public void Save(string key, string value)
        {
            _db.StringSet(key, value);
        }

        public string Get(string key)
        {
            return _db.StringGet(key);
        }

        public List<string> GetKeys()
        {
            var host = _configuration["RedisValues:HOST_NAME"];
            var port = Convert.ToInt32(_configuration["RedisValues:HOST_PORT"]);
            var keys = _connection.GetServer(host, port).Keys();

            return keys.Select(item => item.ToString()).ToList();
        }
    }
}
