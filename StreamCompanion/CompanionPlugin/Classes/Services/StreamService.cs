using System.Reflection;

using CompanionPlugin.Classes.Attributes;
using CompanionPlugin.Interfaces;

namespace CompanionPlugin.Classes.Services;

public class StreamService<T> : IStreamService where T : class, IServiceSettings, new()
{
    #region Поля

    protected IWritableOptions<T> config;

    #endregion Поля

    #region Вспомогательные функции

    /// <summary>
    /// Установка и настройка конфигурации
    /// </summary>
    /// <param name="serviceConfig">Конфигурация сервиса</param>
    protected void SetConfig(IWritableOptions<T> serviceConfig)
    {
        config = serviceConfig;
        // Переинициализация сервиса при изменении опций
        config.OnChange(c => {
            Dispose();
            Init();
        });
    }

    #endregion Вспомогательные функции

    #region IStreamService

    public virtual Dictionary<string, object> GetDescription()
    {
        StreamServiceAttribute desc = GetType().GetCustomAttribute<StreamServiceAttribute>();

        return new Dictionary<string, object> {
            { "name", desc?.Description ?? string.Empty }
        };
    }

    public virtual void Init()
    {

    }

    #endregion IStreamService

    #region IDisposable

    public virtual void Dispose()
    {
    }

    #endregion IDisposable
}