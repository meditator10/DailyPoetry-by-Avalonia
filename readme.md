# 1、服务定位器模式
将View与ViewModel关联起来，需要一个中间人服务定位器（ServiceLocator）
首先把ServiceLocator注册成一个资源，接着把Viewmodel通过依赖注入的方式注册到ServiceLocator里面，然后就可以通过GetService方法_serviceProvider.GetService()取到Viewmodel实例，这样View通过DataContext的Sourse就能绑定ServiceLocator，并找到ViewModel。

ServiceLocator和DependencyInjection是实现IoC的两种不同方式。第三种方式是FactoryPattern。ServiceLocator强调预先定义资源映射，隔离资源的具体实现，也就是在所实现类的外部某处去注册服务。DependencyInjection强调辅助创建和管理对象，在创建对象时采用构造函数注入和属性Setter注入等方式。
名字不同，但都专注于解决隔离依赖对象的问题。依赖项被隔离了，则做UT等就方便多了，同时也使配置或运行时指定、替换对象实现成为可能。

服务定位器模式介绍和评论区的讨论

# 2、Avalonia.Xaml.Behaviors
使用前记得声明xmlns:i="clr-namespace:Avalonia.Xaml.Interactivity;assembly=Avalonia.Xaml.Interactivity"
作用：当一个类的功能已经确定了，不可更改，但随着版本的迭代需要添加新的方法来扩展功能，Behavior提供一种可能性，在原有的基础上给特定类添加新的方法。

此项目中，因为数据在DaliyPoetryA.Library里，显示数据在UI层DaliyPoetryA.Client里，这里对于不能基于Command绑定的控件时，只能通过传统的事件来注册，通过事件来触发命令实现
    <i:Interaction.Behaviors>
        <!-- Initialized是xaml页面自带的事件，用于自动初始化页面 -->
        <EventTriggerBehavior EventName ="Initialized">
			<InvokeCommandAction Command ="{Binding OnInitializedCommand}"/>
        </EventTriggerBehavior>
    </i:Interaction.Behaviors>
# 3、数据绑定原理
对于VM里如果是单独的属性：ViewModel层会调用SetProperty函数来改变属性值，接着调用OnPropertyChanged函数，同时会触发PropertyChanged事件，通知View层数据发生了改变
对于集合，有专门的ObservableCollection，底层实现了ViewModel层数据改变自动通知View层 
```C#
public abstract class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
{
  public event PropertyChangedEventHandler? PropertyChanged;
  public event PropertyChangingEventHandler? PropertyChanging;

  protected bool SetProperty<T>(
      [NotNullIfNotNull("newValue")] ref T field,
      T newValue,
      IEqualityComparer<T> comparer,
      [CallerMemberName] string? propertyName = null)
  {
      CommunityToolkit.Mvvm.ArgumentNullException.ThrowIfNull((object) comparer, nameof (comparer));
      if (comparer.Equals(field, newValue))
        return false;
      this.OnPropertyChanging(propertyName);
      field = newValue;
      this.OnPropertyChanged(propertyName);
      return true;
  }
}

public interface INotifyPropertyChanged
{
    /// <summary>Occurs when a property value changes.</summary>
    event PropertyChangedEventHandler? PropertyChanged;
}
```
名称规范：抽象类名应该是名词，接口名应该是形容词或副词，这是微软设计手册的推荐命名原则之一

# 4、访问Web服务
## 4.1 诗词(文本)数据
## 4.2 图片数据
# 5、导航机制
## 5.1 初始化界面导航
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
## 5.2 一级菜单导航
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
## 5.3 二级内容导航
（有栈的，可以返回上一界面）（考虑传参）
有三个界面：
TodayDetailViewModel和TodayDetailView：
ResultViewModel和ResultView：
DetailViewModel和DetailView：
由各自的界面分别导航到各自的二级内容页：

## 5.4 导航总结
大体有三种方式实现导航
1、载体：这三种方式都需要依赖一个载体(中间人)
1. TabControl+TabItem
2. Frame+Page
3. ContentControl+UserControl
2、映射：导航会有一个NavigationTo方法，它的参数应该基于View还是ViewModel去导航
1. Url                      ——————View
2. Page					    ——————View
3. PageViewModel			——————ViewModel——View
4. typeof（PageViewModel）	——————ViewModel——View
如果是基于View的，那么可以直接渲染界面，如果是基于ViewModel的，需要先找到对应的View，再去渲染界面
1. 可以在View的构造函数中直接实例化ViewModel，或者将DataContext绑定到ViewModel
2. ContentControl的Content属性绑定到ViewModel，并通过switch case与view建立联系
3. ContentControl的Content属性绑定到ViewModel，后面通过DataTemplate的DataType将ViewModel与View建立联系
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
4. 在IoC中注册，此时View就是有参构造，在实例化的时候需要传一个ViewModel
5. 通过更改名称的字符串直接映射，比如Avalonia的ViewLocator、Prism的ViewModelLocator
3、服务：
1. 纯UI实现
2. 单例+IoC注入
3. Messager
Sean's demo
1、载体：ContentControl+UserControl
2、映射：ContentControl的Content属性绑定到ViewModel，后面通过DataTemplate的DataType将ViewModel与View建立联系
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

# 6、批量显示数据
## 6.1 显示Model
## 6.2 显示ViewModel