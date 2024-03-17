using StackExchange.Redis;

namespace Valuator.Redis
{
    public class RedisStorage : IRedisStorage
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IConfiguration _configuration;
        private IDatabase _db;
        private IServer _server;

        public RedisStorage(IConfiguration configuration)
        {
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString("redis")!;

            _connection = ConnectionMultiplexer.Connect(connectionString);
            _db = _connection.GetDatabase();
            _server = _connection.GetServer(connectionString);
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
            var keys = _server.Keys();

            return keys.Select(item => item.ToString()).ToList();
        }
    }
}
