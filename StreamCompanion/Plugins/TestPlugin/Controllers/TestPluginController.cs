using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;
using CompanionPlugin.Services;

using Microsoft.AspNetCore.Mvc;

using TestPlugin.Services;

namespace TestPlugin.Controllers;

public class TestPluginController
{
    #region Поля

    private TestSourceService testSourceService;

    #endregion Поля

    public TestPluginController(ServiceResolver serviceResolver)
    {
        testSourceService = serviceResolver.Resolve<ICommandSourceService, TestSourceService>();
    }

    [HttpPost("message")]
    [Consumes("text/plain")]
    public object Message([FromBody] string message, UserRole role)
    {
        return testSourceService.AddMessage(message, role);
    }

    [HttpGet("version")]
    public object Version()
    {
        return "Plugin Controller v 1.0";
    }
}