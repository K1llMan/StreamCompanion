using CompanionPlugin;

using Microsoft.Extensions.DependencyInjection;

namespace TestPlugin.Services
{
    public class Test : ICompanionPlugin
    {
        public void Init(IServiceCollection services)
        {
        }
    }
}
