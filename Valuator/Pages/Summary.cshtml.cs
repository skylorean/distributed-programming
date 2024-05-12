using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;
using Valuator.Redis;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IRedisStorage _redisStorage;

    public SummaryModel(ILogger<SummaryModel> logger, IRedisStorage storage)
    {
        _logger = logger;
        _redisStorage = storage;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id, string country)
    {
        _logger.LogDebug(id);

        string dbEnvironmentVariable = $"DB_{country}";
        string? dbConnection = Environment.GetEnvironmentVariable(dbEnvironmentVariable);
        if (dbConnection == null)
        {
            return;
        }

        IDatabase db = ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(dbConnection)).GetDatabase();

        string? rankString = db.StringGet($"RANK-{id}");
        string? similarityString = db.StringGet($"SIMILARITY-{id}");

        if (similarityString == null || rankString == null)
        {
            return;
        }

        Rank = double.Parse(rankString, System.Globalization.CultureInfo.InvariantCulture);
        Similarity = double.Parse(similarityString, System.Globalization.CultureInfo.InvariantCulture);

        //TODO: проинициализировать свойства Rank и Similarity значениями из БД
        //Rank = Convert.ToDouble(_redisStorage.Get($"RANK-{id}"));

        //Similarity = Convert.ToDouble(_redisStorage.Get($"SIMILARITY-{id}"));
        //// END TODO
    }
}
