# 1������λ��ģʽ
��View��ViewModel������������Ҫһ���м��˷���λ����ServiceLocator��
���Ȱ�ServiceLocatorע���һ����Դ�����Ű�Viewmodelͨ������ע��ķ�ʽע�ᵽServiceLocator���棬Ȼ��Ϳ���ͨ��GetService����_serviceProvider.GetService()ȡ��Viewmodelʵ��������Viewͨ��DataContext��Sourse���ܰ�ServiceLocator�����ҵ�ViewModel��

ServiceLocator��DependencyInjection��ʵ��IoC�����ֲ�ͬ��ʽ�������ַ�ʽ��FactoryPattern��ServiceLocatorǿ��Ԥ�ȶ�����Դӳ�䣬������Դ�ľ���ʵ�֣�Ҳ��������ʵ������ⲿĳ��ȥע�����DependencyInjectionǿ�����������͹�������ڴ�������ʱ���ù��캯��ע�������Setterע��ȷ�ʽ��
���ֲ�ͬ������רע�ڽ������������������⡣����������ˣ�����UT�Ⱦͷ�����ˣ�ͬʱҲʹ���û�����ʱָ�����滻����ʵ�ֳ�Ϊ���ܡ�

����λ��ģʽ���ܺ�������������

# 2��Avalonia.Xaml.Behaviors
ʹ��ǰ�ǵ�����xmlns:i="clr-namespace:Avalonia.Xaml.Interactivity;assembly=Avalonia.Xaml.Interactivity"
���ã���һ����Ĺ����Ѿ�ȷ���ˣ����ɸ��ģ������Ű汾�ĵ�����Ҫ����µķ�������չ���ܣ�Behavior�ṩһ�ֿ����ԣ���ԭ�еĻ����ϸ��ض�������µķ�����

����Ŀ�У���Ϊ������DaliyPoetryA.Library���ʾ������UI��DaliyPoetryA.Client�������ڲ��ܻ���Command�󶨵Ŀؼ�ʱ��ֻ��ͨ����ͳ���¼���ע�ᣬͨ���¼�����������ʵ��
    <i:Interaction.Behaviors>
        <!-- Initialized��xamlҳ���Դ����¼��������Զ���ʼ��ҳ�� -->
        <EventTriggerBehavior EventName ="Initialized">
			<InvokeCommandAction Command ="{Binding OnInitializedCommand}"/>
        </EventTriggerBehavior>
    </i:Interaction.Behaviors>
# 3�����ݰ�ԭ��
����VM������ǵ��������ԣ�ViewModel������SetProperty�������ı�����ֵ�����ŵ���OnPropertyChanged������ͬʱ�ᴥ��PropertyChanged�¼���֪ͨView�����ݷ����˸ı�
���ڼ��ϣ���ר�ŵ�ObservableCollection���ײ�ʵ����ViewModel�����ݸı��Զ�֪ͨView�� 
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
���ƹ淶����������Ӧ�������ʣ��ӿ���Ӧ�������ݴʻ򸱴ʣ�����΢������ֲ���Ƽ�����ԭ��֮һ

# 4������Web����
## 4.1 ʫ��(�ı�)����
## 4.2 ͼƬ����
# 5����������
## 5.1 ��ʼ�����浼��
��ʼ������ͨ��InitializationViewModel��InitializationView��ʵ�֡�
���ǳ�������UI��Ҫһ�����壬����������MainWindowViewModel��MainWindow����������ͨ���������� RootNavigationService����������MainWindowViewModel������һ��Content���ԣ�ͨ��Contentָ��������InitializationViewҳ��ȥ��ʾ��
���APP�Ѿ���ʼ������ôֱ�ӵ�����һ���˵��Ľ����Ƽ�����
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

    // ע�⣺����ο���Avalonia�ٷ�ʾ������ViewModel�����һ��ViewModel
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
## 5.2 һ���˵�����
��û��ջ�����Ƶ�����ֱ���л���
�������������棺
TodayViewModel��TodayView(�����Ƽ�)����ȡbing������ÿ��ͼƬ������ʫ�ʷ�����ÿ��ʫ��
QueryViewModel��QueryView(ʫ������)���û�����ҳ
FavoriteViewModel��FavoriteView(ʫ���ղ�)���û��ղ�ҳ
����Ҳ��һ������MainViewModel��MainView��ͨ����������MenuNavigationService���ڱ�ǩ��������MainViewModel����Ҳ������һ��Content���ԣ�ͨ��View���ContentControl�󶨵�Content��
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

    // ��־λ�����Ƶ����������غ�չ��
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

    // ע�⣺����ο���Avalonia�ٷ�ʾ������ViewModel�����һ��ViewModel
    private ViewModelBase _content;
    public ViewModelBase Content
    {
        get => _content;
        private set => SetProperty(ref _content, value);
    }

    // ��һ��ObservableCollectionά�����ݵ�������ջ��ջ
    public ObservableCollection<ViewModelBase> ContentStack { get; } = [];
    // ��ջ(���뵽��һҳ)
    public void PushContent(ViewModelBase content)
    {
        Content = content;
        ContentStack.Insert(0, Content);
    }
    // ��ջ(�˳�����һҳ)
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
ps��View���õ��˵��������Projektanker.Icons.Avalonia.FontAwesomeȥ��ʾͼ��
## 5.3 �������ݵ���
����ջ�ģ����Է�����һ���棩�����Ǵ��Σ�
���������棺
TodayDetailViewModel��TodayDetailView��
ResultViewModel��ResultView��
DetailViewModel��DetailView��
�ɸ��ԵĽ���ֱ𵼺������ԵĶ�������ҳ��

## 5.4 �����ܽ�
���������ַ�ʽʵ�ֵ���
1�����壺�����ַ�ʽ����Ҫ����һ������(�м���)
1. TabControl+TabItem
2. Frame+Page
3. ContentControl+UserControl
2��ӳ�䣺��������һ��NavigationTo���������Ĳ���Ӧ�û���View����ViewModelȥ����
1. Url                      ������������View
2. Page					    ������������View
3. PageViewModel			������������ViewModel����View
4. typeof��PageViewModel��	������������ViewModel����View
����ǻ���View�ģ���ô����ֱ����Ⱦ���棬����ǻ���ViewModel�ģ���Ҫ���ҵ���Ӧ��View����ȥ��Ⱦ����
1. ������View�Ĺ��캯����ֱ��ʵ����ViewModel�����߽�DataContext�󶨵�ViewModel
2. ContentControl��Content���԰󶨵�ViewModel����ͨ��switch case��view������ϵ
3. ContentControl��Content���԰󶨵�ViewModel������ͨ��DataTemplate��DataType��ViewModel��View������ϵ
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
4. ��IoC��ע�ᣬ��ʱView�����вι��죬��ʵ������ʱ����Ҫ��һ��ViewModel
5. ͨ���������Ƶ��ַ���ֱ��ӳ�䣬����Avalonia��ViewLocator��Prism��ViewModelLocator
3������
1. ��UIʵ��
2. ����+IoCע��
3. Messager
Sean's demo
1�����壺ContentControl+UserControl
2��ӳ�䣺ContentControl��Content���԰󶨵�ViewModel������ͨ��DataTemplate��DataType��ViewModel��View������ϵ
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
3�����񣺵���+IoCע��

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
	
    // �Ľ���ͨ�����ͣ�ֱ��ȥServiceҪ��Ӧ��ViewModel�������Ǹ��ݴ���Ĳ�������ȡ
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

# 6��������ʾ����
## 6.1 ��ʾModel
## 6.2 ��ʾViewModel