using System.Linq.Expressions;
using DailyPoetryA.UnitTest.Helpers;
using Moq;

namespace DailyPoetryA.UnitTest.Services;

using Dpa.Library.Models;
using Dpa.Library.Services;

public class PoetryStorageTest : IDisposable
{
    public PoetryStorageTest()
    {
        // 防御式编程，在执行前先清理data
        PoetryStorageHelper.RemoveDatabaseFile();
    }

    [Fact]
    public void IsIntialized_Default()
    {
        // Arrange
        var preferenceStorageMock = new Mock<IPreferenceStorage>();

        // // Act
        // 因为preferenceStorageMock是一个假的数据，所以IPreferenceStorage.Get返回的一定是default值
        // 所以要使用Setup将他强制返回对应的值
        preferenceStorageMock
            .Setup(p => 
                p.Get(PoetryStorageConstant.VersionKey, default(int)))
            .Returns(PoetryStorageConstant.Version);

        var mockPreferenceStorage = preferenceStorageMock.Object;
        var poetryStorage = new PoetryStorage(mockPreferenceStorage);

        // Assert
        Assert.True(poetryStorage.IsInitialized);

        preferenceStorageMock
            .Verify(p=>
                p.Get(PoetryStorageConstant.VersionKey, default(int)), Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_Default()
    {
        Assert.False(File.Exists(PoetryStorage.PoetryDbPath));
        
        // Arrange：准备数据
        // mock：一般都是mock接口，而不是去mock实现类，这里引申出接口的意义
        // 接口的意义：可以做到在项目中使用实现类，在单元测试中使用mock类
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;
        var poetryStorage = new PoetryStorage(mockPreferenceStorage);

        // Act：执行操作
        await poetryStorage.InitializeAsync();

        // Assert：断言
        Assert.True(File.Exists(PoetryStorage.PoetryDbPath));
    }

    [Fact]
    public async Task GetPoetryAsync_Default()
    {
        // Arrange
        var poetryStorage = await PoetryStorageHelper.GetInitializedPoetryStorage();
        // Act
        var poetry = await poetryStorage.GetPoetryAsync(10001);
        // Assert
        Assert.Equal("临江仙 · 夜归临皋",poetry.Name);

        // win系统报错
        await poetryStorage.CloseAsync();
    }

    [Fact]
    public async Task QueryAsync_Default()
    {
        // Arrange
        var poetryStorage = await PoetryStorageHelper.GetInitializedPoetryStorage();

        // Act
        var poetries = await poetryStorage.QueryAsync(
            Expression.Lambda<Func<Poetry, bool>>(Expression.Constant(true), 
                Expression.Parameter(typeof(Poetry), "p")),
            0, 
            int.MaxValue);

        // Assert
        Assert.Equal(PoetryStorage.NumberPoetry, poetries.Count());

        await poetryStorage.CloseAsync();
    }

    public void Dispose()
    {
        PoetryStorageHelper.RemoveDatabaseFile();
    }
}