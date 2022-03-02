using Microsoft.Extensions.Options;

namespace CompanionPlugin.Interfaces;

public interface IWritableOptions<T> : IOptions<T> where T : class, new()
{
    void Update(Func<T,T> applyChanges);
    void OnChange(Action<T> listener);
}