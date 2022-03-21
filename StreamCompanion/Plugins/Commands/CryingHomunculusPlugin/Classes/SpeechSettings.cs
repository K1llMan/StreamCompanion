using Json.Schema.Generation;

namespace CryingHomunculusPlugin.Classes;

public class SpeechSettings
{
    [Title("Путь к Балаболке")]
    public string Path { get; set; }
    [Title("Путь для кэша")]
    public string CachePath { get; set; }
    [Title("Идентификатор голоса")]
    public string Voice { get; set; }
}