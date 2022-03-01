namespace CompanionPlugin.Interfaces;

/// <summary>
/// Интерфейс сервиса для стримов
/// </summary>
public interface IStreamService : IDisposable
{
    virtual void Init() { }
    Dictionary<string, object> GetDescription();
}