using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Dpa.Library.Models;
using Dpa.Library.Services;
using System.Windows.Input;

namespace Dpa.Library.ViewModels;

public class FavoriteViewModel : ViewModelBase
{
    private readonly IFavoriteStorage _favoriteStorage;

    private readonly IPoetryStorage _poetryStorage;

    private readonly IContentNavigationService _contentNavigationService;

    public FavoriteViewModel(IFavoriteStorage favoriteStorage,
        IPoetryStorage poetryStorage,
        IContentNavigationService contentNavigationService)
    {
        _favoriteStorage = favoriteStorage;
        _poetryStorage = poetryStorage;
        _contentNavigationService = contentNavigationService;

        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
        ShowPoetryCommand = new RelayCommand<Poetry>(ShowPoetry);
    }
    
    public ObservableCollection<PoetryFavorite> PoetryFavoriteCollection { get; } = new();

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private bool _isLoading;

    public IRelayCommand OnInitializedCommand { get; }

    public async Task OnInitializedAsync()
    {
        IsLoading = true;

        PoetryFavoriteCollection.Clear();
        var favoriteList = await _favoriteStorage.GetFavoritesAsync();

        var poetryFavorites = (await Task.WhenAll(
            favoriteList.Select(p => Task.Run(async () => new PoetryFavorite
            {
                Poetry = await _poetryStorage.GetPoetryAsync(p.PoetryId),
                Favorite = p
            })))).ToList();

        foreach (var poetryFavorite in poetryFavorites)
        {
            PoetryFavoriteCollection.Add(poetryFavorite);
        }

        IsLoading = false;
    }

    public IRelayCommand<Poetry> ShowPoetryCommand { get; }

    public void ShowPoetry(Poetry poetry) =>
        _contentNavigationService.NavigateTo(ContentNavigationConstant.DetailView, poetry);
}

public class PoetryFavorite
{
    public Poetry Poetry { get; set; }

    public Favorite Favorite { get; set; }
}