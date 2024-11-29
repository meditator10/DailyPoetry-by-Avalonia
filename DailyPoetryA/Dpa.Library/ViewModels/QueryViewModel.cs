using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dpa.Library.Models;
using Dpa.Library.Services;

namespace Dpa.Library.ViewModels;

public class QueryViewModel : ViewModelBase
{

    public ObservableCollection<FilterViewModel> FileterViewModelCollection
    {
        get;
    }

    private readonly IContentNavigationService _contentNavigationService;

    public QueryViewModel(IContentNavigationService contentNavigationService)
    {
        _contentNavigationService = contentNavigationService;
        QueryCommand = new RelayCommand(Query);
    }

    public IRelayCommand QueryCommand { get; }
    public void Query()
    {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.ResultView);
    }
}

public class FilterViewModel : ObservableObject
{
    private string _content;

    public string Content
    {
        get => _content; 
        set => SetProperty(ref _content, value);
    }
}