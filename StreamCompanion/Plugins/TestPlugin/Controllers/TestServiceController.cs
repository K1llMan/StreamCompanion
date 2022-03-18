using CompanionPlugin.Controllers;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;
using CompanionPlugin.Services;

using Microsoft.AspNetCore.Mvc;

using TestPlugin.Classes;
using TestPlugin.Services;

namespace TestPlugin.Controllers;

public class TestServiceController : BaseServiceController<TestServiceConfig>
{
    #region Поля

    private TestService testService;

    #endregion Поля

    public TestServiceController(ServiceResolver serviceResolver, IWritableOptions<TestServiceConfig> config) : base(config)
    {
        testService = serviceResolver.Resolve<ICommandService, TestService>();
    }
}