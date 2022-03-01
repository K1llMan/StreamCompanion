using Microsoft.Extensions.Options;

namespace CompanionPlugin.Interfaces;

public interface IWritableOptions<out T> : IOptions<T> where T : class, new()
{
    void Update(Action<T> applyChanges);
    void OnChange(Action<T> listener);
}