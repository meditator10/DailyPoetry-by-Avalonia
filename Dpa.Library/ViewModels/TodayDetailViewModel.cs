using CommunityToolkit.Mvvm.Input;
using Dpa.Library.Models;
using Dpa.Library.Services;
using System.Windows.Input;

namespace Dpa.Library.ViewModels;

public class TodayDetailViewModel : ViewModelBase
{
    private TodayPoetry _todayPoetry;

    public TodayPoetry TodayPoetry
    {
        get => _todayPoetry;
        private set => SetProperty(ref _todayPoetry, value);
    }

    public override void SetParameter(object parameter)
    {
        TodayPoetry = parameter as TodayPoetry;
    }
}