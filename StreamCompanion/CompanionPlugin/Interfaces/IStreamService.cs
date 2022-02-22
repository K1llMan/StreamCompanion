namespace CompanionPlugin.Interfaces;

/// <summary>
/// Интерфейс сервиса для стримов
/// </summary>
public interface IStreamService
{
    Dictionary<string, object> GetDescription();
}