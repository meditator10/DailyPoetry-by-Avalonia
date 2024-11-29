using Dpa.Library.Services;
using Dpa.Library.ViewModels;
using System;

namespace DailyPoetryA.Client.Services;

public class RootNavigationService : IRootNavigationService
{
    public void NavigateTo(string view)
    {
        switch (view)
        {
            case RootNavigationConstant.InitializationView:
                ServiceLocator.Current.MainWindowViewModel.Content =
                    ServiceLocator.Current.InitializationViewModel;
                break;
            case RootNavigationConstant.MainView:
                ServiceLocator.Current.MainWindowViewModel.Content = 
                    ServiceLocator.Current.MainViewModel;
                break;

            default:
                throw new Exception("未知的视图。");
        }
    }
}

// TODO：测试
// public class RootNavigationService : IRootNavigationService
// {
//     private IMenuNavigationService _menuNavigationService;
//     public RootNavigationService(IMenuNavigationService menuNavigationService)
//     {
//         _menuNavigationService = menuNavigationService;
//     }
//     public void NavigateTo(string view)
//     {
//         if (view == RootNavigationConstant.MainView)
//         {
//             // 因为在ServiceLocator类中定义了Current属性，所以可以在全局拿到ServiceLocator实例
//             ServiceLocator.Current.MainWindowViewModel.Content = 
//                 ServiceLocator.Current.MainViewModel;
//             // ServiceLocator.Current.MainViewModel.Content = 
//             //     ServiceLocator.Current.TodayViewModel;
//             //ServiceLocator.Current.MainViewModel.PushContent(ServiceLocator.Current.TodayViewModel);
//
//             _menuNavigationService.NavigateTo(MenuNavigationConstant.TodayView);
//         }
//     }
// }