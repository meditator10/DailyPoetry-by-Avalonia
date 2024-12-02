using CommunityToolkit.Mvvm.Input;
using Dpa.Library.Models;
using Dpa.Library.Services;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows.Input;
using AvaloniaInfiniteScrolling;

namespace Dpa.Library.ViewModels;

public class ResultViewModel : ViewModelBase
{
    private readonly IPoetryStorage _poetryStorage;
    private readonly IContentNavigationService _contentNavigationService;

    public ResultViewModel(IPoetryStorage poetryStorage, 
        IContentNavigationService contentNavigationService)
    {
        _poetryStorage = poetryStorage;
        _contentNavigationService = contentNavigationService;
        ShowPoetryCommand = new RelayCommand<Poetry>(ShowPoetry);

        PoetryCollection = new AvaloniaInfiniteScrollCollection<Poetry>();
        PoetryCollection.OnCanLoadMore = () => _canLoadMore;
        PoetryCollection.OnLoadMore = async () =>
        {
            // 状态先设置为正在加载
            Status = Loading;
            var poetries = await _poetryStorage.QueryAsync(
                Expression.Lambda<Func<Poetry, bool>>(Expression.Constant(true),
                    Expression.Parameter(typeof(Poetry), "p")),
                PoetryCollection.Count,
                PageSize);
            // 记得把状态清空
            Status = string.Empty;

            if (poetries.Count < PageSize)
            {
                _canLoadMore = false;
                Status = NoMoreResult;
            }

            if (PoetryCollection.Count == 0 && poetries.Count == 0)
            {
                Status = NoResult;
            }

            return poetries;
        };
    }

    private const int PageSize = 20;
    public AvaloniaInfiniteScrollCollection<Poetry> PoetryCollection { get; }

    private string _status;

    public string Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    private bool _canLoadMore = true;
    public const string Loading = "正在载入";
    public const string NoResult = "没有满足条件的结果";
    public const string NoMoreResult = "没有更多结果";

    // 用于搜索结果的导航
    public IRelayCommand<Poetry> ShowPoetryCommand { get; }
    public void ShowPoetry(Poetry poetry) => 
        _contentNavigationService.NavigateTo(ContentNavigationConstant.DetailView, poetry);
}

/* 测试
    /// <summary>
    /// 返回结果
    /// </summary>
    // 只初始化一次，所以每次调用这个属性返回同一个Collection
    public ObservableCollection<Poetry> PoetryCollection { get; } = new ObservableCollection<Poetry>();

    // 每次调用这个属性都会返回一个新Collection
    // public ObservableCollection<Poetry> Poetries2 => new ObservableCollection<Poetry>();
    // public ObservableCollection<Poetry> Poetries2
    // {
    //     get { return new ObservableCollection<Poetry>(); }
    // }

    public ICommand OnInitializedCommand { get; }
    public async Task OnInitializedAsync()
    {
        // TODO 仅供测试使用，正式版请删除
        await _poetryStorage.InitializeAsync();

        var poetries = await _poetryStorage.QueryAsync(
            Expression.Lambda<Func<Poetry, bool>>(Expression.Constant(true),
                Expression.Parameter(typeof(Poetry), "p")),
            0,
            int.MaxValue);

        // 因为 ObservableCollection类 没有 AddAll方法 将数据批量加入集合里，所以需要遍历
        foreach (var poetry in poetries)
        {
            PoetryCollection.Add(poetry);
        }
    }
*/

