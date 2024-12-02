using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dpa.Library.Helpers;
using Dpa.Library.Models;

namespace Dpa.Library.Services;

public class JinrishiciService : ITodayPoetryService
{
    private readonly IAlertService _alertService;
    private string _domainName;
    private string _token = String.Empty;
    private readonly IPreferenceStorage _preferenceStorage;
    public static readonly string JinrishiciTokenKey = $"{nameof(JinrishiciService)}.Token";

    public JinrishiciService(IAlertService alertService,
        IPreferenceStorage preferenceStorage,
        IPoetryStorage poetryStorage,
        string domainName = "v2.jinrishici.com")
    {
        _alertService = alertService;
        _domainName = domainName;
        _preferenceStorage = preferenceStorage;
        _poetryStorage = poetryStorage;
    }

    private const string Server = "今日诗词服务器";

    public async Task<string> GetTokenAsync()
    {
        // 先看内存里面有没有缓存的token
        if (!string.IsNullOrWhiteSpace(_token))
        {
            return _token;
        }
        // 没有的话，再去偏好存储里面看看有没有缓存的token
        _token = _preferenceStorage.Get(JinrishiciTokenKey, string.Empty);
        if (!string.IsNullOrWhiteSpace(_token))
        {
            return _token;
        }

        using var httpClient = new HttpClient();

        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync($"https://{_domainName}/token");
            // (安全防御)当服务器正常连接，但是出现500、404等等问题也会抛出异常
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            // TODO: handle error message to user
            // 创建AlertService，通过依赖注入将error message捕捉给user
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(Server, e.Message));
            return _token;
        }

        var json = await response.Content.ReadAsStringAsync();
        var JinrishiciToken = JsonSerializer.Deserialize<JinrishiciToken>(json);

        _token = JinrishiciToken.Data;
        _preferenceStorage.Set(JinrishiciTokenKey, _token);
        return _token;
    }

    public async Task<TodayPoetry> GetTodayPoetryAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            // 从今日诗词获取不到诗词，就从本地数据库随机取一条诗词
            return await GetRandomPoetryAsync();
        }

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-User-Token", token);
        HttpResponseMessage response;
        // 1、网络错误
        try
        {
            response = await httpClient.GetAsync($"https://{_domainName}/sentence");
            // (安全防御)当服务器正常连接，但是出现500、404等等问题也会抛出异常
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(Server, e.Message));

            return await GetRandomPoetryAsync();
        }

        // 2、Json序列化不成功
        var json = await response.Content.ReadAsStringAsync();
        JinrishiciSentence jinrishiciSentence;
        try
        {
            jinrishiciSentence = JsonSerializer
                .Deserialize<JinrishiciSentence>(json) ?? throw new JsonException();
        }
        catch (Exception e)
        {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(Server, e.Message));

            return await GetRandomPoetryAsync();
        }

        // 3、Json序列化没问题，但是可能缺少数据
        try
        {
            return new TodayPoetry
            {
                Snippet = jinrishiciSentence.Data.Content ?? throw new JsonException(),
                Name = jinrishiciSentence.Data.Origin.Title ?? throw new JsonException(),
                Dynasty = jinrishiciSentence.Data.Origin.Dynasty ?? throw new JsonException(),
                Author = jinrishiciSentence.Data.Origin.Author ?? throw new JsonException(),
                Content = string.Join("\n",
                    jinrishiciSentence.Data.Origin.Content) ?? throw new JsonException(),
                Source = TodayPoetrySources.Jinrishici
            };
        }
        catch (Exception e)
        {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(Server, e.Message));

            return await GetRandomPoetryAsync();
        }
    }

    private readonly IPoetryStorage _poetryStorage;
    // 从今日诗词服务器获取不到诗词，就从本地数据库随机取一条诗词
    public async Task<TodayPoetry> GetRandomPoetryAsync()
    {
        var poetries = await _poetryStorage.QueryAsync(
            Expression.Lambda<Func<Poetry, bool>>(Expression.Constant(true), 
            Expression.Parameter(typeof(Poetry), "p")),
            new Random().Next(PoetryStorage.NumberPoetry), 
            1);
        var poetry = poetries.First();
        return new TodayPoetry
        {
            Snippet = poetry.Snippet,
            Name = poetry.Name,
            Dynasty = poetry.Dynasty,
            Author = poetry.Author,
            Content = poetry.Content,
            Source = TodayPoetrySources.Local
        };
    }
}

public class JinrishiciToken
{
    [JsonIgnore]
    public string Status { get; set; }
    [JsonPropertyName("data")]
    public string Data { get; set; }
}

// 用于从服务器上读取诗词的序列化类
public class JinrishiciSentence
{
    [JsonPropertyName("data")]
    public JinrishiciData Data { get; set; }
}

public class JinrishiciData
{
    // 注意这个content是快速浏览诗句Snippet
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
    [JsonPropertyName("origin")]
    public JinrishiciOrigin Origin { get; set; } = new JinrishiciOrigin();
}

public class JinrishiciOrigin
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("dynasty")]
    public string Dynasty { get; set; } = string.Empty;
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;
    [JsonPropertyName("content")]
    public List<string> Content { get; set; } = [];
}

