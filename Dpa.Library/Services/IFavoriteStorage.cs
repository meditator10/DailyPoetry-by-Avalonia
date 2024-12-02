using Dpa.Library.Models;

namespace Dpa.Library.Services;

public interface IFavoriteStorage
{
    bool IsInitialized { get; }

    Task InitializeAsync();

    /// <summary>
    /// 获取一个收藏
    /// </summary>
    /// <param name="poetryId"></param>
    /// <returns>Favorite</returns>
    Task<Favorite> GetFavoriteAsync(int poetryId);

    /// <summary>
    /// 获取所有收藏
    /// </summary>
    /// <returns>IEnumerable<Favorite></returns>
    Task<IEnumerable<Favorite>> GetFavoritesAsync();

    Task SaveFavoriteAsync(Favorite favorite);

    event EventHandler<FavoriteStorageUpdatedEventArgs> Updated;
}

public class FavoriteStorageUpdatedEventArgs : EventArgs
{
    public Favorite UpdatedFavorite { get; }

    public FavoriteStorageUpdatedEventArgs(Favorite favorite)
    {
        UpdatedFavorite = favorite;
    }
}