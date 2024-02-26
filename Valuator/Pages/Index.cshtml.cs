using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _db;

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer connectionMultiplexer)
    {
        _logger = logger;
        _connectionMultiplexer = connectionMultiplexer;
        _db = _connectionMultiplexer.GetDatabase();
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost(string text)
    {
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;
        //TODO: сохранить в БД text по ключу textKey
        _db.StringSet(textKey, text);
        // END TODO

        string rankKey = "RANK-" + id;
        //TODO: посчитать rank и сохранить в БД по ключу rankKey
        double rank = CalculateRank(text);
        _db.StringSet(rankKey, rank);
        // END TODO

        string similarityKey = "SIMILARITY-" + id;
        //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
        int similarity = IsSimilitary(text, id) ? 1 : 0;
        _db.StringSet(similarityKey, similarity);
        // END TODO


        return Redirect($"summary?id={id}");
    }

    private double CalculateRank(string text)
    {
        int totalCharacters = text.Length;
        int nonAlphabeticCharacters = 0;

        foreach (char character in text)
        {
            if (!char.IsLetter(character))
            {
                nonAlphabeticCharacters++;
            }
        }

        double contentRank = (double)nonAlphabeticCharacters / totalCharacters;

        return contentRank;
    }

    public bool IsSimilitary(string text, string currentId)
    {
        var allKeys = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints()[0]).Keys();

        foreach (var key in allKeys)
        {
            bool isKeyInvalidText = key.ToString().StartsWith("TEXT-") && !key.ToString().EndsWith(currentId) && string.IsNullOrEmpty(text);

            if (isKeyInvalidText)
            {
                string storedText = _db.StringGet(key);
                if (text.Equals(storedText, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
