using Dpa.Library.Services;
using Moq;

namespace DailyPoetryA.UnitTest.Helpers;

public class PoetryStorageHelper
{
    // 删除数据库资源
    public static void RemoveDatabaseFile() => 
        File.Delete(PoetryStorage.PoetryDbPath);

    // 取自诗词数据库的测试专用数据库
    public static async Task<PoetryStorage> GetInitializedPoetryStorage()
    {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;
        var poetryStorage = new PoetryStorage(mockPreferenceStorage);
        // 迁移数据库：从资源文件——>物理硬盘上
        await poetryStorage.InitializeAsync();
        return poetryStorage;
    }
}
