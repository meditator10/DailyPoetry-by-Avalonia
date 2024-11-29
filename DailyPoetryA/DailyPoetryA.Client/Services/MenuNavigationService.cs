using System;
using Dpa.Library.Services;
using Dpa.Library.ViewModels;

namespace DailyPoetryA.Client.Services;

public class MenuNavigationService : IMenuNavigationService
{
    public void NavigateTo(string view)
    {
        // switch 语句
        ViewModelBase viewModel = null;
        switch (view)
        {
            case MenuNavigationConstant.TodayView:
                viewModel = ServiceLocator.Current.TodayViewModel;
                break;
            case MenuNavigationConstant.QueryView:
                viewModel = ServiceLocator.Current.QueryViewModel;
                break;
            case MenuNavigationConstant.FavoriteView:
                viewModel = ServiceLocator.Current.FavoriteViewModel;
                break;

            default:
                throw new Exception("未知视图");
        }

        ServiceLocator.Current.MainViewModel.SetMenuAndContent(view, viewModel);

        // switch表达式
        // ViewModelBase viewModel = view switch
        // {
        //     MenuNavigationConstant.TodayView => ServiceLocator.Current
        //         .TodayViewModel,
        //     MenuNavigationConstant.QueryView => ServiceLocator.Current
        //         .QueryViewModel,
        //     MenuNavigationConstant.FavoriteView => ServiceLocator.Current
        //         .FavoriteViewModel,
        //     _ => throw new Exception("未知的视图。")
        // };
    }
}