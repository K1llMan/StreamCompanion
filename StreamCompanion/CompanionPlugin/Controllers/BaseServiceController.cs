using CompanionPlugin.Interfaces;

using Json.Schema;

using Microsoft.AspNetCore.Mvc;

namespace CompanionPlugin.Controllers;

/// <summary>
/// Базовый контроллер для работы с сервисами
/// </summary>
/// <typeparam name="T">Тип конфигурации сервиса</typeparam>
[ApiController]
[Route("api/[controller]")]
public class BaseServiceController<T> : Controller where T: class, IServiceSettings,  new()
{
    #region Поля

    protected IWritableOptions<T> config;

    #endregion Поля

    public BaseServiceController(IWritableOptions<T> serviceConfig)
    {
        config = serviceConfig;
    }

    [HttpGet("config")]
    public T GetConfig()
    {
        return config.Value;
    }

    [HttpPost("config")]
    public void UpdateConfig(T newConfig)
    {
        config.Update(c => newConfig);
    }

    [HttpGet("config/schema")]
    public JsonSchema GetSchema()
    {
        return config.Value.Schema;
    }
}