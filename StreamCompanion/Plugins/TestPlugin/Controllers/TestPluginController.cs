using CompanionPlugin.Controllers;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;
using CompanionPlugin.Services;

using Microsoft.AspNetCore.Mvc;

using TestPlugin.Classes;
using TestPlugin.Services;

namespace TestPlugin.Controllers;

public class TestPluginController : BaseServiceController<TestServiceConfig>
{
    #region Поля

    private TestSourceService testSourceService;

    #endregion Поля

    public TestPluginController(ServiceResolver serviceResolver, IWritableOptions<TestServiceConfig> config) : base(config)
    {
        testSourceService = serviceResolver.Resolve<ICommandSourceService, TestSourceService>();
    }

    [HttpGet("version")]
    public object Version()
    {
        return "Plugin Controller v 1.0";
    }

    [HttpPost("message")]
    [Consumes("text/plain")]
    public object Message([FromBody]string message, UserRole role)
    {
        return testSourceService.AddMessage(message, role);
    }
}