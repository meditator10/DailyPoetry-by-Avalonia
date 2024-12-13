# 1、访问Web服务
## 1.1 诗词(文本)数据
## 1.2 图片数据
# 2、导航机制
## 2.1 初始化界面导航
初始化界面通过InitializationViewModel和InitializationView来实现。
但是承载整个UI需要一个载体，这个载体就是MainWindowViewModel和MainWindow，而且这里通过创建服务 RootNavigationService（在主界面MainWindowViewModel里面有一个Content属性，通过Content指定导航到InitializationView页面去显示）
如果APP已经初始化，那么直接导航到一级菜单的今日推荐界面
```C#
public class MainWindowViewModel : ViewModelBase
{
    private readonly IRootNavigationService _rootNavigationService;
    private readonly IPoetryStorage _poetryStorage;
    private readonly IMenuNavigationService _menuNavigationService;

    public MainWindowViewModel(IRootNavigationService rootNavigationService, 
        IPoetryStorage poetryStorage,
        IMenuNavigationService menuNavigationService)
    {
        _rootNavigationService = rootNavigationService;
        _poetryStorage = poetryStorage;
        _menuNavigationService = menuNavigationService;
        OnInitializedCommand = new RelayCommand(OnInitialized);
    }

    // 注意：这里参考了Avalonia官方示例，在ViewModel里包含一个ViewModel
    private ViewModelBase _content;
    public ViewModelBase Content
    {
        get => _content; 
        set => SetProperty(ref _content, value);
    }

    public ICommand OnInitializedCommand { get; }
    public void OnInitialized()
    {
        if (!_poetryStorage.IsInitialized)
        {
            _rootNavigationService.NavigateTo(RootNavigationConstant.InitializationView);
        }
        else
        {
            _rootNavigationService.NavigateTo(RootNavigationConstant.MainView);
            _menuNavigationService.NavigateTo(MenuNavigationConstant.TodayView);
        }
    }
}
```
## 2.2 一级菜单导航
（没有栈，类似导航栏直接切换）
这里有三个界面：
TodayViewModel和TodayView(今日推荐)：获取bing服务器每日图片、今日诗词服务器每日诗词
QueryViewModel和QueryView(诗词搜索)：用户搜索页
FavoriteViewModel和FavoriteView(诗词收藏)：用户收藏页
这里也有一个载体MainViewModel和MainView，通过创建服务MenuNavigationService（在标签导航界面MainViewModel里面也定义了一个Content属性，通过View层的ContentControl绑定到Content）
```C#
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

    public RelayCommand OpenPaneCommand { get; }
    public void OpenPane() => IsPaneOpen = true;
    public RelayCommand ClosePaneCommand { get; }
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
    public RelayCommand GoBcakCommand { get; }
    public void GoBack()
    {
        if (ContentStack.Count <= 1)
            return;
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
        SelectedMenuItem = MenuItem.MenuItems.FirstOrDefault(p => p.View == view);
        Title = SelectedMenuItem.Name;
        IsPaneOpen = false;
    }

    public RelayCommand OnMenuTappedCommand { get; }
    public void OnMenuTapped()
    {
        if (SelectedMenuItem is null)
            return;
        _menuNavigationService.NavigateTo(SelectedMenuItem.View);
    }
}
```
ps：View层用到了第三方组件Projektanker.Icons.Avalonia.FontAwesome去显示图标
## 2.3 二级内容导航
（有栈的，可以返回上一界面）（考虑传参）
有三个界面：
TodayDetailViewModel和TodayDetailView：
ResultViewModel和ResultView：
DetailViewModel和DetailView：
由各自的界面分别导航到各自的二级内容页：

## 2.4 导航总结
大体有三种方式实现导航
1、载体：这三种方式都需要依赖一个载体(中间人)
1. TabControl+TabItem
2. Frame+Page
3. ContentControl+UserControl
2、映射：导航会有一个NavigationTo方法，它的参数应该基于View还是ViewModel去导航
1. Url                      	——————View
2. Page				——————View
3. PageViewModel		——————ViewModel——View
4. typeof（PageViewModel）	——————ViewModel——View
如果是基于View的，那么可以直接渲染界面，如果是基于ViewModel的，需要先找到对应的View，再去渲染界面
1. 可以在View的构造函数中直接实例化ViewModel，或者将DataContext绑定到ViewModel
2. ContentControl的Content属性绑定到ViewModel，并通过switch case与view建立联系
3. ContentControl的Content属性绑定到ViewModel，后面通过DataTemplate的DataType将ViewModel与View建立联系
```xml
    <ContentControl Content="{Binding CurrentViewModel}" >
        <ContentControl.Resources>
            <DataTemplate DataType="{x:Type vms:HomeViewModel}">
                <vs:HomeView/>
            </DataTemplate>
            <DataTemplate DataType="{x:Type vms:AccountViewModel}">
                <vs:AccountView/>
            </DataTemplate>
        </ContentControl.Resources>
    </ContentControl>
```
5. 在IoC中注册，此时View就是有参构造，在实例化的时候需要传一个ViewModel
6. 通过更改名称的字符串直接映射，比如Avalonia的ViewLocator、Prism的ViewModelLocator
3、服务：
1. 纯UI实现
2. 单例+IoC注入
3. Messager
## 2.5 Sean's demo
1、载体：ContentControl+UserControl
2、映射：ContentControl的Content属性绑定到ViewModel，后面通过DataTemplate的DataType将ViewModel与View建立联系
```xml
<ContentControl Content="{Binding CurrentViewModel}">
	<ContentControl.Resources>
		<DataTemplate DataType="{x:Type vm:LoginViewModel}">
			<v:LoginView />
		</DataTemplate>
		<DataTemplate DataType="{x:Type vm:HomeViewModel}">
			<v:HomeView />
		</DataTemplate>
	</ContentControl.Resources>
</ContentControl>
```
3、服务：单例+IoC注入
```C#
public class NavigationService
{
    private ViewModelBase? _viewModel;
    public ViewModelBase? ViewModel
    {
        get => _viewModel;
        set
        {
            _viewModel = value;
            // notify property changed
            ViewModelChanged?.Invoke();
        }
    }
    public event Action? ViewModelChanged;
	
    // 改进：通过泛型，直接去Service要对应的ViewModel，而不是根据传入的参数来获取
    public void NavigateTo<T>() where T : ViewModelBase => 
        ViewModel = App.Current.Services.GetService<T>();
}
public partial class App : Application
{
    public IServiceProvider Services { get; }

    public static new App Current => (App)Application.Current;

    public App()
    {
        var container = new ServiceCollection();

        container.AddSingleton<MainWindow>();
        container.AddSingleton<MainWindowViewModel>();

        container.AddSingleton<NavigationService>();
        container.AddTransient<HomeViewModel>();
        container.AddTransient<LoginViewModel>();

        container.AddSingleton<UserService>();

        Services = container.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }
}
```
# 3、批量显示数据
## 3.1 显示Model
## 3.2 显示ViewModel
