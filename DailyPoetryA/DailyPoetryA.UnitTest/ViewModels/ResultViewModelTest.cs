using DailyPoetryA.UnitTest.Helpers;
using Dpa.Library.Models;
using Dpa.Library.ViewModels;
using System.Linq.Expressions;

namespace DailyPoetryA.UnitTest.ViewModels;

public class ResultViewModelTest : IDisposable
{
    public ResultViewModelTest() => PoetryStorageHelper.RemoveDatabaseFile();

    public void Dispose() => PoetryStorageHelper.RemoveDatabaseFile();

    [Fact]
    public async Task PoetryCollection_Default()
    {
        var where = Expression.Lambda<Func<Poetry, bool>>(
            Expression.Constant(true),
            Expression.Parameter(typeof(Poetry), "p"));

        // Arrange
        var poetryStorage = await PoetryStorageHelper.GetInitializedPoetryStorage();
        var resultViewModel = new ResultViewModel(poetryStorage, null);

        var statusList = new List<string>();
        resultViewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(resultViewModel.Status))
            {
                statusList.Add(resultViewModel.Status);
            }
        };

        Assert.Equal(0, resultViewModel.PoetryCollection.Count);
        // Act
        await resultViewModel.PoetryCollection.LoadMoreAsync();
        // Assert
        Assert.Equal(20, resultViewModel.PoetryCollection.Count);
        Assert.Equal("观书有感", resultViewModel.PoetryCollection.Last().Name);
        Assert.True(resultViewModel.PoetryCollection.CanLoadMore);
        Assert.Equal(2, statusList.Count);
        Assert.Equal(ResultViewModel.Loading, statusList[0]);
        Assert.Equal("", statusList[1]);

        var poetryCollectionChanged = false;
        resultViewModel.PoetryCollection.CollectionChanged += (sender, args) => poetryCollectionChanged = true;
        // Act
        await resultViewModel.PoetryCollection.LoadMoreAsync();
        // Assert
        Assert.True(poetryCollectionChanged);
        Assert.Equal(30, resultViewModel.PoetryCollection.Count);
        Assert.Equal("记承天寺夜游",
            resultViewModel.PoetryCollection[29].Name);
        Assert.False(resultViewModel.PoetryCollection.CanLoadMore);
        Assert.Equal(5, statusList.Count);
        Assert.Equal("", statusList[3]);

        await poetryStorage.CloseAsync();
    }
}