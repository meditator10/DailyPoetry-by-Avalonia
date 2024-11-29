using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Dpa.Library.Services;

namespace Dpa.Library.ViewModels;

public class MainViewModel : ViewModelBase
{
    private IMenuNavigationService _menuNavigationService;
    public MainViewModel(IMenuNavigationService menuNavigationService)
    {
        _menuNavigationService = menuNavigationService;

        GoBcakCommand = new RelayCommand(GoBack);
        OpenPaneCommand = new RelayCommand(OpenPane);
        ClosePaneCommand = new RelayCommand(ClosePane);

        OnMenuTappedCommand = new RelayCommand(OnMenuTapped);
    }

    private string _title = "DailyPoetry";
    public string Title
    {
        get => _title;

        private set => SetProperty(ref _title, value);
    }

    // 标志位：控制导航栏的隐藏和展开
    private bool _isPaneOpen;
    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        private set => SetProperty(ref _isPaneOpen, value);
    }

    public IRelayCommand OpenPaneCommand { get; }
    public void OpenPane() => IsPaneOpen = true;
    public IRelayCommand ClosePaneCommand { get; }
    public void ClosePane() => IsPaneOpen = false;

    // 注意：这里参考了Avalonia官方示例，在ViewModel里包含一个ViewModel
    private ViewModelBase _content;
    public ViewModelBase Content
    {
        get => _content;
        private set => SetProperty(ref _content, value);
    }

    // 用一个ObservableCollection维护内容导航的入栈出栈
    public ObservableCollection<ViewModelBase> ContentStack { get; } = [];
    // 入栈(进入到下一页)
    public void PushContent(ViewModelBase content)
    {
        Content = content;
        ContentStack.Insert(0, Content);
    }
    // 出栈(退出到上一页)
    public IRelayCommand GoBcakCommand { get; }
    public void GoBack()
    {
        if (ContentStack.Count <= 1)
        {
            return;
        }
        ContentStack.RemoveAt(0);
        Content = ContentStack[0];
    }

    private MenuItem _selectedMenuItem;

    public MenuItem SelectedMenuItem
    {
        get => _selectedMenuItem;
        set => SetProperty(ref _selectedMenuItem, value);
    }

    public void SetMenuAndContent(string view, ViewModelBase content)
    {
        ContentStack.Clear();
        PushContent(content);
        SelectedMenuItem =
            MenuItem.MenuItems.FirstOrDefault(p => p.View == view);
        Title = SelectedMenuItem.Name;
        IsPaneOpen = false;
    }

    public IRelayCommand OnMenuTappedCommand { get; }
    public void OnMenuTapped()
    {
        if (SelectedMenuItem is null)
            return;
        
        _menuNavigationService.NavigateTo(SelectedMenuItem.View);
    }
}

public class MenuItem
{
    public string View { get; private init; }
    public string Name { get; private init; }

    // 私有的构造函数，只有自己可以new
    private MenuItem()
    {

    }

    private static MenuItem TodayView =>
        new MenuItem() { Name = "今日推荐", View = MenuNavigationConstant.TodayView };

    private static MenuItem QueryView =>
        new MenuItem() { Name = "诗词搜索", View = MenuNavigationConstant.QueryView };

    private static MenuItem FavoriteView =>
        new MenuItem() { Name = "诗词收藏", View = MenuNavigationConstant.FavoriteView };

    // 由于MenuItem只能自己实例化，所以先创建三个导航tab常量，将三个常量拼成一个数组
    public static IEnumerable<MenuItem> MenuItems { get; } = 
        [TodayView, QueryView, FavoriteView];
}