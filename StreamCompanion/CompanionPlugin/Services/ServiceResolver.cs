using Microsoft.Extensions.DependencyInjection;

namespace CompanionPlugin.Services;

/// <summary>
/// Сервис для поиска конкрентных реализаций сервисов в контейнере
/// </summary>
public class ServiceResolver
{
    #region Поля

    private IServiceProvider serviceProvider;

    #endregion Поля

    #region Основные функции

    public ServiceResolver(IServiceProvider provider)
    {
        serviceProvider = provider;
    }

    /// <summary>
    /// Получение конкретной реализации типа <typeparamref name="T"/> для интерфейса <typeparamref name="IT"/>
    /// </summary>
    /// <typeparam name="IT">Интерфейс</typeparam>
    /// <typeparam name="T">Тип</typeparam>
    /// <returns></returns>
    public T Resolve<IT, T>() where T : IT
    {
        IEnumerable<IT> services = serviceProvider.GetServices<IT>();

        return (T)services.FirstOrDefault(s => s.GetType().IsAssignableTo(typeof(T)));
    }

    /// <summary>
    /// Получение списка сервисов, реализующих интерфейс <typeparamref name="IT"/>
    /// </summary>
    /// <typeparam name="IT">Тип интерфейса</typeparam>
    /// <returns>Список сервисов</returns>
    public List<IT> Resolve<IT>()
    {
        return serviceProvider.GetServices<IT>().ToList();
    }

    #endregion Основные функции
}