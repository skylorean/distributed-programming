using NATS.Client;
using StackExchange.Redis;
using System.Text;

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
                string id = Encoding.UTF8.GetString(args.Message.Data);

                string textKey = "TEXT-" + id;
                string text = db.StringGet(textKey);

                string rankKey = "RANK-" + id;

                string rank = GetRank(text);

                db.StringSet(rankKey, rank);
            });

            s.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }

        static string GetRank(string text)
        {
            if (String.IsNullOrEmpty(text))
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