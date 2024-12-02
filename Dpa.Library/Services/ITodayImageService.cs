using Dpa.Library.Models;

namespace Dpa.Library.Services;

/// <summary>
/// 两阶段背景图片加载
/// </summary>
public interface ITodayImageService
{
    /// <summary>
    /// 从内存中直接读图，或者从缓存中读取上一次bing图片
    /// </summary>
    /// <returns>TodayImage</returns>
    Task<TodayImage> GetTodayImageAsync();

    /// <summary>
    /// 如果图片有更新，再去刷新背景图
    /// </summary>
    /// <returns>TodayImageServiceCheckUpdateResult</returns>
    Task<TodayImageServiceCheckUpdateResult> CheckUpdateAsync();
}

public class TodayImageServiceCheckUpdateResult
{
    public bool HasUpdate { get; set; }

    public TodayImage TodayImage { get; set; } = new();
}