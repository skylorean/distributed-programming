using StackExchange.Redis;
using System.Text.Json;

namespace Valuator.Redis
{
    public class RedisStorage : IRedisStorage
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IConfiguration _configuration;
        private IDatabase _db;

        public struct LoggerData
        {
            public string eventName { get; set; }
            public string contextId { get; set; }
            public string eventData { get; set; }

            public LoggerData(string nameOfEvent, string idOfContext, string dataOfEvent)
            {
                eventName = nameOfEvent;
                contextId = idOfContext;
                eventData = dataOfEvent;
            }
        }
        public static string DB_RUS = Environment.GetEnvironmentVariable("DB_RUS");
        public static string DB_EU = Environment.GetEnvironmentVariable("DB_EU");
        public static string DB_OTHER = Environment.GetEnvironmentVariable("DB_OTHER");

        public static Dictionary<string, string> DICT_OF_HOSTS_TO_REGIONS = new()
        {
            [DB_RUS] = "RUS",
            [DB_EU] = "EU",
            [DB_OTHER] = "OTHER"
        };

        public static Dictionary<string, string> DICT_OF_COUNTRIES_TO_REGIONS = new()
        {
            ["Russia"] = DB_RUS,
            ["France"] = DB_EU,
            ["Germany"] = DB_EU,
            ["USA"] = DB_OTHER,
            ["India"] = DB_OTHER
        };

        public RedisStorage(IConfiguration configuration)
        {
            _configuration = configuration;
            var host = _configuration["RedisValues:HOST_NAME"];

            _connection = ConnectionMultiplexer.Connect(host);
            _db = _connection.GetDatabase();
        }

        public void Save(string key, string value, string obj)
        {
            string nameOfSecondaryDB = _db.StringGet(key);
            ConnectionMultiplexer secondaryDB = ConnectionMultiplexer.Connect(nameOfSecondaryDB);

            IDatabase secondaryConn = secondaryDB.GetDatabase();
            secondaryConn.StringSet(obj + key, value);

            secondaryDB.Dispose();
            secondaryDB.Close();

            _db.StringSet(key, value);
        }

        public string Get(string key, string obj)
        {
            string nameOfSecondaryDB = _db.StringGet(key);

            ConnectionMultiplexer secondaryDB = ConnectionMultiplexer.Connect(nameOfSecondaryDB);

            IDatabase secondaryConn = secondaryDB.GetDatabase();
            string text = secondaryConn.StringGet(obj + key);

            LoggerData loggerData = new("LOOKUP", key, DICT_OF_HOSTS_TO_REGIONS[nameOfSecondaryDB]);
            string dataToSend = JsonSerializer.Serialize(loggerData);

            return text;
        }

        public List<string> GetKeys()
        {
            var host = _configuration["RedisValues:HOST_NAME"];
            var port = Convert.ToInt32(_configuration["RedisValues:HOST_PORT"]);
            var keys = _connection.GetServer(host, port).Keys();

            return keys.Select(item => item.ToString()).ToList();
        }

        public void SaveIdToRegion(string id, string country)
        {
            _db.StringSet(id, DICT_OF_COUNTRIES_TO_REGIONS[country]);
        }
    }
}
