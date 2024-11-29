using System.Reflection;

namespace Dpa.Library.Helpers;

public class PathHelper
{
    private static string _localFolder = string.Empty;

    // 获取绝对路径
    private static string LocalFolder
    {
        get
        {
            // 懒式初始化
            if (!string.IsNullOrEmpty(_localFolder))
            {
                return _localFolder;
            }

            //string projectName = Assembly.GetExecutingAssembly().GetName().Name;
            _localFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                nameof(Dpa));

            if (!Directory.Exists(_localFolder))
            {
                Directory.CreateDirectory(_localFolder);
            }

            return _localFolder;
        }
    }

    // 绝对路径 + 文件名
    public static string GetLocalFilePath(string fileName)
    {
        return Path.Combine(LocalFolder, fileName);
    }
}