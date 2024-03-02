using Microsoft.AspNetCore.Mvc.RazorPages;
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

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        //TODO: проинициализировать свойства Rank и Similarity значениями из БД
        Rank = Convert.ToDouble(_redisStorage.Get($"RANK-{id}"));

        Similarity = Convert.ToDouble(_redisStorage.Get($"SIMILARITY-{id}"));
        // END TODO
    }
}
