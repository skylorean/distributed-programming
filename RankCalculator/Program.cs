using NATS.Client;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace RankCalculator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redisConnection.GetDatabase();
            ConnectionFactory cf = new ConnectionFactory();
            IConnection c = cf.CreateConnection();

            var s = c.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                //string id = Encoding.UTF8.GetString(args.Message.Data);

                //string textKey = "TEXT-" + id;
                //string text = db.StringGet(textKey);

                //string rankKey = "RANK-" + id;
                //string rank = GetRank(text);

                //db.StringSet(rankKey, rank);

                //TextInfo data = new(textKey, rank);
                //string jsonData = JsonSerializer.Serialize(data);
                //byte[] jsonDataEnc = Encoding.UTF8.GetBytes(jsonData);
                //c.Publish("rankCalculated", jsonDataEnc);

                string data = Encoding.UTF8.GetString(args.Message.Data);
                RegionText? structData = JsonSerializer.Deserialize<RegionText>(data);
                string dbEnvironmentVariable = $"DB_{structData?.country}";
                string? dbConnection = Environment.GetEnvironmentVariable(dbEnvironmentVariable);

                if (dbConnection == null)
                {
                    return;
                }

                IDatabase savingDb = ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(dbConnection)).GetDatabase();

                string textKey = "TEXT-" + structData?.textId;
                string? text = savingDb?.StringGet(textKey);
                string rankKey = "RANK-" + structData?.textId;

                string rank = GetRank(text);

                db.StringSet(rankKey, rank);

                if (structData == null)
                {
                    return;
                }
                TextInfo textData = new TextInfo(structData.textId, rank);

                string jsonData = JsonSerializer.Serialize(textData);

                byte[] jsonDataEnc = Encoding.UTF8.GetBytes(jsonData);
                c.Publish("rankCalculated", jsonDataEnc);
            });

            //s.Start();

            //Console.WriteLine("Press Enter to exit");
            //Console.ReadLine();

            s.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            s.Unsubscribe();

            c.Drain();
            c.Close();
        }

        static string GetRank(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "0";
            }

            double len = text.Length;
            double notLetterCount = 0;
            foreach (char value in text)
            {
                if (!char.IsLetter(value))
                {
                    notLetterCount++;
                }
            }

            string count = Convert.ToString(notLetterCount / len);

            return count;
        }
    }
}