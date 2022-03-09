using Microsoft.Extensions.Logging.Console;

namespace StreamCompanion.Classes;

public class ConsoleMessageFormatterOptions : ConsoleFormatterOptions
{
    public string Layout { get; set; }
}