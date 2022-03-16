using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace NAudioPlayer.Classes.Providers;

public class CommonProvider
{
    #region Поля

    #endregion Поля

    #region Вспомогательные функции

    protected string GetTempFileName(string artist, string title)
    {
        using MD5 md5 = MD5.Create();
        string data = $"{artist} - {title}";
        byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(data));

        return new Guid(hash).ToString();
    }

    protected bool CheckUrl(string url, string pattern)
    {
        Regex regex = new(pattern);

        return regex.IsMatch(url);
    }

    protected void CheckDirectory(string fileName)
    {
        string dir = Path.GetDirectoryName(fileName);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    #endregion Вспомогательные функции
}