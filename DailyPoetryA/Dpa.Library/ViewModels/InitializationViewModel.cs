using CommunityToolkit.Mvvm.Input;
using Dpa.Library.Services;

namespace Dpa.Library.ViewModels;

public class InitializationViewModel : ViewModelBase
{
    private readonly IPoetryStorage _poetryStorage;
    private readonly IRootNavigationService _rootNavigationService;
    private readonly IMenuNavigationService _menuNavigationService;

    public InitializationViewModel(IPoetryStorage poetryStorage,
        IRootNavigationService rootNavigationService,
        IMenuNavigationService menuNavigationService)
    {
        _poetryStorage = poetryStorage;
        _rootNavigationService = rootNavigationService;
        _menuNavigationService = menuNavigationService;

        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
    }

    private IAsyncRelayCommand OnInitializedCommand { get; }

    public async Task OnInitializedAsync()
    {
        if (!_poetryStorage.IsInitialized)
        {
            await _poetryStorage.InitializeAsync();
        }

        await Task.Delay(1000);

        _rootNavigationService.NavigateTo(RootNavigationConstant.MainView);
        _menuNavigationService.NavigateTo(MenuNavigationConstant.TodayView);
    }
}