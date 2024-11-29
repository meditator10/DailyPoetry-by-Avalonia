using Dpa.Library.ViewModels;
using Moq;

namespace DailyPoetryA.UnitTest.ViewModels;

public class MainViewModelTest
{
    [Fact]
    public void GoBack_ContentStackCountIs0()
    {
        var mainViewModel = new MainViewModel(null);
        Assert.Empty(mainViewModel.ContentStack);
        mainViewModel.GoBack();
        Assert.Empty(mainViewModel.ContentStack);
    }

    [Fact]
    public void GoBack_ContentStackCountIs1()
    {
        var mainViewModel = new MainViewModel(null);
        var content = new Mock<ViewModelBase>().Object;

        mainViewModel.PushContent(content);
        mainViewModel.GoBack();
        Assert.Single(mainViewModel.ContentStack);
    }

    [Fact]
    public void GoBack_ContentStackCountIs2()
    {
        var mainViewModel = new MainViewModel(null);
        var content1 = new Mock<ViewModelBase>().Object;
        var content2 = new Mock<ViewModelBase>().Object;

        mainViewModel.PushContent(content1);
        mainViewModel.PushContent(content2);
        mainViewModel.GoBack();
        Assert.Single(mainViewModel.ContentStack);
        Assert.Same(content1, mainViewModel.ContentStack[0]);
    }
}