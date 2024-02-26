using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _db;

    public SummaryModel(ILogger<SummaryModel> logger, IConnectionMultiplexer connectionMultiplexer)
    {
        _logger = logger;
        _connectionMultiplexer = connectionMultiplexer;
        _db = _connectionMultiplexer.GetDatabase();
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        //TODO: проинициализировать свойства Rank и Similarity значениями из БД
        string rankKey = "RANK-" + id;
        if (_db.KeyExists(rankKey))
        {
            string rankString = _db.StringGet(rankKey);
            _logger.LogDebug($"Raw Rank String: {rankString}");
            try
            {
                Rank = Convert.ToDouble(rankString, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
                // Обработка случая, когда значение не может быть преобразовано в double
                _logger.LogError($"Ошибка преобразования значения {rankKey} в тип double: {ex.Message}");
            }
        }

        string similarityKey = "SIMILARITY-" + id;
        if (_db.KeyExists(similarityKey))
        {
            int similarity;
            if (int.TryParse(_db.StringGet(similarityKey), out similarity))
            {
                Similarity = similarity;
            }
            else
            {
                // Обработка случая, когда значение не может быть преобразовано в int
                _logger.LogError($"Ошибка преобразования значения {similarityKey} в тип int");
            }
        }
        // END TODO
    }
}
