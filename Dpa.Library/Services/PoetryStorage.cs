using System.Linq.Expressions;
using Dpa.Library.Helpers;
using Dpa.Library.Models;
using SQLite;

namespace Dpa.Library.Services;

public class PoetryStorage : IPoetryStorage
{
    public const int NumberPoetry = 30;

    // 编译时就是常量，运行时肯定也是常量
    public const string DbName = "poetrydb.sqlite3";
    // 运行时是常量，编译时不一定是常量，根据不同平台自动生成的路径适合用 static readonly
    public static readonly string PoetryDbPath = PathHelper.GetLocalFilePath(DbName);

    // 数据库连接
    private SQLiteAsyncConnection _connection;
    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(PoetryDbPath);

    // 依赖 偏好存储 来保存数据库版本
    private readonly IPreferenceStorage _preferenceStorage;
    public PoetryStorage(IPreferenceStorage preferenceStorage)
    {
        _preferenceStorage = preferenceStorage;
    }

    public bool IsInitialized => 
        _preferenceStorage.Get(PoetryStorageConstant.VersionKey, default(int)) 
            == PoetryStorageConstant.Version;
    public async Task InitializeAsync()
    {
        // 打开目标文件流
        await using var dbFileStream = 
            new FileStream(PoetryDbPath, FileMode.OpenOrCreate);
        // 打开多个目标文件
        // using (FileStream f1 = new FileStream("f1.txt", FileMode.OpenOrCreate),
        //        f2 = new FileStream("f2", FileMode.OpenOrCreate),
        //        f3 = new FileStream("f3", FileMode.OpenOrCreate))
        // {
        //     await Console.Error.WriteLineAsync("FileStream created!");
        // }

        // 打开来源文件流
        await using var dbAssetStream = 
            typeof(PoetryStorage)
                .Assembly
                .GetManifestResourceStream(DbName);
        
        // 把数据库文件拷贝到目标位置上
        await dbAssetStream?.CopyToAsync(dbFileStream);

        // update：同时将数据库版本写到偏好存储里
        _preferenceStorage.Set(PoetryStorageConstant.VersionKey, PoetryStorageConstant.Version);
    }

    public async Task<Poetry> GetPoetryAsync(int id) =>
        await Connection
            .Table<Poetry>()
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IList<Poetry>> QueryAsync(
        Expression<Func<Poetry, bool>> where, int skip, int take) =>
            await Connection
                .Table<Poetry>().Where(where).Skip(skip).Take(take)
                .ToListAsync();

    // 被使用于单元测试
    public async Task CloseAsync() => await Connection.CloseAsync();
}

public static class PoetryStorageConstant
{
    public const int Version = 1;

    //public const string VersionKey = "DbVersionKey";
    public const string VersionKey = 
        nameof(PoetryStorageConstant) + "." + nameof(Version);
}

