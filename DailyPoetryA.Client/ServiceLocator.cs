using System;
using Avalonia;
using DailyPoetryA.Client.Services;
using Dpa.Library.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Dpa.Library.Services;

namespace DailyPoetryA.Client;

public class ServiceLocator
{
    private readonly IServiceProvider _serviceProvider;


    private static ServiceLocator _current;
    // 从全局的静态资源中获取ServiceLocator，为了供C#代码使用Locator，xaml代码可以直接binding
    public static ServiceLocator Current
    {
        get
        {
            if(_current is not null)
                return _current;

            if (Application.Current
                    .TryGetResource(nameof(ServiceLocator),
                                    null,
                                    out var resource)
                && resource is ServiceLocator serviceLocation)
            {
                return _current = serviceLocation;
            }
            throw new Exception("理论上不会发生");
        }
    }

    // 从依赖注入容器中获取viewmodel
    public MainWindowViewModel MainWindowViewModel => 
        _serviceProvider.GetRequiredService<MainWindowViewModel>();

    public MainViewModel MainViewModel =>
        _serviceProvider.GetRequiredService<MainViewModel>();

    public TodayViewModel TodayViewModel =>
        _serviceProvider.GetRequiredService<TodayViewModel>();
    public QueryViewModel QueryViewModel =>
        _serviceProvider.GetRequiredService<QueryViewModel>();
    public FavoriteViewModel FavoriteViewModel =>
        _serviceProvider.GetRequiredService<FavoriteViewModel>();

    public InitializationViewModel InitializationViewModel =>
        _serviceProvider.GetRequiredService<InitializationViewModel>();

    public TodayDetailViewModel TodayDetailViewModel =>
        _serviceProvider.GetRequiredService<TodayDetailViewModel>();
    public DetailViewModel DetailViewModel =>
        _serviceProvider.GetRequiredService<DetailViewModel>();
    public ResultViewModel ResultViewModel =>
        _serviceProvider.GetRequiredService<ResultViewModel>();

    // TODO delete
    public IRootNavigationService RootNavigationService => 
        _serviceProvider.GetRequiredService<IRootNavigationService>();

    public ServiceLocator()
    {
        var serviceCollection = new ServiceCollection();

        // 把各种对象(类或者ViewModel等)注册到ServiceCollection里面
        serviceCollection.AddSingleton<IPoetryStorage, PoetryStorage>();
        serviceCollection.AddSingleton<IPreferenceStorage, FilePreferenceStorage>();
        serviceCollection.AddSingleton<ResultViewModel>();

        serviceCollection.AddSingleton<ITodayPoetryService, JinrishiciService>();
        serviceCollection.AddSingleton<IAlertService, AlertService>();
        serviceCollection.AddSingleton<TodayViewModel>();
        serviceCollection.AddSingleton<QueryViewModel>();
        serviceCollection.AddSingleton<FavoriteViewModel>();

        serviceCollection.AddSingleton<MainWindowViewModel>();
        serviceCollection.AddSingleton<IRootNavigationService, RootNavigationService>();
        serviceCollection.AddSingleton<MainViewModel>();

        serviceCollection.AddSingleton<IMenuNavigationService, MenuNavigationService>();
        serviceCollection.AddSingleton<InitializationViewModel>();

        serviceCollection.AddSingleton<ITodayImageService, BingImageService>();
        serviceCollection.AddSingleton<ITodayImageStorage, TodayImageStorage>();

        serviceCollection.AddSingleton<IContentNavigationService, ContentNavigationService>();
        serviceCollection.AddSingleton<TodayDetailViewModel>();
        serviceCollection.AddSingleton<DetailViewModel>();

        serviceCollection.AddSingleton<IFavoriteStorage, FavoriteStorage>();

        // 取对象
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
}

