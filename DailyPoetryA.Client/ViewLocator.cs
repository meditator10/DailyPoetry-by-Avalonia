using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using Dpa.Library.ViewModels;

namespace DailyPoetryA.Client
{
    public class ViewLocator : IDataTemplate
    {

        public Control? Build(object? data)
        {
            if (data is null)
                return null;

            // 将ViewModel映射为View
            var name = data.GetType().FullName!
                .Replace("ViewModel", "View", StringComparison.Ordinal)
                .Replace("Dpa.Library", "DailyPoetryA.Client");
            var type = Type.GetType(name);

            if (type != null)
            {
                var control = (Control)Activator.CreateInstance(type)!;
                control.DataContext = data;
                return control;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
