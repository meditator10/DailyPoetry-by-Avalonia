using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Dpa.Library.Services;

namespace Dpa.Library.ViewModels;

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

    public IRelayCommand OnInitializedCommand { get; }

    public void OnInitialized()
    {
        if (!_poetryStorage.IsInitialized)
        {
            _rootNavigationService.NavigateTo(RootNavigationConstant
                .InitializationView);
        }
        else
        {
            _rootNavigationService.NavigateTo(RootNavigationConstant.MainView);

            _menuNavigationService.NavigateTo(MenuNavigationConstant.TodayView);
        }
    }
}