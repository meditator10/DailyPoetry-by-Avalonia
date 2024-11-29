using Dpa.Library.Models;

namespace Dpa.Library.Services;

public interface ITodayImageStorage
{
    // 读
    /*
     *  带二进制的image,指的是图像文件的二进制形式,即图像数据的原始字节序列,
     * 可以直接被计算机系统读取和处理，但对人类来说难以直接理解。
     *  不带二进制的image,指的是图像的非二进制表示,比如图像的描述、URL链接、文件路径等
     * 这种形式的数据便于人类阅读和理解，但需要进一步的处理才能获取图像的实际内容
     */
    Task<TodayImage> GetTodayImageAsync(bool isIncludingImageStream);
    // 写
    Task SaveTodayImageAsync(TodayImage todayImage, bool isSavingExpiresAtOnly);
}