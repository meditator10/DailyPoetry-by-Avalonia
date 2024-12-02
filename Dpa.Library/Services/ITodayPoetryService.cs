using Dpa.Library.Models;

namespace Dpa.Library.Services;

/*
    调用第三方今日诗词接口，您需要：
   
    1、对于每一个用户第一次访问，先使用 获取 Token ，然后存到 Storage 里面。
（ Storage表示一些长效的储存机制，如localStorage ，您不应该存储到运行内存中）
   
    2、之后每一次接口调用，把 Token 从 Storage 里面拿出来。
   
    3、使用附带 Token 的接口 ， 发送附带 Token 的请求。

 */
public interface ITodayPoetryService
{
    /// <summary>
    /// 获取今日诗词的每日推荐
    /// </summary>
    Task<TodayPoetry> GetTodayPoetryAsync();
}

public static class TodayPoetrySources
{
    public const string Jinrishici = nameof(Jinrishici);
    public const string Local = nameof(Local);
}