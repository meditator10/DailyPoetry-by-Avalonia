using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Dpa.Library.Models;
using Dpa.Library.Services;

namespace Dpa.Library.ViewModels;

public class TodayViewModel : ViewModelBase
{
    private readonly ITodayPoetryService _todayPoetryService;
    private readonly ITodayImageService _todayImageService;
    private readonly IContentNavigationService _contentNavigationService;

    public TodayViewModel(ITodayPoetryService todayPoetryService, 
        ITodayImageService todayImageService,
        IContentNavigationService contentNavigationService)
    {
        _todayPoetryService = todayPoetryService;
        _todayImageService = todayImageService;
        _contentNavigationService = contentNavigationService;

        OnInitializedCommand = new RelayCommand(OnInitialized);
        ShowDetailCommand = new RelayCommand(ShowDetail);
    }

    private TodayPoetry _todayPoetry;
    public TodayPoetry TodayPoetry
    {
        get => _todayPoetry; 
        set => SetProperty(ref _todayPoetry, value);
    }

    private TodayImage _todayImage;
    public TodayImage TodayImage
    {
        get => _todayImage;
        private set => SetProperty(ref _todayImage, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public IRelayCommand OnInitializedCommand { get; }

    public void OnInitialized()
    {
        // 获取bing服务器每日图片
        Task.Run(async () => {
            // 先从本地获取图片
            TodayImage = await _todayImageService.GetTodayImageAsync();
            // 检查图片更新
            var updateResult = await _todayImageService.CheckUpdateAsync();
            if (updateResult.HasUpdate)
            {
                TodayImage = updateResult.TodayImage;
            }
        });
        // 获取今日诗词服务器每日诗词
        Task.Run(async () => {
            IsLoading = true;
            //await Task.Delay(1000);
            TodayPoetry = await _todayPoetryService.GetTodayPoetryAsync();
            IsLoading = false;
        });
    }

    public IRelayCommand ShowDetailCommand { get; }

    public void ShowDetail()
    {
        _contentNavigationService.NavigateTo(
            ContentNavigationConstant.TodayDetailView, TodayPoetry);
    }
}