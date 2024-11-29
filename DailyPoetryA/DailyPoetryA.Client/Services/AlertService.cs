using System.Threading.Tasks;
using Dpa.Library.Services;
using Ursa.Controls;

namespace DailyPoetryA.Client.Services;

public class AlertService : IAlertService
{
    public async Task AlertAsync(string title, string message)
    {
        await MessageBox.ShowAsync(message, title, button:MessageBoxButton.OK);
    }
}