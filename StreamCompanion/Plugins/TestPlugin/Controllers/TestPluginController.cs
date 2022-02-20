using CompanionPlugin.Enums;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using TestPlugin.Service;

namespace TestPlugin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestPluginController : ControllerBase
    {
        #region Поля

        private TestService service;
        private ILogger log;

        #endregion Поля

        public TestPluginController(TestService testService, ILogger<TestPluginController> logger)
        {
            service = testService;
            log = logger;
        }

        [HttpGet("version")]
        public object Version()
        {
            return "Plugin Controller v 1.0";
        }

        [HttpPost("message")]
        public object Message(string message, UserRole role)
        {
            return service.ProcessCommand(message, role);
        }
    }
}