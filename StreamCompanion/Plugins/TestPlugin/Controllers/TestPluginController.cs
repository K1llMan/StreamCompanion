using Microsoft.AspNetCore.Mvc;

namespace TestPlugin.Controllers;

public class TestPluginController
{

    [HttpGet("version")]
    public object Version()
    {
        return "Plugin Controller v 1.0";
    }
}