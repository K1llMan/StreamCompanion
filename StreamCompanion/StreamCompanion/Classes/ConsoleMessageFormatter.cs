using CompanionPlugin.Extensions;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace StreamCompanion.Classes;

public class ConsoleMessageFormatter : ConsoleFormatter, IDisposable
{
    #region Поля

    private string layout;
    private int categoryCount = 2;
    private string timeFormat;

    private readonly IDisposable optionsReloadToken;
    private ConsoleMessageFormatterOptions options;

    #endregion Поля

    #region Вспомогательные функции

    private string UpdateStringFormat(string logLayout)
    {
        Dictionary<string, string> replaces = new() {
            { "{newline}", "\r\n" },
            { "{category.*?}", "{0}" },
            { "{time.*?}", "{1}" },
            { "{kind}", "{2}" },
            { "{msg}", "{3}" }
        };

        foreach (string key in replaces.Keys)
        {
            // Для категорий может выводиться количество слов
            if (key.IsMatch("category"))
            {
                string categoryStr = logLayout.GetMatches(key).FirstOrDefault();
                if (!string.IsNullOrEmpty(categoryStr))
                {
                    string catCount = categoryStr.GetMatches(@"(?<=\:)[\d]+").FirstOrDefault();
                    categoryCount = string.IsNullOrEmpty(catCount) ? categoryCount : Convert.ToInt32(catCount);
                }
            }

            // Для времени можно задать маску
            if (key.IsMatch("time"))
            {
                string timeStr = logLayout.GetMatches(key).FirstOrDefault();
                if (!string.IsNullOrEmpty(timeStr))
                {
                    string timeFormatStr = timeStr.GetMatches(@"(?<=\:).*(?=\})").FirstOrDefault();
                    timeFormat = timeFormatStr ?? timeFormat;
                }
            }

            logLayout = logLayout.ReplaceRegex(key, replaces[key]);
        }

        return logLayout;
    }

    private string GetLogLevelString(LogLevel level)
    {
        return level switch
        {
            LogLevel.None => "Сообщение",
            LogLevel.Trace => "Сообщение",
            LogLevel.Debug => "Отладка",
            LogLevel.Information => "Информация",
            LogLevel.Warning => "Предупреждение",
            LogLevel.Error => "Ошибка",
            LogLevel.Critical => "Критическая ошибка",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
    }

    private void ReloadLoggerOptions(ConsoleMessageFormatterOptions formatterOptions)
    {
        options = formatterOptions;
    }

    #endregion Вспомогательные функции

    public ConsoleMessageFormatter(IOptionsMonitor<ConsoleMessageFormatterOptions> formatterOptions) : base("ConsoleMessageFormatter")
    {
        (optionsReloadToken, options) = (formatterOptions.OnChange(ReloadLoggerOptions), formatterOptions.CurrentValue);
        layout = UpdateStringFormat(formatterOptions.CurrentValue.Layout);
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        string message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

        if (message is null)
            return;

        string category = logEntry.Category;
        string[] words = category.Split('.');
        category = string.Join(".", words.Skip(words.Length - Math.Min(words.Length, categoryCount)));

        string formattedMessage = string.Format(layout, category, DateTime.Now.ToString(timeFormat), GetLogLevelString(logEntry.LogLevel), message);
        textWriter.WriteLine(formattedMessage);
    }


    public void Dispose()
    {
        optionsReloadToken?.Dispose();
    }
}