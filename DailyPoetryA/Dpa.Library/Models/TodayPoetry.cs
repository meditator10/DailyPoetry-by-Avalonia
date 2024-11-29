namespace Dpa.Library.Models;

public class TodayPoetry
{
    public string Name { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Dynasty { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Snippet { get; set; } = string.Empty;

    // 这个属性的目的是区分获取的诗词是来自服务器还是本地数据，只会赋值为常量
    public string Source { get; set; } = string.Empty;
}