namespace Dpa.Library.Models;

public class TodayImage
{
    // 用于缓存记录
    public string FullStartDate { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }

    // 界面显示
    public string Copyright { get; set; } = string.Empty;
    public string CopyrightLink { get; set; } = string.Empty;
    public byte[] ImageBytes { get; set; }
}