using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NATS.Client;
using System.Text;
using System.Text.Json;
using Valuator.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IRedisStorage _redisStorage;

    public IndexModel(ILogger<IndexModel> logger, IRedisStorage storage)
    {
        _logger = logger;
        _redisStorage = storage;
    }

    public void OnGet()
    {

    }

    public class TextInfo
    {
        public string Id { get; set; }
        public string Result { get; set; }

        public TextInfo(string id, string result)
        {
            Id = id;
            Result = result;
        }
    }

    public IActionResult OnPost(string text)
    {
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;

        string similarityKey = "SIMILARITY-" + id;
        string similarity = GetSimilarity(text, id).ToString();

        _redisStorage.Save(similarityKey, similarity);
        _redisStorage.Save(textKey, text);

        CancellationTokenSource cts = new CancellationTokenSource();
        ProduceAsync(cts.Token, id, textKey, similarity);
        cts.Cancel();

        return Redirect($"summary?id={id}");
    }

    private int GetSimilarity(string text, string id)
    {
        var keys = _redisStorage.GetKeys();
        string textPrefix = "TEXT-";

        foreach (var value in keys)
        {
            if (value.StartsWith(textPrefix) && _redisStorage.Get(value) == text)
            {
                return 1;
            }
        }

        return 0;
    }

    private async Task ProduceAsync(CancellationToken ct, string id, string textKey, string similarity)
    {
        ConnectionFactory cf = new ConnectionFactory();

        using (IConnection c = cf.CreateConnection())
        {
            byte[] data = Encoding.UTF8.GetBytes(id);

            TextInfo? info = new(textKey, similarity);
            string jsonData = JsonSerializer.Serialize(info);
            byte[] jsonDataEncoded = Encoding.UTF8.GetBytes(jsonData);

            c.Publish("valuator.processing.rank", data);
            c.Publish("similarityCalculated", jsonDataEncoded);
            await Task.Delay(1000);
            c.Drain();

            c.Close();
        }
    }
}
