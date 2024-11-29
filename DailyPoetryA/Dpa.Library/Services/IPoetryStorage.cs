using System.Linq.Expressions;
using Dpa.Library.Models;

namespace Dpa.Library.Services;

public interface IPoetryStorage
{
    bool IsInitialized { get; }
    /// <summary>
    /// 初始化数据库(将数据库部署到用户端)
    /// </summary>
    /// <returns></returns>
    Task InitializeAsync();

    /// <summary>
    /// 根据id获取诗词
    /// </summary>
    /// <returns>诗</returns>
    Task<Poetry> GetPoetryAsync(int id);

    /// <summary>
    /// 根据查询条件获取诗词列表
    /// </summary>
    /// <param name="where"></param>
    /// <param name="skip">翻页功能</param>
    /// <param name="take">翻页功能</param>
    /// <returns></returns>
    Task<IList<Poetry>> QueryAsync(Expression<Func<Poetry, bool>> where, int skip, int take);
}