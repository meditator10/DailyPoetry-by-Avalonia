﻿namespace Dpa.Library.Services;

public interface IContentNavigationService
{
    public void NavigateTo(string view, object parameter = null);
}

public static class ContentNavigationConstant
{
    public const string TodayDetailView = nameof(TodayDetailView);

    public const string ResultView = nameof(ResultView);

    public const string DetailView = nameof(DetailView);
}