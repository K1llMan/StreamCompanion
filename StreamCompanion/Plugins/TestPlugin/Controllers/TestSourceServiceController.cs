using CompanionPlugin.Controllers;
using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;
using CompanionPlugin.Services;

using Microsoft.AspNetCore.Mvc;

using TestPlugin.Classes;
using TestPlugin.Services;

namespace TestPlugin.Controllers;

public class TestSourceServiceController : BaseServiceController<TestSourceServiceConfig>
{
    #region Поля

    private TestSourceService testSourceService;

    #endregion Поля

    public TestSourceServiceController(ServiceResolver serviceResolver, IWritableOptions<TestSourceServiceConfig> config) : base(config)
    {
        testSourceService = serviceResolver.Resolve<ICommandSourceService, TestSourceService>();
    }

    [HttpPost("message")]
    [Consumes("text/plain")]
    public object Message([FromBody] string message, UserRole role)
    {
        return testSourceService.AddMessage(message, role);
    }
}