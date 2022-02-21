using CompanionPlugin.Enums;
using CompanionPlugin.Interfaces;
using CompanionPlugin.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using TestPlugin.Services;

namespace TestPlugin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestPluginController : ControllerBase
    {
        #region Поля

        private TestSourceService testSourceService;
        private ILogger log;

        #endregion Поля

        public TestPluginController(ServiceResolver serviceResolver, ILogger<TestPluginController> logger)
        {
            testSourceService = serviceResolver.Resolve<ICommandSourceService, TestSourceService>();
            log = logger;
        }

        [HttpGet("version")]
        [TypeFilter(typeof(TestService))]
        public object Version()
        {
            return "Plugin Controller v 1.0";
        }

        [HttpPost("message")]
        public object Message(string message, UserRole role)
        {
            return testSourceService.AddMessage(message, role);
        }
    }
}