using Dpa.Library.Helpers;
using Dpa.Library.Models;
using System.Globalization;
using System.Text.Json;

namespace Dpa.Library.Services;

public class BingImageService : ITodayImageService
{
    private readonly ITodayImageStorage _todayImageStorage;

    private readonly IAlertService _alertService;

    private static HttpClient _httpClient = new HttpClient();

    private const string Server = "必应每日图片服务器";

    public BingImageService(ITodayImageStorage todayImageStorage,
        IAlertService alertService)
    {
        _todayImageStorage = todayImageStorage;
        _alertService = alertService;
    }

    public async Task<TodayImage> GetTodayImageAsync() =>
        // 带二进制的图片，直接返回本地数据库的图片
        await _todayImageStorage.GetTodayImageAsync(true);

    public async Task<TodayImageServiceCheckUpdateResult> CheckUpdateAsync()
    {
        // 不带二进制的图片，返回URL链接/路径
        var todayImage = await _todayImageStorage.GetTodayImageAsync(false);
        // bing服务器图片没更新
        if (todayImage.ExpiresAt > DateTime.Now)
        {
            return new TodayImageServiceCheckUpdateResult { HasUpdate = false };
        }

        // 不然bing每日图片已更新，去访问bing服务器
        HttpResponseMessage response;
        // 1、网络错误
        try
        {
            response = await _httpClient.GetAsync(
                "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=zh-CN");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(Server, e.Message));
            return new TodayImageServiceCheckUpdateResult { HasUpdate = false };
        }

        // 2、Json序列化不成功
        var json = await response.Content.ReadAsStringAsync();
        string bingImageUrl;
        try
        {
            var bingImage = JsonSerializer.Deserialize<BingImageOfTheDay>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })?.Images?.FirstOrDefault() ?? throw new JsonException();

            // 服务器返回的FullStartDate：202411251600
            var bingImageFullStartDate = DateTime.ParseExact(
                bingImage.FullStartDate ?? throw new JsonException(),
                "yyyyMMddHHmm", CultureInfo.InvariantCulture);
            // 本地图片的FullStartDate
            var todayImageFullStartDate = DateTime.ParseExact(
                todayImage.FullStartDate, "yyyyMMddHHmm",
                CultureInfo.InvariantCulture);

            // 图片没更新，将过期时间+2h
            if (bingImageFullStartDate <= todayImageFullStartDate)
            {
                todayImage.ExpiresAt = DateTime.Now.AddHours(2);
                await _todayImageStorage.SaveTodayImageAsync(todayImage, true);
                return new TodayImageServiceCheckUpdateResult
                {
                    HasUpdate = false
                };
            }
            // 服务器上新图片的文本信息
            todayImage = new TodayImage
            {
                FullStartDate = bingImage.FullStartDate,
                ExpiresAt = bingImageFullStartDate.AddDays(1),
                Copyright = bingImage.Copyright ?? throw new JsonException(),
                CopyrightLink = bingImage.CopyrightLink ??
                    throw new JsonException()
            };
            // 服务器上新图片的URL
            bingImageUrl = bingImage.Url ?? throw new JsonException();
        }
        catch (Exception e)
        {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(Server,
                    e.Message));
            return new TodayImageServiceCheckUpdateResult { HasUpdate = false };
        }

        // 3、去服务器URL获取新的不带二进制的图片
        try
        {
            response =
                await _httpClient.GetAsync(
                    "https://www.bing.com" + bingImageUrl);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(Server, e.Message));
            return new TodayImageServiceCheckUpdateResult { HasUpdate = false };
        }

        // 4、相比于获取每日诗词多一步：下载图片
        todayImage.ImageBytes = await response.Content.ReadAsByteArrayAsync();
        await _todayImageStorage.SaveTodayImageAsync(todayImage, false);
        // 将从服务器上下载的新图片返回
        return new TodayImageServiceCheckUpdateResult
        {
            HasUpdate = true,
            TodayImage = todayImage
        };
    }
}

public class BingImageOfTheDay
{
    public List<BingImageOfTheDayImage> Images { get; set; }
}

public class BingImageOfTheDayImage
{
    public string StartDate { get; set; }

    public string FullStartDate { get; set; }

    public string EndDate { get; set; }

    public string Url { get; set; }

    public string Copyright { get; set; }

    public string CopyrightLink { get; set; }
}