using CompanionPlugin.Interfaces;

using Microsoft.Extensions.DependencyInjection;

using TestPlugin.Service;

namespace TestPlugin
{
    public class Test : ICompanionPlugin
    {
        #region Основные функции

        public Test()
        {
            Name = "Test Plugin";
            Version = new Version(1, 0, 0, 0);
        }

        #endregion Основные функции

        #region ICompanionPlugin

        public string Name { get; }
        public Version Version { get; }

        public void Init(IServiceCollection services)
        {
            services.AddSingleton<ICommandService, TestService>();
        }

        #endregion ICompanionPlugin
    }
}
