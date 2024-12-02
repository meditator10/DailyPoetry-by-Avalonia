using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace Dpa.Library.Models;
/*
 * 因为这里的收藏类独立出一个单独类，他与诗词数据库不相关，诗词数据库是只读数据库，收藏单独创建一个数据库
 *
 * 关于什么样的类可以被称之为模型类
 *【从ASP.NET及EntityFramework出发探讨MVVM中的Model究竟是什么】https://www.bilibili.com/video/BV1Vz421f7Do
 */
public class Favorite : ObservableObject
{
    [PrimaryKey] public int PoetryId { get; set; }

    private bool _isFavorite;

    public virtual bool IsFavorite
    {
        get => _isFavorite;
        set => SetProperty(ref _isFavorite, value);
    }

    public long Timestamp { get; set; }
}