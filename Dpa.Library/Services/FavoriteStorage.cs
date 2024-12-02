using Dpa.Library.Helpers;
using Dpa.Library.Models;
using SQLite;

namespace Dpa.Library.Services;

public class FavoriteStorage : IFavoriteStorage
{
    public const string DbName = "favoritedb.sqlite3";

    // public static readonly string PoetryDbPath =
    //     Path.Combine(
    //         Environment.GetFolderPath(Environment.SpecialFolder
    //             .LocalApplicationData), DbName);
    public static readonly string PoetryDbPath = PathHelper.GetLocalFilePath(DbName);

    public event EventHandler<FavoriteStorageUpdatedEventArgs>? Updated;

    // 数据库连接
    private SQLiteAsyncConnection _connection;
    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(PoetryDbPath);

    // 依赖 偏好存储 来保存数据库版本
    private readonly IPreferenceStorage _preferenceStorage;
    public FavoriteStorage(IPreferenceStorage preferenceStorage)
    {
        _preferenceStorage = preferenceStorage;
    }

    public bool IsInitialized =>
        _preferenceStorage.Get(FavoriteStorageConstant.VersionKey,
            default(int)) == FavoriteStorageConstant.Version;

    public async Task InitializeAsync()
    {
        // 注意这里和PoetryStorage的区别
        await Connection.CreateTableAsync<Favorite>();
        // update：同时将数据库版本写到偏好存储里
        _preferenceStorage.Set(FavoriteStorageConstant.VersionKey,
            FavoriteStorageConstant.Version);
    }

    public async Task<Favorite> GetFavoriteAsync(int poetryId) =>
        await Connection
            .Table<Favorite>()
            .FirstOrDefaultAsync(p => p.PoetryId == poetryId);

    public async Task<IEnumerable<Favorite>> GetFavoritesAsync() =>
        await Connection
            .Table<Favorite>().Where(p => p.IsFavorite)
            .OrderByDescending(p => p.Timestamp).ToListAsync();

    public async Task SaveFavoriteAsync(Favorite favorite)
    {
        favorite.Timestamp = DateTime.Now.Ticks;
        await Connection.InsertOrReplaceAsync(favorite);
        Updated?.Invoke(this, new FavoriteStorageUpdatedEventArgs(favorite));
    }

    public async Task CloseAsync() => await Connection.CloseAsync();
}

public static class FavoriteStorageConstant
{
    public const string VersionKey =
        nameof(FavoriteStorageConstant) + "." + nameof(Version);

    public const int Version = 1;
}