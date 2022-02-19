using Microsoft.Extensions.DependencyInjection;

namespace CompanionPlugin;

public interface ICompanionPlugin
{
    void Init(IServiceCollection services);
}