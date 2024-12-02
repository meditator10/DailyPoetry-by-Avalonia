using CommunityToolkit.Mvvm.Input;
using Dpa.Library.Services;

namespace Dpa.Library.ViewModels;

public class InitializationViewModel : ViewModelBase
{
    private readonly IPoetryStorage _poetryStorage;
    private readonly IRootNavigationService _rootNavigationService;
    private readonly IMenuNavigationService _menuNavigationService;
    private readonly IFavoriteStorage _favoriteStorage;

    public InitializationViewModel(IPoetryStorage poetryStorage,
        IRootNavigationService rootNavigationService,
        IMenuNavigationService menuNavigationService,
        IFavoriteStorage favoriteStorage)
    {
        _poetryStorage = poetryStorage;
        _rootNavigationService = rootNavigationService;
        _menuNavigationService = menuNavigationService;

        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
        _favoriteStorage = favoriteStorage;
    }

    private IAsyncRelayCommand OnInitializedCommand { get; }

    public async Task OnInitializedAsync()
    {
        if (!_poetryStorage.IsInitialized)
        {
            await _poetryStorage.InitializeAsync();
        }

        if (!_favoriteStorage.IsInitialized)
        {
            await _favoriteStorage.InitializeAsync();
        }

        await Task.Delay(1000);

        _rootNavigationService.NavigateTo(RootNavigationConstant.MainView);
        _menuNavigationService.NavigateTo(MenuNavigationConstant.TodayView);
    }
}